using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.Cup;
using Futelo.Shared.DTOs.League;

namespace Futelo.Client.Services.Cup;

public class CupService(HttpClient http) : ApiService(http), ICupService
{
    public Task<CupResponse> GetByIdAsync(int id, CancellationToken ct = default)
        => GetAsync<CupResponse>($"api/cups/{id}", ct);

    public Task StartAsync(int id)
        => PostAsync($"api/cups/{id}/start");

    public Task StartManualAsync(int id)
        => PostAsync($"api/cups/{id}/start-manual");

    public Task<int> AddRoundAsync(int cupId, AddCupRoundRequest request)
        => PostAsync<int>($"api/cups/{cupId}/rounds", request);

    public Task AddMatchAsync(int cupId, int roundId, AddCupMatchRequest request)
        => PostAsync($"api/cups/{cupId}/rounds/{roundId}/matches", request);

    public Task<RecordCupResultResponse> RecordResultAsync(int cupId, int matchId, RecordCupResultRequest request)
        => PutAsync<RecordCupResultResponse>($"api/cups/{cupId}/matches/{matchId}/result", request);

    public Task PatchMatchAsync(int cupId, int matchId, PatchMatchRequest request)
        => PatchAsync($"api/cups/{cupId}/matches/{matchId}", request);

    public Task PatchDatesAsync(int id, PatchDatesRequest request)
        => PatchAsync($"api/cups/{id}/dates", request);
}
