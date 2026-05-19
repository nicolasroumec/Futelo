using Futelo.Shared.DTOs.Cup;
using Futelo.Shared.DTOs.League;

namespace Futelo.Client.Services.Cup;

public class CupService(HttpClient http) : ApiService(http), ICupService
{
    public Task<CupResponse> GetByIdAsync(int id, CancellationToken ct = default)
        => GetAsync<CupResponse>($"api/cups/{id}", ct);

    public Task StartAsync(int id)
        => PostAsync($"api/cups/{id}/start");

    public Task<RecordCupResultResponse> RecordResultAsync(int cupId, int matchId, RecordCupResultRequest request)
        => PutAsync<RecordCupResultResponse>($"api/cups/{cupId}/matches/{matchId}/result", request);

    public Task PatchMatchAsync(int cupId, int matchId, PatchMatchRequest request)
        => PatchAsync($"api/cups/{cupId}/matches/{matchId}", request);
}
