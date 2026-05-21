using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.DTOs.SuperCup;

namespace Futelo.Client.Services.SuperCup;

public class SuperCupService(HttpClient http) : ApiService(http), ISuperCupService
{
    public Task<SuperCupResponse> GetByIdAsync(int id, CancellationToken ct = default)
        => GetAsync<SuperCupResponse>($"api/supercups/{id}", ct);

    public Task StartAsync(int id)
        => PostAsync($"api/supercups/{id}/start");

    public Task<RecordSuperCupResultResponse> RecordResultAsync(int superCupId, int matchId, RecordSuperCupResultRequest request)
        => PutAsync<RecordSuperCupResultResponse>($"api/supercups/{superCupId}/matches/{matchId}/result", request);

    public Task PatchMatchAsync(int superCupId, int matchId, PatchMatchRequest request)
        => PatchAsync($"api/supercups/{superCupId}/matches/{matchId}", request);

    public Task PatchDatesAsync(int id, PatchDatesRequest request)
        => PatchAsync($"api/supercups/{id}/dates", request);
}
