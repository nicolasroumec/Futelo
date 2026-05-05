using Futelo.Server.Models;

namespace Futelo.Server.Repositories.Season;

public interface ISeasonRepository
{
    Task<IEnumerable<Models.Season>> GetByVaultAsync(int vaultId);
    Task<Models.Season?> GetByIdAsync(int id);
    Task CreateAsync(Models.Season season);
    Task ConfigureAsync(int seasonId, List<SeasonPlayer> players, bool hasLeague, bool hasCup, bool hasSuperCup);
}
