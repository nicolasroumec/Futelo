using System.Net.Http.Json;
using Futelo.Shared.DTOs.League;

namespace Futelo.Client.Services.League;

public class LeagueService(HttpClient http) : ILeagueService
{
    public async Task<LeagueResponse> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<LeagueResponse>($"api/leagues/{id}")
            ?? throw new KeyNotFoundException($"League {id} not found.");

    public async Task StartAsync(int id)
    {
        var response = await http.PostAsync($"api/leagues/{id}/start", null);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to start league." : error);
        }
    }

    public async Task ReshuffleAsync(int id)
    {
        var response = await http.PutAsync($"api/leagues/{id}/reshuffle", null);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to reshuffle." : error);
        }
    }

    public async Task<RecordResultResponse> RecordResultAsync(int leagueId, int matchId, RecordResultRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/leagues/{leagueId}/matches/{matchId}/result", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to record result." : error);
        }
        return await response.Content.ReadFromJsonAsync<RecordResultResponse>()
            ?? throw new InvalidOperationException("Invalid server response.");
    }

    public async Task PatchMatchAsync(int leagueId, int matchId, PatchMatchRequest request)
    {
        var response = await http.PatchAsJsonAsync($"api/leagues/{leagueId}/matches/{matchId}", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Failed to update match." : error);
        }
    }
}
