using Futelo.Shared.DTOs.SuperCup;

namespace Futelo.Client.Services.SuperCup;

public interface ISuperCupService
{
    Task<SuperCupResponse> GetByIdAsync(int id);
    Task StartAsync(int id);
    Task<RecordSuperCupResultResponse> RecordResultAsync(int superCupId, int matchId, RecordSuperCupResultRequest request);
}
