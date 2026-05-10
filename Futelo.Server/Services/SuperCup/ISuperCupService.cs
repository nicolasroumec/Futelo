using Futelo.Shared.DTOs.SuperCup;

namespace Futelo.Server.Services.SuperCup;

public interface ISuperCupService
{
    Task<SuperCupResponse> GetByIdAsync(int superCupId, string userId);
    Task StartAsync(int superCupId, string userId);
    Task<RecordSuperCupResultResponse> RecordResultAsync(
        int superCupId, int matchId, int homeScore, int awayScore,
        string? wonOnPenaltiesId, int? homePenaltyScore, int? awayPenaltyScore, string userId);
}
