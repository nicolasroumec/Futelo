using Futelo.Server.Models;
using SeasonModel = Futelo.Server.Models.Season;

namespace Futelo.Server.Repositories.Achievement;

public interface IAchievementEvaluationRepository
{
    // Post-match
    Task<List<Match>> GetPlayerLastMatchesInVaultAsync(string playerId, int vaultId, int count);
    Task<List<EloHistory>> GetPlayerLastHistoricalEloInVaultAsync(string playerId, int vaultId, int count);
    Task<int> CountMatchesInVaultAsync(string playerId, int vaultId);
    Task<int> CountPenaltyWinsInVaultAsync(string playerId, int vaultId);
    Task<int> CountExact1_0WinsInVaultAsync(string playerId, int vaultId);
    Task<int?> GetMinHistoricalEloInVaultAsync(string playerId, int vaultId);

    // Post-season
    Task<SeasonModel?> GetSeasonWithCompetitionsAsync(int seasonId);
    Task<List<Match>> GetSeasonMatchesAsync(int seasonId);
    Task<List<EloHistory>> GetSeasonEloHistoriesAsync(int seasonId, bool isSeasonElo);
    Task<List<SeasonPlayer>> GetSeasonPlayersAsync(int seasonId);
    Task<List<SeasonModel>> GetVaultSeasonsWithChampionsAsync(int vaultId);
}
