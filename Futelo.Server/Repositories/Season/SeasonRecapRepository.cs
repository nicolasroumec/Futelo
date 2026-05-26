using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Season;

public class SeasonRecapRepository(FuteloContext context) : ISeasonRecapRepository
{
    public async Task<Models.Season?> GetSeasonWithChampionsAsync(int seasonId)
        => await context.Seasons
            .Include(s => s.League).ThenInclude(l => l!.Champion)
            .Include(s => s.Cup).ThenInclude(c => c!.Champion)
            .Include(s => s.SuperCup).ThenInclude(sc => sc!.Champion)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(s => s.Id == seasonId);

    public async Task<List<Match>> GetSeasonMatchesAsync(int seasonId)
        => await context.Matches
            .Where(m => m.Status == MatchStatus.Played)
            .Where(m =>
                (m.LeagueId != null && m.League!.SeasonId == seasonId) ||
                (m.CupRoundId != null && m.CupRound!.Cup.SeasonId == seasonId) ||
                (m.SuperCupId != null && m.SuperCup!.SeasonId == seasonId))
            .Include(m => m.HomePlayer)
            .Include(m => m.AwayPlayer)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<EloHistory>> GetSeasonEloHistoriesAsync(int seasonId)
        => await context.EloHistories
            .Where(e => e.SeasonId == seasonId && e.IsSeasonElo)
            .Include(e => e.Player)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();
}
