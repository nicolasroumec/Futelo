using Futelo.Server.Models;
using Futelo.Shared.Enums;

namespace Futelo.Server.Repositories.Season;

public interface ISeasonRepository
{
    Task<IEnumerable<Models.Season>> GetByVaultAsync(int vaultId);
    Task<Models.Season?> GetByIdAsync(int id);
    Task CreateAsync(Models.Season season);
    Task ConfigureAsync(int seasonId, List<SeasonPlayer> players, bool hasLeague, string leagueName, bool leagueIsHomeAndAway, bool hasCup, string cupName, bool hasSuperCup, string superCupName);
    Task UpdateStatusAsync(int seasonId, SeasonStatus status);
    Task PatchVideoGameAsync(int seasonId, int? videoGameId);
    Task SetPlayerTeamAsync(int seasonId, string playerId, int? teamId);
}
