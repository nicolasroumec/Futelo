using Futelo.Shared.DTOs.Cup;

namespace Futelo.Server.Services.Cup;

public interface ICupService
{
    Task<CupResponse> GetByIdAsync(int cupId, string userId);
    Task GenerateBracketAsync(int cupId, string userId);
    Task<RecordCupResultResponse> RecordResultAsync(int cupId, int matchId, int homeScore, int awayScore, string? wonOnPenaltiesId, int? homePenaltyScore, int? awayPenaltyScore, string userId);
    Task PatchMatchAsync(int cupId, int matchId, int? homeTeamId, int? awayTeamId, int? videoGameId, string userId);
}
