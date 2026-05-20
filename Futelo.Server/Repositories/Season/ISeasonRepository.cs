using Futelo.Server.Models;
using Futelo.Shared.Enums;

namespace Futelo.Server.Repositories.Season;

public interface ISeasonRepository
{
    Task<IEnumerable<Models.Season>> GetByVaultAsync(int vaultId);
    Task<Models.Season?> GetByIdAsync(int id);
    Task CreateAsync(Models.Season season);
    Task ConfigureAsync(int seasonId, List<SeasonPlayer> players, bool hasLeague, string leagueName, bool leagueIsHomeAndAway, DateTime? leagueStartDate, DateTime? leagueEndDate, bool hasCup, string cupName, DateTime? cupStartDate, DateTime? cupEndDate, bool hasSuperCup, string superCupName, DateTime? superCupStartDate, DateTime? superCupEndDate);
    Task UpdateStatusAsync(int seasonId, SeasonStatus status);
    Task PatchVideoGameAsync(int seasonId, int? videoGameId);
    Task SetPlayerTeamAsync(int seasonId, string playerId, int? teamId);
    Task DeleteAsync(int seasonId);
}
