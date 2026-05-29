using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Futelo.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.SuperCup;

public class SuperCupRepository(FuteloContext context) : BaseRepository<Models.SuperCup>(context), ISuperCupRepository
{
    public async Task<Models.SuperCup?> GetByIdAsync(int id)
        => await Context.Set<Models.SuperCup>()
            .Include(sc => sc.Season).ThenInclude(s => s.Vault).ThenInclude(v => v.Players)
            .Include(sc => sc.Season).ThenInclude(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(sc => sc.Season).ThenInclude(s => s.Players).ThenInclude(sp => sp.Team)
            .Include(sc => sc.Season).ThenInclude(s => s.League)
            .Include(sc => sc.Season).ThenInclude(s => s.Cup).ThenInclude(c => c!.Players)
            .Include(sc => sc.Player1)
            .Include(sc => sc.Player2)
            .Include(sc => sc.Matches).ThenInclude(m => m.HomePlayer)
            .Include(sc => sc.Matches).ThenInclude(m => m.AwayPlayer)
            .Include(sc => sc.Matches).ThenInclude(m => m.HomeTeam)
            .Include(sc => sc.Matches).ThenInclude(m => m.AwayTeam)
            .Include(sc => sc.Matches).ThenInclude(m => m.VideoGame)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(sc => sc.Id == id);

    public async Task SetParticipantsAsync(int superCupId, string player1Id, string player2Id, List<Match> matches)
    {
        var superCup = await Context.Set<Models.SuperCup>().FindAsync(superCupId);
        if (superCup == null) return;

        superCup.Player1Id = player1Id;
        superCup.Player2Id = player2Id;
        superCup.Status = TournamentStatus.Active;

        Context.Set<Match>().AddRange(matches);
        await SaveChangesAsync();
    }

    public async Task SaveMatchResultAsync(SuperCupMatchResultData data)
    {
        var match = await Context.Set<Match>().FindAsync(data.MatchId);
        if (match != null)
        {
            match.HomeScore = data.HomeScore;
            match.AwayScore = data.AwayScore;
            match.WonOnPenaltiesId = data.WonOnPenaltiesId;
            match.HomePenaltyScore = data.HomePenaltyScore;
            match.AwayPenaltyScore = data.AwayPenaltyScore;
            match.Status = MatchStatus.Played;
            match.PlayedAt = DateTime.UtcNow;
            match.VideoGameId = data.VideoGameId;
            match.HomeTeamId = data.HomeTeamId;
            match.AwayTeamId = data.AwayTeamId;
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

        if (data.Finished)
        {
            var superCup = await Context.Set<Models.SuperCup>().FindAsync(data.SuperCupId);
            if (superCup != null)
            {
                superCup.Status = TournamentStatus.Finished;
                superCup.ChampionId = data.ChampionId;
            }
        }

        await SaveChangesAsync();
    }

    public async Task PatchMatchAsync(int matchId, int? homeTeamId, int? awayTeamId, int? videoGameId, DateTime? scheduledDate)
    {
        var match = await Context.Set<Match>().FindAsync(matchId);
        if (match == null) return;
        match.HomeTeamId = homeTeamId;
        match.AwayTeamId = awayTeamId;
        match.VideoGameId = videoGameId;
        match.ScheduledDate = scheduledDate;
        await SaveChangesAsync();
    }

    public async Task PatchDatesAsync(int superCupId, DateTime? startDate, DateTime? endDate)
    {
        var superCup = await Context.Set<Models.SuperCup>().FindAsync(superCupId);
        if (superCup == null) return;
        superCup.StartDate = startDate;
        superCup.EndDate = endDate;
        await SaveChangesAsync();
    }

    public async Task ResetSuperCupFinishAsync(int superCupId)
    {
        var superCup = await Context.Set<Models.SuperCup>().FindAsync(superCupId);
        if (superCup != null)
        {
            superCup.Status = TournamentStatus.Active;
            superCup.ChampionId = null;
        }

        await SaveChangesAsync();
    }
}
