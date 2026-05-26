using Futelo.Server.Models;

namespace Futelo.Server.Repositories.Season;

public interface ISeasonRecapRepository
{
    Task<Models.Season?> GetSeasonWithChampionsAsync(int seasonId);
    Task<List<Match>> GetSeasonMatchesAsync(int seasonId);
    Task<List<EloHistory>> GetSeasonEloHistoriesAsync(int seasonId);
}
