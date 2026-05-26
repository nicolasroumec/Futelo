using Futelo.Server.Models;
using Futelo.Shared.Enums;

namespace Futelo.Server.Repositories.Achievement;

public interface IAchievementRepository
{
    Task<HashSet<AchievementType>> GetUnlockedTypesAsync(string playerId, int vaultId);
    Task<List<PlayerAchievement>> GetByPlayerAndVaultAsync(string playerId, int vaultId);
    Task AwardManyAsync(IEnumerable<PlayerAchievement> achievements);
}
