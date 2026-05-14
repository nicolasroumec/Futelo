using System.Net.Http.Json;
using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Client.Services.Vault;

public class VaultService(HttpClient http) : IVaultService
{
    public async Task<List<VaultResponse>> GetAllAsync()
        => await http.GetFromJsonAsync<List<VaultResponse>>("api/vaults") ?? [];

    public async Task<VaultResponse> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<VaultResponse>($"api/vaults/{id}")
            ?? throw new KeyNotFoundException($"Vault {id} not found.");

    public async Task<List<RecentMatchResponse>> GetRecentMatchesAsync(int vaultId, int limit = 10)
        => await http.GetFromJsonAsync<List<RecentMatchResponse>>($"api/vaults/{vaultId}/recent-matches?limit={limit}") ?? [];

    public async Task<MatchHistoryPageResponse> GetMatchHistoryAsync(int vaultId, int page = 1, int pageSize = 10)
        => await http.GetFromJsonAsync<MatchHistoryPageResponse>($"api/vaults/{vaultId}/matches?page={page}&pageSize={pageSize}")
            ?? new MatchHistoryPageResponse { Page = page, PageSize = pageSize };

    public async Task<VaultResponse> CreateAsync(CreateVaultRequest request)
    {
        var response = await http.PostAsJsonAsync("api/vaults", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VaultResponse>()
            ?? throw new InvalidOperationException("Invalid server response.");
    }

    public async Task UpdateAsync(int id, UpdateVaultRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/vaults/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/vaults/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<InvitationResponse> InviteAsync(int vaultId, InviteRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/vaults/{vaultId}/invite", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<InvitationResponse>()
            ?? throw new InvalidOperationException("Invalid server response.");
    }
}
