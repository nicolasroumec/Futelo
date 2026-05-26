using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.DTOs.SuperCup;

namespace Futelo.Client.Services.SuperCup;

public interface ISuperCupService
{
    Task<SuperCupResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task StartAsync(int id);
    Task<RecordSuperCupResultResponse> RecordResultAsync(int superCupId, int matchId, RecordSuperCupResultRequest request);
    Task PatchMatchAsync(int superCupId, int matchId, PatchMatchRequest request);
    Task PatchDatesAsync(int id, PatchDatesRequest request);
}
