using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Client.Services.Vault;

public class VaultService(HttpClient http) : ApiService(http), IVaultService
{
    public Task<List<VaultResponse>> GetAllAsync(CancellationToken ct = default)
        => GetListAsync<VaultResponse>("api/vaults", ct);

    public Task<VaultResponse> GetByIdAsync(int id, CancellationToken ct = default)
        => GetAsync<VaultResponse>($"api/vaults/{id}", ct);

    public Task<List<RecentMatchResponse>> GetRecentMatchesAsync(int vaultId, int limit = 10, CancellationToken ct = default)
        => GetListAsync<RecentMatchResponse>($"api/vaults/{vaultId}/recent-matches?limit={limit}", ct);

    public Task<MatchHistoryPageResponse> GetMatchHistoryAsync(int vaultId, int page = 1, int pageSize = 10, CancellationToken ct = default)
        => GetAsync<MatchHistoryPageResponse>($"api/vaults/{vaultId}/matches?page={page}&pageSize={pageSize}", ct);

    public Task<VaultResponse> CreateAsync(CreateVaultRequest request)
        => PostAsync<VaultResponse>("api/vaults", request);

    public Task UpdateAsync(int id, UpdateVaultRequest request)
        => PutAsync($"api/vaults/{id}", request);

    public Task DeleteAsync(int id)
        => DeleteAsync($"api/vaults/{id}");

    public Task<InvitationResponse> InviteAsync(int vaultId, InviteRequest request)
        => PostAsync<InvitationResponse>($"api/vaults/{vaultId}/invite", request);
}
