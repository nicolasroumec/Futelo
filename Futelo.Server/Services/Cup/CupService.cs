using Futelo.Server.Helpers;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Cup;
using Futelo.Server.Repositories.Shared;
using Futelo.Server.Services.Achievement;
using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.Cup;
using Futelo.Shared.Enums;

namespace Futelo.Server.Services.Cup;

using static ErrorMessages;

public class CupService(ICupRepository cupRepository, IAchievementEngine achievementEngine, IEloRollbackRepository eloRollback) : ICupService
{
    public async Task<CupResponse> GetByIdAsync(int cupId, string userId)
    {
        var cup = await cupRepository.GetByIdAsync(cupId);
        if (cup == null || cup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(CupNotFound);

        var caller = cup.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        bool canEdit = caller?.Role == VaultRole.Admin || caller?.Role == VaultRole.Editor;
        return MapToResponse(cup, canEdit, LastPlayedMatchId(cup));
    }

    public async Task GenerateBracketAsync(int cupId, string userId)
    {
        var cup = await cupRepository.GetByIdAsync(cupId);
        if (cup == null || cup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(CupNotFound);

        if (cup.Status != TournamentStatus.NotStarted)
            throw new InvalidOperationException(BracketAlreadyGenerated);

        var caller = cup.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || (caller.Role != VaultRole.Admin && caller.Role != VaultRole.Editor))
            throw new UnauthorizedAccessException(OnlyAdminsAndEditorsCanEdit);

        var seasonPlayers = cup.Season.Players.ToList();
        int n = seasonPlayers.Count;
        if (n != 4 && n != 5 && n != 6 && n != 8)
            throw new InvalidOperationException($"Cup requires exactly 4, 5, 6, or 8 players. Found: {n}.");

        List<string> seeds;
        if (cup.SeedingMode == CupSeedingMode.Random)
        {
            seeds = seasonPlayers.OrderBy(_ => Random.Shared.Next()).Select(sp => sp.PlayerId).ToList();
        }
        else if (cup.SeedingMode == CupSeedingMode.LeaguePosition)
        {
            var league = cup.Season.League;
            if (league == null || league.Status == TournamentStatus.NotStarted)
                throw new InvalidOperationException("League must be active or finished to use league position seeding.");
            var played = league.Matches.Where(m => m.Status == MatchStatus.Played).ToList();
            var standings = Futelo.Server.Helpers.StandingsCalculator.Compute(played, league.Players, league.TiebreakerCriteria);
            seeds = standings.Select(row => row.PlayerId).ToList();
        }
        else
        {
            // SeasonElo (default)
            seeds = seasonPlayers.OrderByDescending(sp => sp.Player.EloRating).Select(sp => sp.PlayerId).ToList();
        }

        var (rounds, players) = BuildBracket(seeds, cupId, cup.IsHomeAndAway);

        await cupRepository.SetBracketAsync(cupId, players, rounds);
        await cupRepository.UpdateStatusAsync(cupId, TournamentStatus.Active);
    }

    public async Task StartManualAsync(int cupId, string userId)
    {
        var cup = await cupRepository.GetByIdAsync(cupId);
        if (cup == null || cup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(CupNotFound);

        if (cup.Status != TournamentStatus.NotStarted)
            throw new InvalidOperationException(BracketAlreadyGenerated);

        var caller = cup.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || (caller.Role != VaultRole.Admin && caller.Role != VaultRole.Editor))
            throw new UnauthorizedAccessException(OnlyAdminsAndEditorsCanEdit);

        var seasonPlayers = cup.Season.Players.ToList();
        if (seasonPlayers.Count < 2)
            throw new InvalidOperationException(AtLeast2PlayersRequired);

        var players = seasonPlayers.Select(sp => new CupPlayer { CupId = cupId, PlayerId = sp.PlayerId }).ToList();
        await cupRepository.InitPlayersAsync(cupId, players);
        await cupRepository.ActivateManualAsync(cupId);
    }

    public async Task<int> AddRoundAsync(int cupId, AddCupRoundRequest request, string userId)
    {
        var cup = await cupRepository.GetByIdAsync(cupId);
        if (cup == null || cup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(CupNotFound);

        if (cup.Status != TournamentStatus.Active || !cup.IsManual)
            throw new InvalidOperationException(CupNotActive);

        var caller = cup.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || (caller.Role != VaultRole.Admin && caller.Role != VaultRole.Editor))
            throw new UnauthorizedAccessException(OnlyAdminsAndEditorsCanEdit);

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidOperationException("Round name is required.");

        if (request.RoundNumber < 1)
            throw new InvalidOperationException("Round number must be at least 1.");

        var round = new CupRound
        {
            CupId = cupId,
            Name = request.Name,
            RoundNumber = request.RoundNumber
        };

        return await cupRepository.AddRoundAsync(round);
    }

    public async Task AddMatchAsync(int cupId, int roundId, AddCupMatchRequest request, string userId)
    {
        var cup = await cupRepository.GetByIdAsync(cupId);
        if (cup == null || cup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(CupNotFound);

        if (cup.Status != TournamentStatus.Active || !cup.IsManual)
            throw new InvalidOperationException(CupNotActive);

        var caller = cup.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || (caller.Role != VaultRole.Admin && caller.Role != VaultRole.Editor))
            throw new UnauthorizedAccessException(OnlyAdminsAndEditorsCanEdit);

        var round = cup.Rounds.FirstOrDefault(r => r.Id == roundId);
        if (round == null)
            throw new KeyNotFoundException(CupRoundNotFound);

        var seasonPlayerIds = cup.Season.Players.Select(sp => sp.PlayerId).ToHashSet();
        if (!seasonPlayerIds.Contains(request.HomePlayerId))
            throw new InvalidOperationException(PlayerNotFoundInSeason);
        if (!seasonPlayerIds.Contains(request.AwayPlayerId))
            throw new InvalidOperationException(PlayerNotFoundInSeason);
        if (request.HomePlayerId == request.AwayPlayerId)
            throw new InvalidOperationException("Home and away players must be different.");

        var match = new Match
        {
            CupRoundId = roundId,
            HomePlayerId = request.HomePlayerId,
            AwayPlayerId = request.AwayPlayerId,
            Leg = request.Leg < 1 ? 1 : request.Leg,
            Status = MatchStatus.Pending
        };

        await cupRepository.AddMatchToRoundAsync(match);
    }

    public async Task<RecordCupResultResponse> RecordResultAsync(
        int cupId, int matchId, int homeScore, int awayScore, string? wonOnPenaltiesId,
        int? homePenaltyScore, int? awayPenaltyScore, string userId)
    {
        var cup = await cupRepository.GetByIdAsync(cupId);
        if (cup == null || cup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(CupNotFound);

        if (cup.Status != TournamentStatus.Active)
            throw new InvalidOperationException(CupNotActive);

        var caller = cup.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || (caller.Role != VaultRole.Admin && caller.Role != VaultRole.Editor))
            throw new UnauthorizedAccessException(OnlyAdminsAndEditorsCanEdit);

        if (homeScore < 0 || awayScore < 0)
            throw new InvalidOperationException(ScoresCannotBeNegative);

        var allRounds = cup.Rounds.OrderBy(r => r.RoundNumber).ToList();
        CupRound? matchRound = null;
        Match? match = null;
        int matchIndexInRound = -1;

        foreach (var round in allRounds)
        {
            var orderedMatches = round.Matches.OrderBy(m => m.Id).ToList();
            int idx = orderedMatches.FindIndex(m => m.Id == matchId);
            if (idx >= 0)
            {
                matchRound = round;
                match = orderedMatches[idx];
                matchIndexInRound = idx;
                break;
            }
        }

        if (matchRound == null || match == null)
            throw new KeyNotFoundException("Match not found in this cup.");
        if (string.IsNullOrEmpty(match.HomePlayerId) || string.IsNullOrEmpty(match.AwayPlayerId))
            throw new InvalidOperationException(MatchParticipantsNotDetermined);

        if (match.Status == MatchStatus.Played)
        {
            if (!await eloRollback.IsLastMatchForBothPlayersAsync(matchId, match.HomePlayerId, match.AwayPlayerId))
                throw new InvalidOperationException(CanOnlyCorrectLastMatch);

            var oldWinnerId = cup.Rounds
                .Where(r => r.RoundNumber > matchRound.RoundNumber)
                .SelectMany(r => r.Matches)
                .Where(m => m.Status == MatchStatus.Pending)
                .SelectMany(m => new[] { m.HomePlayerId, m.AwayPlayerId })
                .FirstOrDefault(pid => pid == match.HomePlayerId || pid == match.AwayPlayerId);

            if (cup.Status == TournamentStatus.Finished)
                await cupRepository.ResetCupFinishAsync(cupId);
            if (oldWinnerId != null)
                await cupRepository.RevertBracketAdvancementAsync(cupId, oldWinnerId);
            await eloRollback.RollbackMatchEloAsync(matchId, match.HomePlayerId, match.AwayPlayerId, cup.SeasonId);

            cup = (await cupRepository.GetByIdAsync(cupId))!;
            allRounds = cup.Rounds.OrderBy(r => r.RoundNumber).ToList();
            matchRound = null; match = null; matchIndexInRound = -1;
            foreach (var round in allRounds)
            {
                var orderedMatches = round.Matches.OrderBy(m => m.Id).ToList();
                int idx = orderedMatches.FindIndex(m => m.Id == matchId);
                if (idx >= 0) { matchRound = round; match = orderedMatches[idx]; matchIndexInRound = idx; break; }
            }
            if (matchRound == null || match == null)
                throw new InvalidOperationException("Failed to reload match after rollback.");
        }

        // ELO
        var seasonPlayers = cup.Season.Players.ToList();
        var homesp = seasonPlayers.First(sp => sp.PlayerId == match.HomePlayerId);
        var awaysp = seasonPlayers.First(sp => sp.PlayerId == match.AwayPlayerId);

        int totalRounds = allRounds.Count;
        int roundFromEnd = totalRounds - matchRound.RoundNumber;
        double kMultiplier = roundFromEnd switch { 0 => 1.5, 1 => 1.2, _ => 1.0 };
        int k = (int)(EloCalculator.CupBaseK * kMultiplier);

        double homeResult = homeScore > awayScore ? 1.0 : homeScore == awayScore ? 0.5 : 0.0;
        double awayResult = 1.0 - homeResult;
        int goalDiff = Math.Abs(homeScore - awayScore);

        if (homeScore == awayScore && wonOnPenaltiesId != null)
        {
            homeResult = wonOnPenaltiesId == match.HomePlayerId ? 1.0 : 0.0;
            awayResult = 1.0 - homeResult;
        }

        var (homeSeasonChange, homeNewSeasonElo) = EloCalculator.Compute(homesp.SeasonElo, awaysp.SeasonElo, homeResult, goalDiff, k);
        var (awaySeasonChange, awayNewSeasonElo) = EloCalculator.Compute(awaysp.SeasonElo, homesp.SeasonElo, awayResult, goalDiff, k);

        int homeHistElo = homesp.Player.EloRating;
        int awayHistElo = awaysp.Player.EloRating;
        var (homeHistChange, homeNewHistElo) = EloCalculator.Compute(homeHistElo, awayHistElo, homeResult, goalDiff, k);
        var (awayHistChange, awayNewHistElo) = EloCalculator.Compute(awayHistElo, homeHistElo, awayResult, goalDiff, k);

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
            new() { PlayerId = match.HomePlayerId, MatchId = matchId, SeasonId = cup.SeasonId, EloBefore = homesp.SeasonElo, EloAfter = homeNewSeasonElo, EloChange = homeSeasonChange, RankBefore = homeSeasonRankBefore, RankAfter = homeSeasonRankAfter, IsSeasonElo = true, CreatedAt = now },
            new() { PlayerId = match.HomePlayerId, MatchId = matchId, SeasonId = cup.SeasonId, EloBefore = homeHistElo, EloAfter = homeNewHistElo, EloChange = homeHistChange, RankBefore = homeHistRankBefore, RankAfter = homeHistRankAfter, IsSeasonElo = false, CreatedAt = now },
            new() { PlayerId = match.AwayPlayerId, MatchId = matchId, SeasonId = cup.SeasonId, EloBefore = awaysp.SeasonElo, EloAfter = awayNewSeasonElo, EloChange = awaySeasonChange, RankBefore = awaySeasonRankBefore, RankAfter = awaySeasonRankAfter, IsSeasonElo = true, CreatedAt = now },
            new() { PlayerId = match.AwayPlayerId, MatchId = matchId, SeasonId = cup.SeasonId, EloBefore = awayHistElo, EloAfter = awayNewHistElo, EloChange = awayHistChange, RankBefore = awayHistRankBefore, RankAfter = awayHistRankAfter, IsSeasonElo = false, CreatedAt = now },
        };

        string? tieWinnerId = null;
        bool tieDecided = false;
        int tieIndex = matchIndexInRound;

        if (cup.IsManual)
        {
            if (homeScore != awayScore)
                tieWinnerId = homeScore > awayScore ? match.HomePlayerId : match.AwayPlayerId;
            else if (wonOnPenaltiesId != null)
                tieWinnerId = wonOnPenaltiesId;
            else
                throw new InvalidOperationException(MatchIsDrawnNeedsPenalties);

            tieDecided = true;
        }
        else if (!cup.IsHomeAndAway)
        {
            if (homeScore != awayScore)
                tieWinnerId = homeScore > awayScore ? match.HomePlayerId : match.AwayPlayerId;
            else if (wonOnPenaltiesId != null)
                tieWinnerId = wonOnPenaltiesId;
            else
                throw new InvalidOperationException(MatchIsDrawnNeedsPenalties);

            tieDecided = true;
        }
        else
        {
            tieIndex = matchIndexInRound / 2;
            var roundMatches = matchRound.Matches.OrderBy(m => m.Id).ToList();
            var leg1 = roundMatches[tieIndex * 2];
            var leg2 = roundMatches[tieIndex * 2 + 1];
            var otherLeg = match.Id == leg1.Id ? leg2 : leg1;

            if (otherLeg.Status == MatchStatus.Played)
            {
                // Determine aggregate: leg1 Home player = A, Away = B
                int l1HomeScore = leg1.Id == matchId ? homeScore : leg1.HomeScore ?? 0;
                int l1AwayScore = leg1.Id == matchId ? awayScore : leg1.AwayScore ?? 0;
                int l2HomeScore = leg2.Id == matchId ? homeScore : leg2.HomeScore ?? 0;
                int l2AwayScore = leg2.Id == matchId ? awayScore : leg2.AwayScore ?? 0;

                string playerA = leg1.HomePlayerId!;
                string playerB = leg1.AwayPlayerId!;
                int aGoals = l1HomeScore + l2AwayScore;
                int bGoals = l1AwayScore + l2HomeScore;

                if (aGoals > bGoals)
                    tieWinnerId = playerA;
                else if (bGoals > aGoals)
                    tieWinnerId = playerB;
                else if (cup.AwayGoalRule && roundFromEnd > 0)
                {
                    int aAwayGoals = l2AwayScore;
                    int bAwayGoals = l1AwayScore;
                    if (aAwayGoals > bAwayGoals)
                        tieWinnerId = playerA;
                    else if (bAwayGoals > aAwayGoals)
                        tieWinnerId = playerB;
                    else if (wonOnPenaltiesId != null)
                        tieWinnerId = wonOnPenaltiesId;
                    else
                        throw new InvalidOperationException(TieOnAggregateMNeedsPenalties);
                }
                else if (wonOnPenaltiesId != null)
                    tieWinnerId = wonOnPenaltiesId;
                else
                    throw new InvalidOperationException(TieOnAggregateMNeedsPenalties);

                tieDecided = true;
            }
        }

        int? advanceToMatchId = null;
        bool advanceAsHome = false;
        int? advanceToLeg2MatchId = null;
        bool cupFinished = false;
        string? championId = null;
        Dictionary<string, int> finalPositions = [];

        if (tieDecided)
        {
            if (cup.IsManual)
            {
                var maxRoundNumber = allRounds.Max(r => r.RoundNumber);
                if (matchRound.RoundNumber == maxRoundNumber)
                {
                    var pendingInFinal = matchRound.Matches.Count(m => m.Id != matchId && m.Status == MatchStatus.Pending);
                    if (pendingInFinal == 0)
                    {
                        cupFinished = true;
                        championId = tieWinnerId;
                        finalPositions = ComputeCupPositions(cup, championId!, matchRound);
                    }
                }
            }
            else
            {
                var nextRound = allRounds.FirstOrDefault(r => r.RoundNumber == matchRound.RoundNumber + 1);

                if (nextRound == null)
                {
                    cupFinished = true;
                    championId = tieWinnerId;
                    finalPositions = ComputeCupPositions(cup, championId!, matchRound);
                }
                else
                {
                    var nextRoundMatches = nextRound.Matches.OrderBy(m => m.Id).ToList();
                    int prevTieCount = cup.IsHomeAndAway
                        ? matchRound.Matches.Count / 2
                        : matchRound.Matches.Count;
                    int nextMatchCount = cup.IsHomeAndAway
                        ? nextRound.Matches.Count / 2
                        : nextRound.Matches.Count;

                    int targetMatchIdx;
                    bool targetIsHome;

                    if (prevTieCount > nextMatchCount)
                    {
                        // Standard halving (e.g. QF→SF, SF→F)
                        targetMatchIdx = tieIndex / 2;
                        targetIsHome = tieIndex % 2 == 0;
                    }
                    else
                    {
                        // 1:1 mapping (Qualifying→SF for n=5,6): seeded player is already Home, winner goes Away
                        targetMatchIdx = tieIndex;
                        targetIsHome = false;
                    }

                    int actualMatchIdx = cup.IsHomeAndAway ? targetMatchIdx * 2 : targetMatchIdx;
                    advanceToMatchId = nextRoundMatches[actualMatchIdx].Id;
                    advanceAsHome = targetIsHome;

                    if (cup.IsHomeAndAway)
                        advanceToLeg2MatchId = nextRoundMatches[actualMatchIdx + 1].Id;
                }
            }
        }

        await cupRepository.SaveMatchResultAsync(new CupMatchResultData
        {
            MatchId = matchId,
            CupId = cupId,
            SeasonId = cup.SeasonId,
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
            TieWinnerId = tieWinnerId,
            AdvanceToMatchId = advanceToMatchId,
            AdvanceAsHome = advanceAsHome,
            AdvanceToLeg2MatchId = advanceToLeg2MatchId,
            CupFinished = cupFinished,
            ChampionId = championId,
            FinalCupPositions = finalPositions,
            VideoGameId = cup.Season.VideoGameId,
            HomeTeamId = homesp.TeamId,
            AwayTeamId = awaysp.TeamId
        });

        await achievementEngine.EvaluateAfterMatchAsync(new MatchAchievementContext(
            MatchId: matchId,
            VaultId: cup.Season.VaultId,
            SeasonId: cup.SeasonId,
            HomePlayerId: match.HomePlayerId,
            AwayPlayerId: match.AwayPlayerId,
            HomeScore: homeScore,
            AwayScore: awayScore,
            WonOnPenaltiesId: wonOnPenaltiesId,
            HomeOldHistElo: homeHistElo,
            HomeNewHistElo: homeNewHistElo,
            AwayOldHistElo: awayHistElo,
            AwayNewHistElo: awayNewHistElo,
            IsFinal: roundFromEnd == 0
        ));

        return new RecordCupResultResponse
        {
            Home = new CupEloChangeResult
            {
                PlayerId = match.HomePlayerId,
                DisplayName = homesp.Player.DisplayName,
                EloBefore = homesp.SeasonElo,
                EloAfter = homeNewSeasonElo,
                EloChange = homeSeasonChange,
                RankBefore = homeSeasonRankBefore,
                RankAfter = homeSeasonRankAfter
            },
            Away = new CupEloChangeResult
            {
                PlayerId = match.AwayPlayerId,
                DisplayName = awaysp.Player.DisplayName,
                EloBefore = awaysp.SeasonElo,
                EloAfter = awayNewSeasonElo,
                EloChange = awaySeasonChange,
                RankBefore = awaySeasonRankBefore,
                RankAfter = awaySeasonRankAfter
            },
            TieDecided = tieDecided,
            TieWinnerId = tieWinnerId
        };
    }

    private static (List<CupRound> rounds, List<CupPlayer> players) BuildBracket(
        List<string> seeds, int cupId, bool isHomeAndAway)
    {
        var players = seeds.Select(pid => new CupPlayer { CupId = cupId, PlayerId = pid }).ToList();

        // Each entry: (roundName, list of (home seed, away seed) — null = TBD)
        var roundDefinitions = seeds.Count switch
        {
            4 => new List<(string name, List<(string? home, string? away)>)>
            {
                ("Semifinals", [(seeds[0], seeds[3]), (seeds[1], seeds[2])]),
                ("Final",      [(null, null)])
            },
            5 => new List<(string name, List<(string? home, string? away)>)>
            {
                ("Qualifying", [(seeds[3], seeds[4])]),
                ("Semifinals", [(seeds[0], null), (seeds[1], seeds[2])]),
                ("Final",      [(null, null)])
            },
            6 => new List<(string name, List<(string? home, string? away)>)>
            {
                ("Qualifying", [(seeds[2], seeds[5]), (seeds[3], seeds[4])]),
                ("Semifinals", [(seeds[0], null), (seeds[1], null)]),
                ("Final",      [(null, null)])
            },
            8 => new List<(string name, List<(string? home, string? away)>)>
            {
                ("Quarterfinals", [(seeds[0], seeds[7]), (seeds[3], seeds[4]), (seeds[1], seeds[6]), (seeds[2], seeds[5])]),
                ("Semifinals",    [(null, null), (null, null)]),
                ("Final",         [(null, null)])
            },
            _ => throw new InvalidOperationException($"Unsupported player count: {seeds.Count}")
        };

        var rounds = new List<CupRound>();

        for (int i = 0; i < roundDefinitions.Count; i++)
        {
            var (name, matchups) = roundDefinitions[i];
            var matches = new List<Match>();

            foreach (var (home, away) in matchups)
            {
                matches.Add(new Match
                {
                    HomePlayerId = home,
                    AwayPlayerId = away,
                    Status = MatchStatus.Pending,
                    Leg = 1
                });

                if (isHomeAndAway)
                {
                    matches.Add(new Match
                    {
                        HomePlayerId = away,
                        AwayPlayerId = home,
                        Status = MatchStatus.Pending,
                        Leg = 2
                    });
                }
            }

            rounds.Add(new CupRound
            {
                CupId = cupId,
                RoundNumber = i + 1,
                Name = name,
                Matches = matches
            });
        }

        return (rounds, players);
    }

    private static Dictionary<string, int> ComputeCupPositions(Models.Cup cup, string championId, CupRound finalRound)
    {
        var positions = new Dictionary<string, int>();
        positions[championId] = 1;

        // Runner-up: the other player in the final
        var finalMatches = finalRound.Matches.OrderBy(m => m.Id).ToList();
        var finalMatch = finalMatches[0];
        string runnerUpId = finalMatch.HomePlayerId == championId
            ? finalMatch.AwayPlayerId!
            : finalMatch.HomePlayerId!;
        positions[runnerUpId] = 2;

        // Semi-final losers (position 3)
        var allRounds = cup.Rounds.OrderByDescending(r => r.RoundNumber).ToList();
        if (allRounds.Count > 1)
        {
            var semiRound = allRounds[1];
            var semiMatches = semiRound.Matches.OrderBy(m => m.Id).ToList();
            int tieCount = cup.IsHomeAndAway ? semiMatches.Count / 2 : semiMatches.Count;

            for (int t = 0; t < tieCount; t++)
            {
                var leg1 = semiMatches[cup.IsHomeAndAway ? t * 2 : t];

                if (!string.IsNullOrEmpty(leg1.HomePlayerId) && !positions.ContainsKey(leg1.HomePlayerId))
                    positions[leg1.HomePlayerId] = 3;
                if (!string.IsNullOrEmpty(leg1.AwayPlayerId) && !positions.ContainsKey(leg1.AwayPlayerId))
                    positions[leg1.AwayPlayerId] = 3;
            }
        }

        int nextPos = positions.Values.DefaultIfEmpty(0).Max() + 1;
        foreach (var player in cup.Players)
        {
            if (!positions.ContainsKey(player.PlayerId))
                positions[player.PlayerId] = nextPos++;
        }

        return positions;
    }

    private static int? LastPlayedMatchId(Models.Cup cup) =>
        cup.Rounds.SelectMany(r => r.Matches)
            .Where(m => m.Status == MatchStatus.Played)
            .OrderByDescending(m => m.PlayedAt).ThenByDescending(m => m.Id)
            .FirstOrDefault()?.Id;

    private static CupResponse MapToResponse(Models.Cup cup, bool canEdit = false, int? lastPlayedMatchId = null)
    {
        var seasonPlayerMap = cup.Season.Players
            .ToDictionary(sp => sp.PlayerId, sp => sp.Player.DisplayName);

        return new CupResponse
        {
            Id = cup.Id,
            SeasonId = cup.SeasonId,
            SeasonName = cup.Season.Name,
            VaultId = cup.Season.VaultId,
            VaultName = cup.Season.Vault.Name,
            Status = cup.Status.ToString(),
            Name = cup.Name,
            IsHomeAndAway = cup.IsHomeAndAway,
            IsManual = cup.IsManual,
            SeedingMode = cup.SeedingMode,
            AwayGoalRule = cup.AwayGoalRule,
            StartDate = cup.StartDate,
            EndDate = cup.EndDate,
            ChampionId = cup.ChampionId,
            ChampionName = cup.ChampionId != null
                ? seasonPlayerMap.GetValueOrDefault(cup.ChampionId)
                : null,
            CanEdit = canEdit,
            SeasonPlayers = cup.Season.Players
                .OrderBy(sp => sp.Player.DisplayName)
                .Select(sp => new PlayerSummary { Id = sp.PlayerId, DisplayName = sp.Player.DisplayName })
                .ToList(),
            Rounds = cup.Rounds
                .OrderBy(r => r.RoundNumber)
                .Select(r => new CupRoundResponse
                {
                    Id = r.Id,
                    RoundNumber = r.RoundNumber,
                    Name = r.Name,
                    Matches = r.Matches
                        .OrderBy(m => m.Id)
                        .Select(m => new CupMatchResponse
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
                            ScheduledDate = m.ScheduledDate,
                            PlayedAt = m.PlayedAt,
                            HomeTeamId = m.HomeTeamId,
                            HomeTeamName = m.HomeTeam?.Name,
                            AwayTeamId = m.AwayTeamId,
                            AwayTeamName = m.AwayTeam?.Name,
                            VideoGameId = m.VideoGameId,
                            VideoGameName = m.VideoGame?.Name,
                            IsLastPlayed = m.Id == lastPlayedMatchId
                        }).ToList()
                }).ToList()
        };
    }

    public async Task PatchMatchAsync(int cupId, int matchId, int? homeTeamId, int? awayTeamId, int? videoGameId, DateTime? scheduledDate, string userId)
    {
        var cup = await cupRepository.GetByIdAsync(cupId);
        if (cup == null || cup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(CupNotFound);

        var caller = cup.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || (caller.Role != VaultRole.Admin && caller.Role != VaultRole.Editor))
            throw new UnauthorizedAccessException(OnlyAdminsAndEditorsCanEdit);

        var match = cup.Rounds.SelectMany(r => r.Matches).FirstOrDefault(m => m.Id == matchId);
        if (match == null)
            throw new KeyNotFoundException(MatchNotFound);

        await cupRepository.PatchMatchAsync(matchId, homeTeamId, awayTeamId, videoGameId, scheduledDate);
    }

    public async Task PatchDatesAsync(int cupId, string userId, DateTime? startDate, DateTime? endDate)
    {
        var cup = await cupRepository.GetByIdAsync(cupId);
        if (cup == null || cup.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(CupNotFound);
        if (cup.Season.Vault.OwnerId != userId)
            throw new UnauthorizedAccessException(OnlyAdminsAndEditorsCanEdit);

        await cupRepository.PatchDatesAsync(cupId, startDate, endDate);
    }

}
