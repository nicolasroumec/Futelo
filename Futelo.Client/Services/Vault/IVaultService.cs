using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Client.Services.Vault;

public interface IVaultService
{
    Task<List<VaultResponse>> GetAllAsync(CancellationToken ct = default);
    Task<VaultResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<RecentMatchResponse>> GetRecentMatchesAsync(int vaultId, int limit = 10, CancellationToken ct = default);
    Task<MatchHistoryPageResponse> GetMatchHistoryAsync(int vaultId, int page = 1, int pageSize = 10, string? competitionType = null, CancellationToken ct = default);
    Task<VaultResponse> CreateAsync(CreateVaultRequest request);
    Task UpdateAsync(int id, UpdateVaultRequest request);
    Task DeleteAsync(int id);
    Task<InvitationResponse> InviteAsync(int vaultId, InviteRequest request);
}
