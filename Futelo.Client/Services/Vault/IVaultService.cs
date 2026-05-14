using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Client.Services.Vault;

public interface IVaultService
{
    Task<List<VaultResponse>> GetAllAsync();
    Task<VaultResponse> GetByIdAsync(int id);
    Task<List<RecentMatchResponse>> GetRecentMatchesAsync(int vaultId, int limit = 10);
    Task<MatchHistoryPageResponse> GetMatchHistoryAsync(int vaultId, int page = 1, int pageSize = 10);
    Task<VaultResponse> CreateAsync(CreateVaultRequest request);
    Task UpdateAsync(int id, UpdateVaultRequest request);
    Task DeleteAsync(int id);
    Task<InvitationResponse> InviteAsync(int vaultId, InviteRequest request);
}
