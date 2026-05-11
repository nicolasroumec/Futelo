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
            .OrderByDescending(vp => vp.Player.EloRating)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

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
}
