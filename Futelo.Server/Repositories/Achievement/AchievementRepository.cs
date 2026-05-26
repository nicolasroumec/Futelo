using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Achievement;

public class AchievementRepository(FuteloContext context) : IAchievementRepository
{
    public async Task<HashSet<AchievementType>> GetUnlockedTypesAsync(string playerId, int vaultId)
        => (await context.PlayerAchievements
            .Where(a => a.PlayerId == playerId && a.VaultId == vaultId)
            .Select(a => a.Type)
            .ToListAsync())
            .ToHashSet();

    public async Task<List<PlayerAchievement>> GetByPlayerAndVaultAsync(string playerId, int vaultId)
        => await context.PlayerAchievements
            .Where(a => a.PlayerId == playerId && a.VaultId == vaultId)
            .OrderBy(a => a.UnlockedAt)
            .AsNoTracking()
            .ToListAsync();

    public async Task AwardManyAsync(IEnumerable<PlayerAchievement> achievements)
    {
        context.PlayerAchievements.AddRange(achievements);
        await context.SaveChangesAsync();
    }
}
