using Futelo.Server.Models;
using Futelo.Server.Repositories.League;

namespace Futelo.Server.Services.League;

public class LeagueService(ILeagueRepository leagueRepository) : ILeagueService
{
    public async Task GenerateFixtureAsync(int leagueId, string userId)
    {
        var league = await leagueRepository.GetByIdAsync(leagueId);
        if (league == null || league.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("League not found.");

        if (league.Status != TournamentStatus.NotStarted)
            throw new InvalidOperationException("Fixture has already been generated.");

        var playerIds = league.Season.Players.Select(sp => sp.PlayerId).ToList();
        if (playerIds.Count < 2)
            throw new InvalidOperationException("At least 2 players are required.");

        var leaguePlayers = playerIds.Select(pid => new LeaguePlayer
        {
            LeagueId = leagueId,
            PlayerId = pid
        }).ToList();

        var matches = BuildRoundRobin(playerIds, leagueId, league.IsHomeAndAway);

        await leagueRepository.SetFixtureAsync(leagueId, leaguePlayers, matches);
        await leagueRepository.UpdateStatusAsync(leagueId, TournamentStatus.Active);
    }

    public async Task RegenerateFixtureAsync(int leagueId, string userId)
    {
        var league = await leagueRepository.GetByIdAsync(leagueId);
        if (league == null || league.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("League not found.");

        if (league.Matches.Any(m => m.Status == MatchStatus.Played))
            throw new InvalidOperationException("Cannot regenerate fixture after results have been recorded.");

        var playerIds = league.Season.Players.Select(sp => sp.PlayerId).ToList();
        var shuffled = playerIds.OrderBy(_ => Random.Shared.Next()).ToList();

        var leaguePlayers = shuffled.Select(pid => new LeaguePlayer
        {
            LeagueId = leagueId,
            PlayerId = pid
        }).ToList();

        var matches = BuildRoundRobin(shuffled, leagueId, league.IsHomeAndAway);

        await leagueRepository.SetFixtureAsync(leagueId, leaguePlayers, matches);
    }

    private static List<Match> BuildRoundRobin(List<string> playerIds, int leagueId, bool isHomeAndAway)
    {
        var slots = new List<string?>(playerIds.Cast<string?>());
        if (slots.Count % 2 != 0)
            slots.Add(null); // bye

        int n = slots.Count;
        int totalRounds = n - 1;
        var matches = new List<Match>();

        for (int round = 0; round < totalRounds; round++)
        {
            int matchday = round + 1;
            for (int i = 0; i < n / 2; i++)
            {
                string? home = slots[i];
                string? away = slots[n - 1 - i];

                if (home != null && away != null)
                {
                    matches.Add(new Match
                    {
                        LeagueId = leagueId,
                        HomePlayerId = home,
                        AwayPlayerId = away,
                        Status = MatchStatus.Pending,
                        Leg = matchday
                    });
                }
            }

            // Rotate: keep slots[0] fixed, move last element to position 1
            var last = slots[n - 1];
            for (int i = n - 1; i > 1; i--)
                slots[i] = slots[i - 1];
            slots[1] = last;
        }

        if (isHomeAndAway)
        {
            var returnMatches = matches
                .Select(m => new Match
                {
                    LeagueId = leagueId,
                    HomePlayerId = m.AwayPlayerId,
                    AwayPlayerId = m.HomePlayerId,
                    Status = MatchStatus.Pending,
                    Leg = m.Leg + totalRounds
                })
                .ToList();
            matches.AddRange(returnMatches);
        }

        return matches;
    }
}
