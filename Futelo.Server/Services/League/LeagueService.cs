using Futelo.Server.Helpers;
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
            : StandingsCalculator.Compute(league.Matches.Where(m => m.Status == MatchStatus.Played).ToList(), league.Players);

        var caller = league.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        bool canEdit = caller?.Role == VaultRole.Admin || caller?.Role == VaultRole.Editor;

        return new LeagueResponse
        {
            Id = league.Id,
            SeasonId = league.SeasonId,
            Status = league.Status.ToString(),
            Name = league.Name,
            IsHomeAndAway = league.IsHomeAndAway,
            ChampionId = league.ChampionId,
            ChampionName = league.ChampionId != null
                ? league.Season.Players.FirstOrDefault(sp => sp.PlayerId == league.ChampionId)?.Player.DisplayName
                : null,
            CanEdit = canEdit,
            Matches = league.Matches
                .OrderBy(m => m.Leg).ThenBy(m => m.Id)
                .Select(m => new MatchResponse
                {
                    Id = m.Id,
                    HomePlayerId = m.HomePlayerId!,
                    HomePlayerName = m.HomePlayer!.DisplayName,
                    AwayPlayerId = m.AwayPlayerId!,
                    AwayPlayerName = m.AwayPlayer!.DisplayName,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    Status = m.Status.ToString(),
                    Matchday = m.Leg,
                    PlayedAt = m.PlayedAt,
                    HomeTeamId = m.HomeTeamId,
                    HomeTeamName = m.HomeTeam?.Name,
                    AwayTeamId = m.AwayTeamId,
                    AwayTeamName = m.AwayTeam?.Name,
                    VideoGameId = m.VideoGameId,
                    VideoGameName = m.VideoGame?.Name
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

        var matches = FixtureGenerator.Build(playerIds, leagueId, league.IsHomeAndAway);

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

        var matches = FixtureGenerator.Build(shuffled, leagueId, league.IsHomeAndAway);

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

        if (homeScore < 0 || awayScore < 0)
            throw new InvalidOperationException("Scores cannot be negative.");

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

        var (homeSeasonChange, homeNewSeasonElo) = EloCalculator.Compute(homesp.SeasonElo, awaysp.SeasonElo, homeResult, goalDiff, k: 32);
        var (awaySeasonChange, awayNewSeasonElo) = EloCalculator.Compute(awaysp.SeasonElo, homesp.SeasonElo, awayResult, goalDiff, k: 32);

        int homeHistElo = homesp.Player.EloRating;
        int awayHistElo = awaysp.Player.EloRating;
        var (homeHistChange, homeNewHistElo) = EloCalculator.Compute(homeHistElo, awayHistElo, homeResult, goalDiff, k: 32);
        var (awayHistChange, awayNewHistElo) = EloCalculator.Compute(awayHistElo, homeHistElo, awayResult, goalDiff, k: 32);

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

            var finalStandings = StandingsCalculator.Compute(allPlayedWithNew, league.Players);
            finalPositions = finalStandings
                .Select((row, i) => (row.PlayerId, Position: i + 1))
                .ToDictionary(x => x.PlayerId, x => x.Position);
        }

        string? championId = leagueFinished
            ? finalPositions.FirstOrDefault(x => x.Value == 1).Key
            : null;

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
            ChampionId = championId,
            FinalLeaguePositions = finalPositions,
            VideoGameId = league.Season.VideoGameId,
            HomeTeamId = homesp.TeamId,
            AwayTeamId = awaysp.TeamId
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

    public async Task PatchMatchAsync(int leagueId, int matchId, int? homeTeamId, int? awayTeamId, int? videoGameId, string userId)
    {
        var league = await leagueRepository.GetByIdAsync(leagueId);
        if (league == null || league.Season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("League not found.");

        var caller = league.Season.Vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || (caller.Role != VaultRole.Admin && caller.Role != VaultRole.Editor))
            throw new UnauthorizedAccessException("Only vault admins and editors can edit matches.");

        if (league.Matches.All(m => m.Id != matchId))
            throw new KeyNotFoundException("Match not found.");

        await leagueRepository.PatchMatchAsync(matchId, homeTeamId, awayTeamId, videoGameId);
    }

}
