using Futelo.Server.Models;
using Futelo.Shared.Enums;

namespace Futelo.Server.Repositories.Cup;

public interface ICupRepository
{
    Task<Models.Cup?> GetByIdAsync(int id);
    Task UpdateStatusAsync(int cupId, TournamentStatus status);
    Task SetBracketAsync(int cupId, List<CupPlayer> players, List<CupRound> rounds);
    Task SaveMatchResultAsync(CupMatchResultData data);
}
