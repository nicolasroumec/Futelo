using Futelo.Shared.DTOs.Vault;

namespace Futelo.Server.Services.Vault;

public interface IVaultService
{
    Task<List<VaultResponse>> GetUserVaultsAsync(string userId);
    Task<VaultResponse> GetByIdAsync(int id, string userId);
    Task<List<RecentMatchResponse>> GetRecentMatchesAsync(int id, string userId, int limit);
    Task<MatchHistoryPageResponse> GetMatchHistoryAsync(int id, string userId, int page, int pageSize, string? competitionType = null);
    Task<VaultResponse> CreateAsync(string ownerId, CreateVaultRequest request);
    Task UpdateAsync(int id, string userId, UpdateVaultRequest request);
    Task DeleteAsync(int id, string userId);
}
