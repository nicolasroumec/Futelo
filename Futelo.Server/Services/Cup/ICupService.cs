using Futelo.Shared.DTOs.Cup;

namespace Futelo.Server.Services.Cup;

public interface ICupService
{
    Task<CupResponse> GetByIdAsync(int cupId, string userId);
    Task GenerateBracketAsync(int cupId, string userId);
    Task StartManualAsync(int cupId, string userId);
    Task<int> AddRoundAsync(int cupId, AddCupRoundRequest request, string userId);
    Task AddMatchAsync(int cupId, int roundId, AddCupMatchRequest request, string userId);
    Task<RecordCupResultResponse> RecordResultAsync(int cupId, int matchId, int homeScore, int awayScore, string? wonOnPenaltiesId, int? homePenaltyScore, int? awayPenaltyScore, string userId);
    Task PatchMatchAsync(int cupId, int matchId, int? homeTeamId, int? awayTeamId, int? videoGameId, DateTime? scheduledDate, string userId);
    Task PatchDatesAsync(int cupId, string userId, DateTime? startDate, DateTime? endDate);
}
