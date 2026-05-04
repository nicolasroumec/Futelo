using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Vault;

public class VaultRepository(FuteloContext context) : BaseRepository<Models.Vault>(context), IVaultRepository
{
    public async Task<IEnumerable<Models.Vault>> GetByUserAsync(string userId)
        => await Context.Set<Models.Vault>()
            .Include(v => v.Owner)
            .Include(v => v.Players).ThenInclude(p => p.Player)
            .Where(v => v.Players.Any(p => p.PlayerId == userId))
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<Models.Vault?> GetByIdAsync(int id)
        => await Context.Set<Models.Vault>()
            .Include(v => v.Owner)
            .Include(v => v.Players).ThenInclude(p => p.Player)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(v => v.Id == id);

    public async Task CreateAsync(Models.Vault vault)
    {
        Create(vault);
        await SaveChangesAsync();
    }

    public async Task UpdateAsync(Models.Vault vault)
    {
        Update(vault);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(Models.Vault vault)
    {
        Delete(vault);
        await SaveChangesAsync();
    }
}
