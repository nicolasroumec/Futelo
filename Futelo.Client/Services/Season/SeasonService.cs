using System.Net.Http.Json;
using Futelo.Shared.DTOs.Season;

namespace Futelo.Client.Services.Season;

public class SeasonService(HttpClient http) : ISeasonService
{
    public async Task<List<SeasonResponse>> GetByVaultAsync(int vaultId)
        => await http.GetFromJsonAsync<List<SeasonResponse>>($"api/seasons?vaultId={vaultId}") ?? [];

    public async Task<SeasonResponse> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<SeasonResponse>($"api/seasons/{id}")
            ?? throw new KeyNotFoundException($"Season {id} not found.");

    public async Task<SeasonResponse> CreateAsync(CreateSeasonRequest request)
    {
        var response = await http.PostAsJsonAsync("api/seasons", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SeasonResponse>()
            ?? throw new InvalidOperationException("Invalid server response.");
    }

    public async Task ActivateAsync(int id)
    {
        var response = await http.PutAsync($"api/seasons/{id}/activate", null);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to activate season." : error);
        }
    }

    public async Task FinishAsync(int id)
    {
        var response = await http.PutAsync($"api/seasons/{id}/finish", null);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to finish season." : error);
        }
    }

    public async Task ConfigureAsync(int id, ConfigureSeasonRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/seasons/{id}/configure", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to configure season." : error);
        }
    }
}
