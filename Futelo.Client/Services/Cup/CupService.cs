using System.Net.Http.Json;
using Futelo.Shared.DTOs.Cup;
using Futelo.Shared.DTOs.League;

namespace Futelo.Client.Services.Cup;

public class CupService(HttpClient http) : ICupService
{
    public async Task<CupResponse> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<CupResponse>($"api/cups/{id}")
            ?? throw new KeyNotFoundException($"Cup {id} not found.");

    public async Task StartAsync(int id)
    {
        var response = await http.PostAsync($"api/cups/{id}/start", null);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to start cup." : error);
        }
    }

    public async Task<RecordCupResultResponse> RecordResultAsync(int cupId, int matchId, RecordCupResultRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/cups/{cupId}/matches/{matchId}/result", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to record result." : error);
        }
        return await response.Content.ReadFromJsonAsync<RecordCupResultResponse>()
            ?? throw new InvalidOperationException("Invalid server response.");
    }

    public async Task PatchMatchAsync(int cupId, int matchId, PatchMatchRequest request)
    {
        var response = await http.PatchAsJsonAsync($"api/cups/{cupId}/matches/{matchId}", request);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Failed to patch match.");
    }
}
