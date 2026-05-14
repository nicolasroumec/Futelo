using System.Net.Http.Json;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.DTOs.SuperCup;

namespace Futelo.Client.Services.SuperCup;

public class SuperCupService(HttpClient http) : ISuperCupService
{
    public async Task<SuperCupResponse> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<SuperCupResponse>($"api/supercups/{id}")
            ?? throw new KeyNotFoundException($"SuperCup {id} not found.");

    public async Task StartAsync(int id)
    {
        var response = await http.PostAsync($"api/supercups/{id}/start", null);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to start SuperCup." : error);
        }
    }

    public async Task<RecordSuperCupResultResponse> RecordResultAsync(int superCupId, int matchId, RecordSuperCupResultRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/supercups/{superCupId}/matches/{matchId}/result", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to record result." : error);
        }
        return await response.Content.ReadFromJsonAsync<RecordSuperCupResultResponse>()
            ?? throw new InvalidOperationException("Invalid server response.");
    }

    public async Task PatchMatchAsync(int superCupId, int matchId, PatchMatchRequest request)
    {
        var response = await http.PatchAsJsonAsync($"api/supercups/{superCupId}/matches/{matchId}", request);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Failed to patch match.");
    }
}
