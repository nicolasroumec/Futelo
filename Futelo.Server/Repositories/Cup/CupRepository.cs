using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Futelo.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Cup;

public class CupRepository(FuteloContext context) : BaseRepository<Models.Cup>(context), ICupRepository
{
    public async Task<Models.Cup?> GetByIdAsync(int id)
        => await Context.Set<Models.Cup>()
            .Include(c => c.Season).ThenInclude(s => s.Vault).ThenInclude(v => v.Players)
            .Include(c => c.Season).ThenInclude(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(c => c.Season).ThenInclude(s => s.Players).ThenInclude(sp => sp.Team)
            .Include(c => c.Season).ThenInclude(s => s.League).ThenInclude(l => l!.Matches)
            .Include(c => c.Season).ThenInclude(s => s.League).ThenInclude(l => l!.Players)
            .Include(c => c.Players).ThenInclude(cp => cp.Player)
            .Include(c => c.Rounds).ThenInclude(r => r.Matches).ThenInclude(m => m.HomePlayer)
            .Include(c => c.Rounds).ThenInclude(r => r.Matches).ThenInclude(m => m.AwayPlayer)
            .Include(c => c.Rounds).ThenInclude(r => r.Matches).ThenInclude(m => m.HomeTeam)
            .Include(c => c.Rounds).ThenInclude(r => r.Matches).ThenInclude(m => m.AwayTeam)
            .Include(c => c.Rounds).ThenInclude(r => r.Matches).ThenInclude(m => m.VideoGame)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task UpdateStatusAsync(int cupId, TournamentStatus status)
    {
        var cup = await Context.Set<Models.Cup>().FindAsync(cupId);
        if (cup == null) return;
        cup.Status = status;
        await SaveChangesAsync();
    }

    public async Task SetBracketAsync(int cupId, List<CupPlayer> players, List<CupRound> rounds)
    {
        var existingPlayers = await Context.Set<CupPlayer>()
            .Where(cp => cp.CupId == cupId).ToListAsync();
        Context.Set<CupPlayer>().RemoveRange(existingPlayers);

        var existingRounds = await Context.Set<CupRound>()
            .Include(r => r.Matches)
            .Where(r => r.CupId == cupId).ToListAsync();
        foreach (var round in existingRounds)
            Context.Set<Match>().RemoveRange(round.Matches);
        Context.Set<CupRound>().RemoveRange(existingRounds);

        Context.Set<CupPlayer>().AddRange(players);
        Context.Set<CupRound>().AddRange(rounds);
        await SaveChangesAsync();
    }

    public async Task SaveMatchResultAsync(CupMatchResultData data)
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

        if (data.TieWinnerId != null && data.AdvanceToMatchId.HasValue)
        {
            var nextLeg1 = await Context.Set<Match>().FindAsync(data.AdvanceToMatchId.Value);
            if (nextLeg1 != null)
            {
                if (data.AdvanceAsHome)
                    nextLeg1.HomePlayerId = data.TieWinnerId;
                else
                    nextLeg1.AwayPlayerId = data.TieWinnerId;
            }

            if (data.AdvanceToLeg2MatchId.HasValue)
            {
                var nextLeg2 = await Context.Set<Match>().FindAsync(data.AdvanceToLeg2MatchId.Value);
                if (nextLeg2 != null)
                {
                    // Leg 2 is reversed: if winner is Home in leg 1, they are Away in leg 2
                    if (data.AdvanceAsHome)
                        nextLeg2.AwayPlayerId = data.TieWinnerId;
                    else
                        nextLeg2.HomePlayerId = data.TieWinnerId;
                }
            }
        }

        if (data.CupFinished)
        {
            var cup = await Context.Set<Models.Cup>().FindAsync(data.CupId);
            if (cup != null)
            {
                cup.Status = TournamentStatus.Finished;
                cup.ChampionId = data.ChampionId;
            }

            var cupPlayers = await Context.Set<CupPlayer>()
                .Where(cp => cp.CupId == data.CupId).ToListAsync();
            foreach (var (playerId, position) in data.FinalCupPositions)
            {
                var cp = cupPlayers.FirstOrDefault(cp => cp.PlayerId == playerId);
                if (cp != null)
                    cp.CupPosition = position;
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

    public async Task PatchDatesAsync(int cupId, DateTime? startDate, DateTime? endDate)
    {
        var cup = await Context.Set<Models.Cup>().FindAsync(cupId);
        if (cup == null) return;
        cup.StartDate = startDate;
        cup.EndDate = endDate;
        await SaveChangesAsync();
    }
}
