using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Shared;

public class EloRollbackRepository(FuteloContext context) : IEloRollbackRepository
{
    public async Task<bool> IsLastMatchForBothPlayersAsync(int matchId, string player1Id, string player2Id)
    {
        var lastHome = await context.Set<EloHistory>()
            .Where(h => h.PlayerId == player1Id)
            .OrderByDescending(h => h.Id)
            .Select(h => (int?)h.MatchId)
            .FirstOrDefaultAsync();

        var lastAway = await context.Set<EloHistory>()
            .Where(h => h.PlayerId == player2Id)
            .OrderByDescending(h => h.Id)
            .Select(h => (int?)h.MatchId)
            .FirstOrDefaultAsync();

        return lastHome == matchId && lastAway == matchId;
    }

    public async Task RollbackMatchEloAsync(int matchId, string player1Id, string player2Id, int seasonId)
    {
        var histories = await context.Set<EloHistory>()
            .Where(h => h.MatchId == matchId && (h.PlayerId == player1Id || h.PlayerId == player2Id))
            .ToListAsync();

        var vaultId = await context.Set<Models.Season>()
            .Where(s => s.Id == seasonId)
            .Select(s => s.VaultId)
            .FirstAsync();

        foreach (var h in histories)
        {
            if (h.IsSeasonElo)
            {
                var sp = await context.Set<SeasonPlayer>()
                    .FirstOrDefaultAsync(sp => sp.SeasonId == seasonId && sp.PlayerId == h.PlayerId);
                if (sp != null) sp.SeasonElo = h.EloBefore;
            }
            else
            {
                var vp = await context.Set<VaultPlayer>()
                    .FirstOrDefaultAsync(vp => vp.VaultId == vaultId && vp.PlayerId == h.PlayerId);
                if (vp != null) vp.EloRating = h.EloBefore;
            }
        }

        context.Set<EloHistory>().RemoveRange(histories);

        var match = await context.Set<Match>().FindAsync(matchId);
        if (match != null)
        {
            match.HomeScore = null;
            match.AwayScore = null;
            match.WonOnPenaltiesId = null;
            match.HomePenaltyScore = null;
            match.AwayPenaltyScore = null;
            match.Status = MatchStatus.Pending;
            match.PlayedAt = null;
        }

        await context.SaveChangesAsync();
    }
}
