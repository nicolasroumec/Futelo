using Futelo.Server.Models;
using Futelo.Server.Repositories.SuperCup;
using Futelo.Shared.DTOs.SuperCup;
using Futelo.Shared.Enums;

namespace Futelo.Server.Services.SuperCup;

public class SuperCupService(ISuperCupRepository superCupRepository) : ISuperCupService
{
    public async Task<SuperCupResponse> GetByIdAsync(int superCupId, string userId)
    {
        var superCup = await superCupRepository.GetByIdAsync(superCupId);
        if (superCup == null || superCup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("SuperCup not found.");

        return MapToResponse(superCup);
    }

    public async Task StartAsync(int superCupId, string userId)
    {
        var superCup = await superCupRepository.GetByIdAsync(superCupId);
        if (superCup == null || superCup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("SuperCup not found.");

        if (superCup.Status != TournamentStatus.NotStarted)
            throw new InvalidOperationException("SuperCup has already been started.");

        var caller = superCup.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || (caller.Role != VaultRole.Admin && caller.Role != VaultRole.Editor))
            throw new UnauthorizedAccessException("Only vault admins and editors can start the SuperCup.");

        var league = superCup.Season.League;
        var cup = superCup.Season.Cup;

        if (league == null || league.Status != TournamentStatus.Finished || league.ChampionId == null)
            throw new InvalidOperationException("League must be finished before starting the SuperCup.");
        if (cup == null || cup.Status != TournamentStatus.Finished || cup.ChampionId == null)
            throw new InvalidOperationException("Cup must be finished before starting the SuperCup.");

        string player1Id = league.ChampionId;
        string player2Id;

        if (cup.ChampionId != league.ChampionId)
        {
            player2Id = cup.ChampionId;
        }
        else
        {
            var runnerUp = cup.Players.FirstOrDefault(cp => cp.CupPosition == 2);
            if (runnerUp == null)
                throw new InvalidOperationException("Could not determine Cup runner-up for SuperCup.");
            player2Id = runnerUp.PlayerId;
        }

        var matches = new List<Match>();

        matches.Add(new Match
        {
            SuperCupId = superCupId,
            HomePlayerId = player1Id,
            AwayPlayerId = player2Id,
            Status = MatchStatus.Pending,
            Leg = 1
        });

        if (superCup.IsHomeAndAway)
        {
            matches.Add(new Match
            {
                SuperCupId = superCupId,
                HomePlayerId = player2Id,
                AwayPlayerId = player1Id,
                Status = MatchStatus.Pending,
                Leg = 2
            });
        }

        await superCupRepository.SetParticipantsAsync(superCupId, player1Id, player2Id, matches);
    }

    public async Task<RecordSuperCupResultResponse> RecordResultAsync(
        int superCupId, int matchId, int homeScore, int awayScore,
        string? wonOnPenaltiesId, int? homePenaltyScore, int? awayPenaltyScore, string userId)
    {
        var superCup = await superCupRepository.GetByIdAsync(superCupId);
        if (superCup == null || superCup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("SuperCup not found.");

        if (superCup.Status != TournamentStatus.Active)
            throw new InvalidOperationException("SuperCup is not active.");

        var caller = superCup.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || (caller.Role != VaultRole.Admin && caller.Role != VaultRole.Editor))
            throw new UnauthorizedAccessException("Only vault admins and editors can record results.");

        if (homeScore < 0 || awayScore < 0)
            throw new InvalidOperationException("Scores cannot be negative.");

        var match = superCup.Matches.FirstOrDefault(m => m.Id == matchId);
        if (match == null)
            throw new KeyNotFoundException("Match not found in this SuperCup.");
        if (match.Status == MatchStatus.Played)
            throw new InvalidOperationException("Match already has a result.");
        if (string.IsNullOrEmpty(match.HomePlayerId) || string.IsNullOrEmpty(match.AwayPlayerId))
            throw new InvalidOperationException("Match participants are not yet determined.");

        // ELO
        const int k = 16;
        var seasonPlayers = superCup.Season.Players.ToList();
        var homesp = seasonPlayers.First(sp => sp.PlayerId == match.HomePlayerId);
        var awaysp = seasonPlayers.First(sp => sp.PlayerId == match.AwayPlayerId);

        double homeResult = homeScore > awayScore ? 1.0 : homeScore == awayScore ? 0.5 : 0.0;
        double awayResult = 1.0 - homeResult;
        int goalDiff = Math.Abs(homeScore - awayScore);

        if (homeScore == awayScore && wonOnPenaltiesId != null)
        {
            homeResult = wonOnPenaltiesId == match.HomePlayerId ? 1.0 : 0.0;
            awayResult = 1.0 - homeResult;
        }

        var (homeSeasonChange, homeNewSeasonElo) = ComputeElo(homesp.SeasonElo, awaysp.SeasonElo, homeResult, goalDiff, k);
        var (awaySeasonChange, awayNewSeasonElo) = ComputeElo(awaysp.SeasonElo, homesp.SeasonElo, awayResult, goalDiff, k);

        int homeHistElo = homesp.Player.EloRating;
        int awayHistElo = awaysp.Player.EloRating;
        var (homeHistChange, homeNewHistElo) = ComputeElo(homeHistElo, awayHistElo, homeResult, goalDiff, k);
        var (awayHistChange, awayNewHistElo) = ComputeElo(awayHistElo, homeHistElo, awayResult, goalDiff, k);

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
            new() { PlayerId = match.HomePlayerId, MatchId = matchId, SeasonId = superCup.SeasonId, EloBefore = homesp.SeasonElo, EloAfter = homeNewSeasonElo, EloChange = homeSeasonChange, RankBefore = homeSeasonRankBefore, RankAfter = homeSeasonRankAfter, IsSeasonElo = true, CreatedAt = now },
            new() { PlayerId = match.HomePlayerId, MatchId = matchId, SeasonId = superCup.SeasonId, EloBefore = homeHistElo, EloAfter = homeNewHistElo, EloChange = homeHistChange, RankBefore = homeHistRankBefore, RankAfter = homeHistRankAfter, IsSeasonElo = false, CreatedAt = now },
            new() { PlayerId = match.AwayPlayerId, MatchId = matchId, SeasonId = superCup.SeasonId, EloBefore = awaysp.SeasonElo, EloAfter = awayNewSeasonElo, EloChange = awaySeasonChange, RankBefore = awaySeasonRankBefore, RankAfter = awaySeasonRankAfter, IsSeasonElo = true, CreatedAt = now },
            new() { PlayerId = match.AwayPlayerId, MatchId = matchId, SeasonId = superCup.SeasonId, EloBefore = awayHistElo, EloAfter = awayNewHistElo, EloChange = awayHistChange, RankBefore = awayHistRankBefore, RankAfter = awayHistRankAfter, IsSeasonElo = false, CreatedAt = now },
        };

        bool finished = false;
        string? championId = null;

        if (!superCup.IsHomeAndAway)
        {
            if (homeScore != awayScore)
                championId = homeScore > awayScore ? match.HomePlayerId : match.AwayPlayerId;
            else if (wonOnPenaltiesId != null)
                championId = wonOnPenaltiesId;
            else
                throw new InvalidOperationException("Match is drawn — provide the winner on penalties.");

            finished = true;
        }
        else
        {
            var orderedMatches = superCup.Matches.OrderBy(m => m.Id).ToList();
            var leg1 = orderedMatches[0];
            var leg2 = orderedMatches[1];
            var otherLeg = match.Id == leg1.Id ? leg2 : leg1;

            if (otherLeg.Status == MatchStatus.Played)
            {
                int l1Home = leg1.Id == matchId ? homeScore : leg1.HomeScore ?? 0;
                int l1Away = leg1.Id == matchId ? awayScore : leg1.AwayScore ?? 0;
                int l2Home = leg2.Id == matchId ? homeScore : leg2.HomeScore ?? 0;
                int l2Away = leg2.Id == matchId ? awayScore : leg2.AwayScore ?? 0;

                // Player1 is leg1.Home; Player2 is leg1.Away
                int p1Goals = l1Home + l2Away;
                int p2Goals = l1Away + l2Home;

                if (p1Goals > p2Goals)
                    championId = leg1.HomePlayerId!;
                else if (p2Goals > p1Goals)
                    championId = leg1.AwayPlayerId!;
                else if (wonOnPenaltiesId != null)
                    championId = wonOnPenaltiesId;
                else
                    throw new InvalidOperationException("Tie is level on aggregate — provide the winner on penalties.");

                finished = true;
            }
        }

        await superCupRepository.SaveMatchResultAsync(new SuperCupMatchResultData
        {
            MatchId = matchId,
            SuperCupId = superCupId,
            SeasonId = superCup.SeasonId,
            HomeScore = homeScore,
            AwayScore = awayScore,
            WonOnPenaltiesId = wonOnPenaltiesId,
            HomePenaltyScore = wonOnPenaltiesId != null ? homePenaltyScore : null,
            AwayPenaltyScore = wonOnPenaltiesId != null ? awayPenaltyScore : null,
            HomePlayerId = match.HomePlayerId,
            HomeNewSeasonElo = homeNewSeasonElo,
            HomeNewHistoricalElo = homeNewHistElo,
            AwayPlayerId = match.AwayPlayerId,
            AwayNewSeasonElo = awayNewSeasonElo,
            AwayNewHistoricalElo = awayNewHistElo,
            EloHistories = histories,
            Finished = finished,
            ChampionId = championId
        });

        return new RecordSuperCupResultResponse
        {
            Home = new SuperCupEloChangeResult
            {
                PlayerId = match.HomePlayerId,
                DisplayName = homesp.Player.DisplayName,
                EloBefore = homesp.SeasonElo,
                EloAfter = homeNewSeasonElo,
                EloChange = homeSeasonChange,
                RankBefore = homeSeasonRankBefore,
                RankAfter = homeSeasonRankAfter
            },
            Away = new SuperCupEloChangeResult
            {
                PlayerId = match.AwayPlayerId,
                DisplayName = awaysp.Player.DisplayName,
                EloBefore = awaysp.SeasonElo,
                EloAfter = awayNewSeasonElo,
                EloChange = awaySeasonChange,
                RankBefore = awaySeasonRankBefore,
                RankAfter = awaySeasonRankAfter
            },
            Finished = finished,
            ChampionId = championId
        };
    }

    private static SuperCupResponse MapToResponse(Models.SuperCup superCup)
    {
        var seasonPlayerMap = superCup.Season.Players
            .ToDictionary(sp => sp.PlayerId, sp => sp.Player.DisplayName);

        return new SuperCupResponse
        {
            Id = superCup.Id,
            SeasonId = superCup.SeasonId,
            Name = superCup.Name,
            Status = superCup.Status.ToString(),
            IsHomeAndAway = superCup.IsHomeAndAway,
            Player1Id = superCup.Player1Id,
            Player1Name = superCup.Player1Id != null
                ? seasonPlayerMap.GetValueOrDefault(superCup.Player1Id)
                : null,
            Player2Id = superCup.Player2Id,
            Player2Name = superCup.Player2Id != null
                ? seasonPlayerMap.GetValueOrDefault(superCup.Player2Id)
                : null,
            ChampionId = superCup.ChampionId,
            ChampionName = superCup.ChampionId != null
                ? seasonPlayerMap.GetValueOrDefault(superCup.ChampionId)
                : null,
            Matches = superCup.Matches
                .OrderBy(m => m.Id)
                .Select(m => new SuperCupMatchResponse
                {
                    Id = m.Id,
                    HomePlayerId = m.HomePlayerId ?? string.Empty,
                    HomePlayerName = !string.IsNullOrEmpty(m.HomePlayerId)
                        ? seasonPlayerMap.GetValueOrDefault(m.HomePlayerId, m.HomePlayerId)
                        : "TBD",
                    AwayPlayerId = m.AwayPlayerId ?? string.Empty,
                    AwayPlayerName = !string.IsNullOrEmpty(m.AwayPlayerId)
                        ? seasonPlayerMap.GetValueOrDefault(m.AwayPlayerId, m.AwayPlayerId)
                        : "TBD",
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    WonOnPenaltiesId = m.WonOnPenaltiesId,
                    HomePenaltyScore = m.HomePenaltyScore,
                    AwayPenaltyScore = m.AwayPenaltyScore,
                    Status = m.Status.ToString(),
                    Leg = m.Leg,
                    PlayedAt = m.PlayedAt
                }).ToList()
        };
    }

    private static (int change, int newElo) ComputeElo(int myElo, int opponentElo, double result, int goalDiff, int k)
    {
        double expected = 1.0 / (1.0 + Math.Pow(10, (opponentElo - myElo) / 400.0));
        double multiplier = goalDiff >= 3 ? 1.5 : goalDiff == 2 ? 1.2 : 1.0;
        int change = (int)Math.Round(k * multiplier * (result - expected), MidpointRounding.AwayFromZero);
        return (change, myElo + change);
    }
}
