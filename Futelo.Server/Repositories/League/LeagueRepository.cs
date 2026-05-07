using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Futelo.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.League;

public class LeagueRepository(FuteloContext context) : BaseRepository<Models.League>(context), ILeagueRepository
{
    public async Task<Models.League?> GetByIdAsync(int id)
        => await Context.Set<Models.League>()
            .Include(l => l.Season).ThenInclude(s => s.Vault).ThenInclude(v => v.Players)
            .Include(l => l.Season).ThenInclude(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(l => l.Players).ThenInclude(lp => lp.Player)
            .Include(l => l.Matches).ThenInclude(m => m.HomePlayer)
            .Include(l => l.Matches).ThenInclude(m => m.AwayPlayer)
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

    public async Task SaveMatchResultAsync(MatchResultData data)
    {
        var match = await Context.Set<Match>().FindAsync(data.MatchId);
        if (match != null)
        {
            match.HomeScore = data.HomeScore;
            match.AwayScore = data.AwayScore;
            match.Status = MatchStatus.Played;
            match.PlayedAt = DateTime.UtcNow;
        }

        var homeSeasonPlayer = await Context.Set<SeasonPlayer>()
            .FirstOrDefaultAsync(sp => sp.SeasonId == data.SeasonId && sp.PlayerId == data.HomePlayerId);
        if (homeSeasonPlayer != null)
            homeSeasonPlayer.SeasonElo = data.HomeNewSeasonElo;

        var awaySeasonPlayer = await Context.Set<SeasonPlayer>()
            .FirstOrDefaultAsync(sp => sp.SeasonId == data.SeasonId && sp.PlayerId == data.AwayPlayerId);
        if (awaySeasonPlayer != null)
            awaySeasonPlayer.SeasonElo = data.AwayNewSeasonElo;

        var homeUser = await Context.Set<AppUser>().FindAsync(data.HomePlayerId);
        if (homeUser != null)
            homeUser.EloRating = data.HomeNewHistoricalElo;

        var awayUser = await Context.Set<AppUser>().FindAsync(data.AwayPlayerId);
        if (awayUser != null)
            awayUser.EloRating = data.AwayNewHistoricalElo;

        Context.Set<EloHistory>().AddRange(data.EloHistories);

        if (data.LeagueFinished)
        {
            var league = await Context.Set<Models.League>().FindAsync(data.LeagueId);
            if (league != null)
            {
                league.Status = TournamentStatus.Finished;
                league.ChampionId = data.ChampionId;
            }

            var leaguePlayers = await Context.Set<LeaguePlayer>()
                .Where(lp => lp.LeagueId == data.LeagueId)
                .ToListAsync();

            foreach (var (playerId, position) in data.FinalLeaguePositions)
            {
                var lp = leaguePlayers.FirstOrDefault(lp => lp.PlayerId == playerId);
                if (lp != null)
                    lp.LeaguePosition = position;
            }
        }

        await SaveChangesAsync();
    }
}
