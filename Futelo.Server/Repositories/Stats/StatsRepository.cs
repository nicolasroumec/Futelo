using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Stats;

public class StatsRepository(FuteloContext context) : IStatsRepository
{
    public async Task<AppUser?> GetPlayerAsync(string playerId)
        => await context.Users
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(u => u.Id == playerId);

    public async Task<List<Match>> GetPlayerMatchesInVaultAsync(string playerId, int vaultId)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m => m.HomePlayerId == playerId || m.AwayPlayerId == playerId)
            .Where(m =>
                (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId))
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.VideoGame)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<Match>> GetH2HMatchesInVaultAsync(string player1Id, string player2Id, int vaultId)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m =>
                (m.HomePlayerId == player1Id && m.AwayPlayerId == player2Id) ||
                (m.HomePlayerId == player2Id && m.AwayPlayerId == player1Id))
            .Where(m =>
                (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId))
            .Include(m => m.HomePlayer)
            .Include(m => m.AwayPlayer)
            .Include(m => m.VideoGame)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<VaultPlayer>> GetGeneralRankingAsync(int vaultId)
        => await context.VaultPlayers
            .Where(vp => vp.VaultId == vaultId)
            .Include(vp => vp.Player)
            .OrderByDescending(vp => vp.EloRating)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<Dictionary<string, int>> GetVaultEloMapAsync(int vaultId)
        => await context.VaultPlayers
            .Where(vp => vp.VaultId == vaultId)
            .AsNoTrackingWithIdentityResolution()
            .ToDictionaryAsync(vp => vp.PlayerId, vp => vp.EloRating);

    public async Task<bool> IsVaultMemberAsync(string playerId, int vaultId)
        => await context.VaultPlayers
            .AnyAsync(vp => vp.VaultId == vaultId && vp.PlayerId == playerId);

    public async Task<List<SeasonPlayer>> GetSeasonRankingAsync(int seasonId, int vaultId)
        => await context.SeasonPlayers
            .Where(sp => sp.SeasonId == seasonId && sp.Season.VaultId == vaultId)
            .Include(sp => sp.Player)
            .OrderByDescending(sp => sp.SeasonElo)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<Models.Season>> GetVaultPalmaresAsync(int vaultId)
        => await context.Seasons
            .Where(s => s.VaultId == vaultId && s.Status != SeasonStatus.Draft)
            .Include(s => s.League).ThenInclude(l => l!.Champion)
            .Include(s => s.Cup).ThenInclude(c => c!.Champion)
            .Include(s => s.SuperCup).ThenInclude(sc => sc!.Champion)
            .OrderBy(s => s.Year)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<EloHistory>> GetPlayerEloHistoryInVaultAsync(string playerId, int vaultId)
        => await context.EloHistories
            .Where(e => e.PlayerId == playerId && !e.IsSeasonElo && e.Season.VaultId == vaultId)
            .OrderBy(e => e.CreatedAt)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<EloHistory>> GetPlayerGlobalEloHistoryAsync(string playerId, int vaultId, string? competitionType)
    {
        var query = context.EloHistories
            .Include(e => e.Match)
            .Include(e => e.Season)
            .Where(e => e.PlayerId == playerId && !e.IsSeasonElo && e.Season.VaultId == vaultId);

        query = competitionType switch
        {
            "League"   => query.Where(e => e.Match.LeagueId != null),
            "Cup"      => query.Where(e => e.Match.CupRoundId != null),
            "SuperCup" => query.Where(e => e.Match.SuperCupId != null),
            _          => query
        };

        return await query
            .OrderBy(e => e.CreatedAt)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();
    }

    public async Task<List<Match>> GetAllPlayedMatchesInVaultAsync(int vaultId)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m =>
                (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId))
            .Include(m => m.HomePlayer)
            .Include(m => m.AwayPlayer)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<Match>> GetAllPlayedMatchesWithTeamsInVaultAsync(int vaultId)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m =>
                (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId))
            .Include(m => m.HomePlayer)
            .Include(m => m.AwayPlayer)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<Match>> GetAllPlayedMatchesWithVideoGameInVaultAsync(int vaultId)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played && m.VideoGameId != null)
            .Where(m =>
                (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId))
            .Include(m => m.HomePlayer)
            .Include(m => m.AwayPlayer)
            .Include(m => m.VideoGame)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<Match>> GetPlayerLastNMatchesAsync(string playerId, int vaultId, int n)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m => m.HomePlayerId == playerId || m.AwayPlayerId == playerId)
            .Where(m =>
                (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId))
            .Include(m => m.HomePlayer)
            .Include(m => m.AwayPlayer)
            .OrderByDescending(m => m.PlayedAt)
            .Take(n)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<Models.Season>> GetPlayerTitleSeasonsInVaultAsync(string playerId, int vaultId)
        => await context.Seasons
            .Where(s => s.VaultId == vaultId)
            .Where(s =>
                (s.League != null && s.League.ChampionId == playerId) ||
                (s.Cup != null && s.Cup.ChampionId == playerId) ||
                (s.SuperCup != null && s.SuperCup.ChampionId == playerId))
            .Include(s => s.League)
            .Include(s => s.Cup)
            .Include(s => s.SuperCup)
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Id)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<Match>> GetPlayerRecentMatchesAsync(string playerId, int vaultId, int limit)
        => await PlayerMatchesRichQuery(playerId, vaultId)
            .Take(limit)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<Match>> GetPlayerMatchesPageAsync(string playerId, int vaultId, int skip, int take, string? competitionType = null)
        => await ApplyCompetitionFilter(PlayerMatchesRichQuery(playerId, vaultId), competitionType)
            .Skip(skip)
            .Take(take)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<int> CountPlayerMatchesAsync(string playerId, int vaultId, string? competitionType = null)
        => await ApplyCompetitionFilter(
                context.Matches
                    .Where(m => m.Status == MatchStatus.Played)
                    .Where(m => m.HomePlayerId == playerId || m.AwayPlayerId == playerId)
                    .Where(m =>
                        (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                        (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                        (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId)),
                competitionType)
            .CountAsync();

    private IQueryable<Match> PlayerMatchesRichQuery(string playerId, int vaultId)
        => context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m => m.HomePlayerId == playerId || m.AwayPlayerId == playerId)
            .Where(m =>
                (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId))
            .Include(m => m.HomePlayer)
            .Include(m => m.AwayPlayer)
            .Include(m => m.WonOnPenalties)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.VideoGame)
            .Include(m => m.League).ThenInclude(l => l!.Season)
            .Include(m => m.CupRound).ThenInclude(cr => cr!.Cup).ThenInclude(c => c.Season)
            .Include(m => m.SuperCup).ThenInclude(sc => sc!.Season)
            .OrderByDescending(m => m.PlayedAt);

    private static IQueryable<Match> ApplyCompetitionFilter(IQueryable<Match> query, string? competitionType)
        => competitionType switch
        {
            "League" => query.Where(m => m.LeagueId != null),
            "Cup" => query.Where(m => m.CupRoundId != null),
            "SuperCup" => query.Where(m => m.SuperCupId != null),
            _ => query
        };

    public async Task<List<EloHistory>> GetAllHistoricalEloInVaultAsync(int vaultId)
        => await context.EloHistories
            .Where(e => !e.IsSeasonElo && e.Season.VaultId == vaultId)
            .Include(e => e.Player)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<Match>> GetPlayerMatchesWithOpponentsInVaultAsync(string playerId, int vaultId)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m => m.HomePlayerId == playerId || m.AwayPlayerId == playerId)
            .Where(m =>
                (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId))
            .Include(m => m.HomePlayer)
            .Include(m => m.AwayPlayer)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<Match?> GetTopScoringMatchInVaultAsync(int vaultId)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m =>
                (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId))
            .Include(m => m.HomePlayer)
            .Include(m => m.AwayPlayer)
            .Include(m => m.League).ThenInclude(l => l!.Season)
            .Include(m => m.CupRound).ThenInclude(cr => cr!.Cup).ThenInclude(c => c.Season)
            .Include(m => m.SuperCup).ThenInclude(sc => sc!.Season)
            .OrderByDescending(m => (m.HomeScore ?? 0) + (m.AwayScore ?? 0))
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync();
}
