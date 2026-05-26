using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.Cup;
using Futelo.Shared.DTOs.League;

namespace Futelo.Client.Services.Cup;

public interface ICupService
{
    Task<CupResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task StartAsync(int id);
    Task StartManualAsync(int id);
    Task<int> AddRoundAsync(int cupId, AddCupRoundRequest request);
    Task AddMatchAsync(int cupId, int roundId, AddCupMatchRequest request);
    Task<RecordCupResultResponse> RecordResultAsync(int cupId, int matchId, RecordCupResultRequest request);
    Task PatchMatchAsync(int cupId, int matchId, PatchMatchRequest request);
    Task PatchDatesAsync(int id, PatchDatesRequest request);
}
