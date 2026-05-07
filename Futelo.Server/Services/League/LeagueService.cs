using Futelo.Server.Models;
using Futelo.Server.Repositories.League;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.Enums;

namespace Futelo.Server.Services.League;

public class LeagueService(ILeagueRepository leagueRepository) : ILeagueService
{
    public async Task<LeagueResponse> GetByIdAsync(int leagueId, string userId)
    {
        var league = await leagueRepository.GetByIdAsync(leagueId);
        if (league == null || league.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("League not found.");

        var standings = league.Status == TournamentStatus.NotStarted
            ? []
            : await GetStandingsAsync(leagueId, userId);

        return new LeagueResponse
        {
            Id = league.Id,
            SeasonId = league.SeasonId,
            Status = league.Status.ToString(),
            IsHomeAndAway = league.IsHomeAndAway,
            Matches = league.Matches
                .OrderBy(m => m.Leg).ThenBy(m => m.Id)
                .Select(m => new MatchResponse
                {
                    Id = m.Id,
                    HomePlayerId = m.HomePlayerId,
                    HomePlayerName = m.HomePlayer.DisplayName,
                    AwayPlayerId = m.AwayPlayerId,
                    AwayPlayerName = m.AwayPlayer.DisplayName,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    Status = m.Status.ToString(),
                    Matchday = m.Leg,
                    PlayedAt = m.PlayedAt
                }).ToList(),
            Standings = standings
        };
    }

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

    public async Task<RecordResultResponse> RecordResultAsync(int leagueId, int matchId, int homeScore, int awayScore, string userId)
    {
        var league = await leagueRepository.GetByIdAsync(leagueId);
        if (league == null || league.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("League not found.");

        if (league.Status != TournamentStatus.Active)
            throw new InvalidOperationException("League is not active.");

        var caller = league.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || (caller.Role != VaultRole.Admin && caller.Role != VaultRole.Editor))
            throw new UnauthorizedAccessException("Only vault admins and editors can record results.");

        var match = league.Matches.FirstOrDefault(m => m.Id == matchId);
        if (match == null)
            throw new KeyNotFoundException("Match not found.");
        if (match.Status == MatchStatus.Played)
            throw new InvalidOperationException("Match already has a result.");

        var seasonPlayers = league.Season.Players.ToList();
        var homesp = seasonPlayers.First(sp => sp.PlayerId == match.HomePlayerId);
        var awaysp = seasonPlayers.First(sp => sp.PlayerId == match.AwayPlayerId);

        double homeResult = homeScore > awayScore ? 1.0 : homeScore == awayScore ? 0.5 : 0.0;
        double awayResult = 1.0 - homeResult;
        int goalDiff = Math.Abs(homeScore - awayScore);

        var (homeSeasonChange, homeNewSeasonElo) = ComputeElo(homesp.SeasonElo, awaysp.SeasonElo, homeResult, goalDiff);
        var (awaySeasonChange, awayNewSeasonElo) = ComputeElo(awaysp.SeasonElo, homesp.SeasonElo, awayResult, goalDiff);

        int homeHistElo = homesp.Player.EloRating;
        int awayHistElo = awaysp.Player.EloRating;
        var (homeHistChange, homeNewHistElo) = ComputeElo(homeHistElo, awayHistElo, homeResult, goalDiff);
        var (awayHistChange, awayNewHistElo) = ComputeElo(awayHistElo, homeHistElo, awayResult, goalDiff);

        var seasonElos = seasonPlayers.ToDictionary(sp => sp.PlayerId, sp => sp.SeasonElo);
        int homeSeasonRankBefore = seasonElos.Count(kv => kv.Value > homesp.SeasonElo) + 1;
        int awaySeasonRankBefore = seasonElos.Count(kv => kv.Value > awaysp.SeasonElo) + 1;

        seasonElos[match.HomePlayerId] = homeNewSeasonElo;
        seasonElos[match.AwayPlayerId] = awayNewSeasonElo;
        int homeSeasonRankAfter = seasonElos.Count(kv => kv.Value > homeNewSeasonElo) + 1;
        int awaySeasonRankAfter = seasonElos.Count(kv => kv.Value > awayNewSeasonElo) + 1;

        var histElos = seasonPlayers.ToDictionary(sp => sp.PlayerId, sp => sp.Player.EloRating);
        int homeHistRankBefore = histElos.Count(kv => kv.Value > homeHistElo) + 1;
        int awayHistRankBefore = histElos.Count(kv => kv.Value > awayHistElo) + 1;

        histElos[match.HomePlayerId] = homeNewHistElo;
        histElos[match.AwayPlayerId] = awayNewHistElo;
        int homeHistRankAfter = histElos.Count(kv => kv.Value > homeNewHistElo) + 1;
        int awayHistRankAfter = histElos.Count(kv => kv.Value > awayNewHistElo) + 1;

        var now = DateTime.UtcNow;
        var histories = new List<EloHistory>
        {
            new() { PlayerId = match.HomePlayerId, MatchId = matchId, SeasonId = league.SeasonId, EloBefore = homesp.SeasonElo, EloAfter = homeNewSeasonElo, EloChange = homeSeasonChange, RankBefore = homeSeasonRankBefore, RankAfter = homeSeasonRankAfter, IsSeasonElo = true, CreatedAt = now },
            new() { PlayerId = match.HomePlayerId, MatchId = matchId, SeasonId = league.SeasonId, EloBefore = homeHistElo, EloAfter = homeNewHistElo, EloChange = homeHistChange, RankBefore = homeHistRankBefore, RankAfter = homeHistRankAfter, IsSeasonElo = false, CreatedAt = now },
            new() { PlayerId = match.AwayPlayerId, MatchId = matchId, SeasonId = league.SeasonId, EloBefore = awaysp.SeasonElo, EloAfter = awayNewSeasonElo, EloChange = awaySeasonChange, RankBefore = awaySeasonRankBefore, RankAfter = awaySeasonRankAfter, IsSeasonElo = true, CreatedAt = now },
            new() { PlayerId = match.AwayPlayerId, MatchId = matchId, SeasonId = league.SeasonId, EloBefore = awayHistElo, EloAfter = awayNewHistElo, EloChange = awayHistChange, RankBefore = awayHistRankBefore, RankAfter = awayHistRankAfter, IsSeasonElo = false, CreatedAt = now },
        };

        bool leagueFinished = league.Matches.Count(m => m.Status == MatchStatus.Pending) == 1;

        Dictionary<string, int> finalPositions = [];
        if (leagueFinished)
        {
            var allPlayedWithNew = league.Matches
                .Where(m => m.Status == MatchStatus.Played)
                .Append(new Match
                {
                    HomePlayerId = match.HomePlayerId,
                    AwayPlayerId = match.AwayPlayerId,
                    HomeScore = homeScore,
                    AwayScore = awayScore,
                    Status = MatchStatus.Played
                })
                .ToList();

            var finalStandings = ComputeStandings(allPlayedWithNew, league.Players);
            finalPositions = finalStandings
                .Select((row, i) => (row.PlayerId, Position: i + 1))
                .ToDictionary(x => x.PlayerId, x => x.Position);
        }

        await leagueRepository.SaveMatchResultAsync(new MatchResultData
        {
            MatchId = matchId,
            HomeScore = homeScore,
            AwayScore = awayScore,
            LeagueId = leagueId,
            SeasonId = league.SeasonId,
            HomePlayerId = match.HomePlayerId,
            HomeNewSeasonElo = homeNewSeasonElo,
            HomeNewHistoricalElo = homeNewHistElo,
            AwayPlayerId = match.AwayPlayerId,
            AwayNewSeasonElo = awayNewSeasonElo,
            AwayNewHistoricalElo = awayNewHistElo,
            EloHistories = histories,
            LeagueFinished = leagueFinished,
            FinalLeaguePositions = finalPositions
        });

        return new RecordResultResponse
        {
            Home = new EloChangeResult
            {
                PlayerId = match.HomePlayerId,
                DisplayName = homesp.Player.DisplayName,
                EloBefore = homesp.SeasonElo,
                EloAfter = homeNewSeasonElo,
                EloChange = homeSeasonChange,
                RankBefore = homeSeasonRankBefore,
                RankAfter = homeSeasonRankAfter
            },
            Away = new EloChangeResult
            {
                PlayerId = match.AwayPlayerId,
                DisplayName = awaysp.Player.DisplayName,
                EloBefore = awaysp.SeasonElo,
                EloAfter = awayNewSeasonElo,
                EloChange = awaySeasonChange,
                RankBefore = awaySeasonRankBefore,
                RankAfter = awaySeasonRankAfter
            }
        };
    }

    public async Task<List<StandingRow>> GetStandingsAsync(int leagueId, string userId)
    {
        var league = await leagueRepository.GetByIdAsync(leagueId);
        if (league == null || league.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("League not found.");

        var played = league.Matches.Where(m => m.Status == MatchStatus.Played).ToList();
        return ComputeStandings(played, league.Players);
    }

    private static List<StandingRow> ComputeStandings(List<Match> played, IEnumerable<LeaguePlayer> leaguePlayers)
    {
        var rows = leaguePlayers.Select(lp =>
        {
            string pid = lp.PlayerId;
            var home = played.Where(m => m.HomePlayerId == pid).ToList();
            var away = played.Where(m => m.AwayPlayerId == pid).ToList();

            int wins = home.Count(m => m.HomeScore > m.AwayScore) + away.Count(m => m.AwayScore > m.HomeScore);
            int draws = home.Count(m => m.HomeScore == m.AwayScore) + away.Count(m => m.HomeScore == m.AwayScore);
            int p = home.Count + away.Count;
            int gf = home.Sum(m => m.HomeScore ?? 0) + away.Sum(m => m.AwayScore ?? 0);
            int ga = home.Sum(m => m.AwayScore ?? 0) + away.Sum(m => m.HomeScore ?? 0);

            return new StandingRow
            {
                PlayerId = pid,
                DisplayName = lp.Player?.DisplayName ?? pid,
                Played = p,
                Won = wins,
                Drawn = draws,
                Lost = p - wins - draws,
                GoalsFor = gf,
                GoalsAgainst = ga,
                GoalDifference = gf - ga,
                Points = wins * 3 + draws
            };
        }).ToList();

        rows.Sort((a, b) =>
        {
            if (a.Points != b.Points) return b.Points.CompareTo(a.Points);
            if (a.GoalDifference != b.GoalDifference) return b.GoalDifference.CompareTo(a.GoalDifference);
            if (a.GoalsFor != b.GoalsFor) return b.GoalsFor.CompareTo(a.GoalsFor);

            var h2h = played.Where(m =>
                (m.HomePlayerId == a.PlayerId && m.AwayPlayerId == b.PlayerId) ||
                (m.HomePlayerId == b.PlayerId && m.AwayPlayerId == a.PlayerId)).ToList();

            if (h2h.Count > 0)
            {
                int aPts = h2h.Sum(m => m.HomePlayerId == a.PlayerId
                    ? (m.HomeScore > m.AwayScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0)
                    : (m.AwayScore > m.HomeScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0));
                int bPts = h2h.Sum(m => m.HomePlayerId == b.PlayerId
                    ? (m.HomeScore > m.AwayScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0)
                    : (m.AwayScore > m.HomeScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0));
                if (aPts != bPts) return bPts.CompareTo(aPts);
            }

            return string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase);
        });

        return rows;
    }

    private static (int change, int newElo) ComputeElo(int myElo, int opponentElo, double result, int goalDiff)
    {
        double expected = 1.0 / (1.0 + Math.Pow(10, (opponentElo - myElo) / 400.0));
        double multiplier = goalDiff >= 3 ? 1.5 : goalDiff == 2 ? 1.2 : 1.0;
        int change = (int)Math.Round(32 * multiplier * (result - expected), MidpointRounding.AwayFromZero);
        return (change, myElo + change);
    }

    private static List<Match> BuildRoundRobin(List<string> playerIds, int leagueId, bool isHomeAndAway)
    {
        var slots = new List<string?>(playerIds.Cast<string?>());
        if (slots.Count % 2 != 0)
            slots.Add(null);

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
