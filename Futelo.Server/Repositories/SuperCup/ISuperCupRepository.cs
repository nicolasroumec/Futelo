using Futelo.Server.Models;
using Futelo.Shared.Enums;

namespace Futelo.Server.Repositories.SuperCup;

public interface ISuperCupRepository
{
    Task<Models.SuperCup?> GetByIdAsync(int id);
    Task SetParticipantsAsync(int superCupId, string player1Id, string player2Id, List<Match> matches);
    Task SaveMatchResultAsync(SuperCupMatchResultData data);
    Task PatchMatchAsync(int matchId, int? homeTeamId, int? awayTeamId, int? videoGameId, DateTime? scheduledDate);
    Task PatchDatesAsync(int superCupId, DateTime? startDate, DateTime? endDate);
}
