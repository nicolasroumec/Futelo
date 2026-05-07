using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Season;

public class SeasonRepository(FuteloContext context) : BaseRepository<Models.Season>(context), ISeasonRepository
{
    public async Task<IEnumerable<Models.Season>> GetByVaultAsync(int vaultId)
        => await Context.Set<Models.Season>()
            .Include(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(s => s.League)
            .Include(s => s.Cup)
            .Include(s => s.SuperCup)
            .Where(s => s.VaultId == vaultId)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<Models.Season?> GetByIdAsync(int id)
        => await Context.Set<Models.Season>()
            .Include(s => s.Vault).ThenInclude(v => v.Players)
            .Include(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(s => s.League)
            .Include(s => s.Cup)
            .Include(s => s.SuperCup)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task CreateAsync(Models.Season season)
    {
        Create(season);
        await SaveChangesAsync();
    }

    public async Task ConfigureAsync(int seasonId, List<SeasonPlayer> players, bool hasLeague, bool hasCup, bool hasSuperCup)
    {
        var existing = await Context.Set<SeasonPlayer>().Where(p => p.SeasonId == seasonId).ToListAsync();
        Context.Set<SeasonPlayer>().RemoveRange(existing);
        Context.Set<SeasonPlayer>().AddRange(players);

        var league = await Context.Set<Models.League>().FirstOrDefaultAsync(l => l.SeasonId == seasonId);
        if (hasLeague && league == null)
            Context.Set<Models.League>().Add(new Models.League { SeasonId = seasonId });
        else if (!hasLeague && league != null)
            Context.Set<Models.League>().Remove(league);

        var cup = await Context.Set<Cup>().FirstOrDefaultAsync(c => c.SeasonId == seasonId);
        if (hasCup && cup == null)
            Context.Set<Cup>().Add(new Cup { SeasonId = seasonId });
        else if (!hasCup && cup != null)
            Context.Set<Cup>().Remove(cup);

        var superCup = await Context.Set<SuperCup>().FirstOrDefaultAsync(sc => sc.SeasonId == seasonId);
        if (hasSuperCup && superCup == null)
            Context.Set<SuperCup>().Add(new SuperCup { SeasonId = seasonId });
        else if (!hasSuperCup && superCup != null)
            Context.Set<SuperCup>().Remove(superCup);

        await SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int seasonId, SeasonStatus status)
    {
        var season = await Context.Set<Models.Season>().FindAsync(seasonId);
        if (season == null) return;
        season.Status = status;
        await SaveChangesAsync();
    }
}
