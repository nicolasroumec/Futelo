using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using SeasonModel = Futelo.Server.Models.Season;

namespace Futelo.Server.Repositories.Achievement;

public class AchievementEvaluationRepository(FuteloContext context) : IAchievementEvaluationRepository
{
    public async Task<List<Match>> GetPlayerLastMatchesInVaultAsync(string playerId, int vaultId, int count)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m => m.HomePlayerId == playerId || m.AwayPlayerId == playerId)
            .Where(m =>
                (m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId))
            .OrderByDescending(m => m.PlayedAt)
            .Take(count)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<EloHistory>> GetPlayerLastHistoricalEloInVaultAsync(string playerId, int vaultId, int count)
        => await context.EloHistories
            .Where(e => e.PlayerId == playerId && !e.IsSeasonElo && e.Season.VaultId == vaultId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(count)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public Task<int> CountMatchesInVaultAsync(string playerId, int vaultId)
        => context.Matches
            .CountAsync(m =>
                m.Status == MatchStatus.Played &&
                (m.HomePlayerId == playerId || m.AwayPlayerId == playerId) &&
                ((m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                 (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                 (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId)));

    public Task<int> CountPenaltyWinsInVaultAsync(string playerId, int vaultId)
        => context.Matches
            .CountAsync(m =>
                m.Status == MatchStatus.Played &&
                m.WonOnPenaltiesId == playerId &&
                ((m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                 (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                 (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId)));

    public Task<int> CountExact1_0WinsInVaultAsync(string playerId, int vaultId)
        => context.Matches
            .CountAsync(m =>
                m.Status == MatchStatus.Played &&
                ((m.HomePlayerId == playerId && m.HomeScore == 1 && m.AwayScore == 0) ||
                 (m.AwayPlayerId == playerId && m.AwayScore == 1 && m.HomeScore == 0)) &&
                ((m.LeagueId != null && m.League!.Season.VaultId == vaultId) ||
                 (m.CupRoundId != null && m.CupRound!.Cup.Season.VaultId == vaultId) ||
                 (m.SuperCupId != null && m.SuperCup!.Season.VaultId == vaultId)));

    public async Task<int?> GetMinHistoricalEloInVaultAsync(string playerId, int vaultId)
    {
        var min = await context.EloHistories
            .Where(e => e.PlayerId == playerId && !e.IsSeasonElo && e.Season.VaultId == vaultId)
            .Select(e => (int?)e.EloAfter)
            .MinAsync();
        return min;
    }

    public async Task<SeasonModel?> GetSeasonWithCompetitionsAsync(int seasonId)
        => await context.Seasons
            .Include(s => s.League).ThenInclude(l => l!.Champion)
            .Include(s => s.Cup).ThenInclude(c => c!.Champion)
            .Include(s => s.SuperCup).ThenInclude(sc => sc!.Champion)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(s => s.Id == seasonId);

    public async Task<List<Match>> GetSeasonMatchesAsync(int seasonId)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m =>
                (m.LeagueId != null && m.League!.SeasonId == seasonId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.SeasonId == seasonId) ||
                (m.SuperCupId != null && m.SuperCup!.SeasonId == seasonId))
            .OrderBy(m => m.PlayedAt)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<EloHistory>> GetSeasonEloHistoriesAsync(int seasonId, bool isSeasonElo)
        => await context.EloHistories
            .Where(e => e.SeasonId == seasonId && e.IsSeasonElo == isSeasonElo)
            .Include(e => e.Player)
            .OrderBy(e => e.CreatedAt)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<SeasonPlayer>> GetSeasonPlayersAsync(int seasonId)
        => await context.SeasonPlayers
            .Where(sp => sp.SeasonId == seasonId)
            .Include(sp => sp.Player)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<SeasonModel>> GetVaultSeasonsWithChampionsAsync(int vaultId)
        => await context.Seasons
            .Where(s => s.VaultId == vaultId && s.Status == SeasonStatus.Finished)
            .Include(s => s.League).ThenInclude(l => l!.Champion)
            .Include(s => s.Cup).ThenInclude(c => c!.Champion)
            .Include(s => s.SuperCup).ThenInclude(sc => sc!.Champion)
            .OrderBy(s => s.Year).ThenBy(s => s.Id)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();
}
