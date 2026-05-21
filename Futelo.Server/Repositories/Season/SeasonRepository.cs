using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Futelo.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Season;

public class SeasonRepository(FuteloContext context) : BaseRepository<Models.Season>(context), ISeasonRepository
{
    public async Task<IEnumerable<Models.Season>> GetByVaultAsync(int vaultId)
        => await Context.Set<Models.Season>()
            .Include(s => s.VideoGame)
            .Include(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(s => s.Players).ThenInclude(sp => sp.Team)
            .Include(s => s.League)
            .Include(s => s.Cup)
            .Include(s => s.SuperCup)
            .Where(s => s.VaultId == vaultId)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<Models.Season?> GetByIdAsync(int id)
        => await Context.Set<Models.Season>()
            .Include(s => s.Vault).ThenInclude(v => v.Players)
            .Include(s => s.VideoGame)
            .Include(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(s => s.Players).ThenInclude(sp => sp.Team)
            .Include(s => s.League).ThenInclude(l => l!.Matches).ThenInclude(m => m.HomePlayer)
            .Include(s => s.League).ThenInclude(l => l!.Matches).ThenInclude(m => m.AwayPlayer)
            .Include(s => s.League).ThenInclude(l => l!.Players)
            .Include(s => s.Cup).ThenInclude(c => c!.Rounds).ThenInclude(r => r.Matches).ThenInclude(m => m.HomePlayer)
            .Include(s => s.Cup).ThenInclude(c => c!.Rounds).ThenInclude(r => r.Matches).ThenInclude(m => m.AwayPlayer)
            .Include(s => s.SuperCup).ThenInclude(sc => sc!.Matches).ThenInclude(m => m.HomePlayer)
            .Include(s => s.SuperCup).ThenInclude(sc => sc!.Matches).ThenInclude(m => m.AwayPlayer)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task CreateAsync(Models.Season season)
    {
        Create(season);
        await SaveChangesAsync();
    }

    public async Task ConfigureAsync(int seasonId, List<SeasonPlayer> players, bool hasLeague, string leagueName, bool leagueIsHomeAndAway, TiebreakerRule leagueTiebreakerRule, DateTime? leagueStartDate, DateTime? leagueEndDate, bool hasCup, string cupName, DateTime? cupStartDate, DateTime? cupEndDate, bool hasSuperCup, string superCupName, DateTime? superCupStartDate, DateTime? superCupEndDate)
    {
        var existing = await Context.Set<SeasonPlayer>().Where(p => p.SeasonId == seasonId).ToListAsync();
        Context.Set<SeasonPlayer>().RemoveRange(existing);
        Context.Set<SeasonPlayer>().AddRange(players);

        var league = await Context.Set<Models.League>().FirstOrDefaultAsync(l => l.SeasonId == seasonId);
        if (hasLeague && league == null)
            Context.Set<Models.League>().Add(new Models.League { SeasonId = seasonId, Name = leagueName, IsHomeAndAway = leagueIsHomeAndAway, TiebreakerRule = leagueTiebreakerRule, StartDate = leagueStartDate, EndDate = leagueEndDate });
        else if (hasLeague && league != null)
        {
            league.Name = leagueName;
            league.IsHomeAndAway = leagueIsHomeAndAway;
            league.TiebreakerRule = leagueTiebreakerRule;
            league.StartDate = leagueStartDate;
            league.EndDate = leagueEndDate;
        }
        else if (!hasLeague && league != null)
            Context.Set<Models.League>().Remove(league);

        var cup = await Context.Set<Models.Cup>().FirstOrDefaultAsync(c => c.SeasonId == seasonId);
        if (hasCup && cup == null)
            Context.Set<Models.Cup>().Add(new Models.Cup { SeasonId = seasonId, Name = cupName, StartDate = cupStartDate, EndDate = cupEndDate });
        else if (hasCup && cup != null)
        {
            cup.Name = cupName;
            cup.StartDate = cupStartDate;
            cup.EndDate = cupEndDate;
        }
        else if (!hasCup && cup != null)
            Context.Set<Models.Cup>().Remove(cup);

        var superCup = await Context.Set<Models.SuperCup>().FirstOrDefaultAsync(sc => sc.SeasonId == seasonId);
        if (hasSuperCup && superCup == null)
            Context.Set<Models.SuperCup>().Add(new Models.SuperCup { SeasonId = seasonId, Name = superCupName, StartDate = superCupStartDate, EndDate = superCupEndDate });
        else if (hasSuperCup && superCup != null)
        {
            superCup.Name = superCupName;
            superCup.StartDate = superCupStartDate;
            superCup.EndDate = superCupEndDate;
        }
        else if (!hasSuperCup && superCup != null)
            Context.Set<Models.SuperCup>().Remove(superCup);

        await SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int seasonId, SeasonStatus status)
    {
        var season = await Context.Set<Models.Season>().FindAsync(seasonId);
        if (season == null) return;
        season.Status = status;
        await SaveChangesAsync();
    }

    public async Task PatchVideoGameAsync(int seasonId, int? videoGameId)
    {
        var season = await Context.Set<Models.Season>().FindAsync(seasonId);
        if (season == null) return;
        season.VideoGameId = videoGameId;
        await SaveChangesAsync();
    }

    public async Task PatchDatesAsync(int seasonId, DateTime? startDate, DateTime? endDate)
    {
        var season = await Context.Set<Models.Season>().FindAsync(seasonId);
        if (season == null) return;
        season.StartDate = startDate;
        season.EndDate = endDate;
        await SaveChangesAsync();
    }

    public async Task SetPlayerTeamAsync(int seasonId, string playerId, int? teamId)
    {
        var sp = await Context.Set<SeasonPlayer>()
            .FirstOrDefaultAsync(sp => sp.SeasonId == seasonId && sp.PlayerId == playerId);
        if (sp == null) return;
        sp.TeamId = teamId;
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(int seasonId)
    {
        // EloHistory must go first (Restrict FK on Season and Match)
        var eloHistories = await Context.Set<EloHistory>()
            .Where(e => e.SeasonId == seasonId).ToListAsync();
        Context.Set<EloHistory>().RemoveRange(eloHistories);

        // League matches and players
        var league = await Context.Set<Models.League>()
            .Include(l => l.Matches)
            .FirstOrDefaultAsync(l => l.SeasonId == seasonId);
        if (league != null)
        {
            Context.Set<Match>().RemoveRange(league.Matches);
            var leaguePlayers = await Context.Set<LeaguePlayer>()
                .Where(lp => lp.LeagueId == league.Id).ToListAsync();
            Context.Set<LeaguePlayer>().RemoveRange(leaguePlayers);
            Context.Set<Models.League>().Remove(league);
        }

        // Cup rounds, matches and players
        var cup = await Context.Set<Models.Cup>()
            .Include(c => c.Rounds).ThenInclude(r => r.Matches)
            .FirstOrDefaultAsync(c => c.SeasonId == seasonId);
        if (cup != null)
        {
            foreach (var round in cup.Rounds)
                Context.Set<Match>().RemoveRange(round.Matches);
            Context.Set<CupRound>().RemoveRange(cup.Rounds);
            var cupPlayers = await Context.Set<CupPlayer>()
                .Where(cp => cp.CupId == cup.Id).ToListAsync();
            Context.Set<CupPlayer>().RemoveRange(cupPlayers);
            Context.Set<Models.Cup>().Remove(cup);
        }

        // SuperCup matches
        var superCup = await Context.Set<Models.SuperCup>()
            .Include(sc => sc.Matches)
            .FirstOrDefaultAsync(sc => sc.SeasonId == seasonId);
        if (superCup != null)
        {
            Context.Set<Match>().RemoveRange(superCup.Matches);
            Context.Set<Models.SuperCup>().Remove(superCup);
        }

        // Season players and the season itself
        var seasonPlayers = await Context.Set<SeasonPlayer>()
            .Where(sp => sp.SeasonId == seasonId).ToListAsync();
        Context.Set<SeasonPlayer>().RemoveRange(seasonPlayers);

        var season = await Context.Set<Models.Season>().FindAsync(seasonId);
        if (season != null)
            Context.Set<Models.Season>().Remove(season);

        await SaveChangesAsync();
    }
}
