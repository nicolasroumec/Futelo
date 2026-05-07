using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.League;

public class LeagueRepository(FuteloContext context) : BaseRepository<Models.League>(context), ILeagueRepository
{
    public async Task<Models.League?> GetByIdAsync(int id)
        => await Context.Set<Models.League>()
            .Include(l => l.Season).ThenInclude(s => s.Vault).ThenInclude(v => v.Players)
            .Include(l => l.Season).ThenInclude(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(l => l.Players).ThenInclude(lp => lp.Player)
            .Include(l => l.Matches)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task UpdateStatusAsync(int leagueId, TournamentStatus status)
    {
        var league = await Context.Set<Models.League>().FindAsync(leagueId);
        if (league == null) return;
        league.Status = status;
        await SaveChangesAsync();
    }

    public async Task SetFixtureAsync(int leagueId, List<LeaguePlayer> players, List<Match> matches)
    {
        var existingPlayers = await Context.Set<LeaguePlayer>()
            .Where(lp => lp.LeagueId == leagueId).ToListAsync();
        Context.Set<LeaguePlayer>().RemoveRange(existingPlayers);

        var existingMatches = await Context.Set<Match>()
            .Where(m => m.LeagueId == leagueId).ToListAsync();
        Context.Set<Match>().RemoveRange(existingMatches);

        Context.Set<LeaguePlayer>().AddRange(players);
        Context.Set<Match>().AddRange(matches);
        await SaveChangesAsync();
    }

    public async Task ClearFixtureAsync(int leagueId)
    {
        var matches = await Context.Set<Match>()
            .Where(m => m.LeagueId == leagueId).ToListAsync();
        Context.Set<Match>().RemoveRange(matches);

        var players = await Context.Set<LeaguePlayer>()
            .Where(lp => lp.LeagueId == leagueId).ToListAsync();
        Context.Set<LeaguePlayer>().RemoveRange(players);

        await SaveChangesAsync();
    }
}
