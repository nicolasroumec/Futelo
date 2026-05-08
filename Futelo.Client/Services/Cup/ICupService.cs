using Futelo.Shared.DTOs.Cup;

namespace Futelo.Client.Services.Cup;

public interface ICupService
{
    Task<CupResponse> GetByIdAsync(int id);
    Task StartAsync(int id);
    Task<RecordCupResultResponse> RecordResultAsync(int cupId, int matchId, RecordCupResultRequest request);
}
