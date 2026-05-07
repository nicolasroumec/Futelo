using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.League;

public class LeagueRepository(FuteloContext context) : BaseRepository<Models.League>(context), ILeagueRepository
{
    public async Task<Models.League?> GetByIdAsync(int id)
        => await Context.Set<Models.League>()
            .Include(l => l.Season).ThenInclude(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(l => l.Players).ThenInclude(lp => lp.Player)
            .Include(l => l.Matches)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task UpdateAsync(Models.League league)
    {
        Update(league);
        await SaveChangesAsync();
    }

    public async Task AddMatchesAsync(IEnumerable<Match> matches)
    {
        Context.Set<Match>().AddRange(matches);
        await SaveChangesAsync();
    }
}
