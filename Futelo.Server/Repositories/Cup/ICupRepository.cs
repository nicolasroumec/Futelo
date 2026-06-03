using Futelo.Server.Models;
using Futelo.Shared.Enums;

namespace Futelo.Server.Repositories.Cup;

public interface ICupRepository
{
    Task<Models.Cup?> GetByIdAsync(int id);
    Task UpdateStatusAsync(int cupId, TournamentStatus status);
    Task SetBracketAsync(int cupId, List<CupPlayer> players, List<CupRound> rounds);
    Task SaveMatchResultAsync(CupMatchResultData data);
    Task PatchMatchAsync(int matchId, int? homeTeamId, int? awayTeamId, int? videoGameId, DateTime? scheduledDate);
    Task PatchDatesAsync(int cupId, DateTime? startDate, DateTime? endDate);
    Task InitPlayersAsync(int cupId, List<CupPlayer> players);
    Task ActivateManualAsync(int cupId);
    Task<int> AddRoundAsync(CupRound round);
    Task AddMatchToRoundAsync(Match match);
    Task ResetCupFinishAsync(int cupId);
    Task RevertBracketAdvancementAsync(int cupId, string winnerId);
}
