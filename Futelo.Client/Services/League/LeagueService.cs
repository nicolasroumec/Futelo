using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.League;

namespace Futelo.Client.Services.League;

public class LeagueService(HttpClient http) : ApiService(http), ILeagueService
{
    public Task<LeagueResponse> GetByIdAsync(int id, CancellationToken ct = default)
        => GetAsync<LeagueResponse>($"api/leagues/{id}", ct);

    public Task StartAsync(int id)
        => PostAsync($"api/leagues/{id}/start");

    public Task ReshuffleAsync(int id)
        => PutAsync($"api/leagues/{id}/reshuffle");

    public Task<RecordResultResponse> RecordResultAsync(int leagueId, int matchId, RecordResultRequest request)
        => PutAsync<RecordResultResponse>($"api/leagues/{leagueId}/matches/{matchId}/result", request);

    public Task PatchMatchAsync(int leagueId, int matchId, PatchMatchRequest request)
        => PatchAsync($"api/leagues/{leagueId}/matches/{matchId}", request);

    public Task PatchDatesAsync(int id, PatchDatesRequest request)
        => PatchAsync($"api/leagues/{id}/dates", request);
}
