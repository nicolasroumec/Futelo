using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Futelo.Shared.Enums;
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

    public async Task<List<Match>> GetRecentMatchesAsync(int vaultId, int limit)
        => await VaultMatchesQuery(vaultId)
            .Take(limit)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<List<Match>> GetMatchesPageAsync(int vaultId, int skip, int take, string? competitionType = null)
        => await ApplyCompetitionFilter(VaultMatchesQuery(vaultId), competitionType)
            .Skip(skip)
            .Take(take)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

    public async Task<int> CountMatchesAsync(int vaultId, string? competitionType = null)
        => await ApplyCompetitionFilter(
                Context.Set<Match>()
                    .Where(m =>
                        m.Status == MatchStatus.Played &&
                        (
                            (m.League != null && m.League.Season.VaultId == vaultId) ||
                            (m.CupRound != null && m.CupRound.Cup.Season.VaultId == vaultId) ||
                            (m.SuperCup != null && m.SuperCup.Season.VaultId == vaultId)
                        )
                    ),
                competitionType)
            .CountAsync();

    private IQueryable<Match> VaultMatchesQuery(int vaultId)
        => Context.Set<Match>()
            .Include(m => m.HomePlayer)
            .Include(m => m.AwayPlayer)
            .Include(m => m.WonOnPenalties)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.VideoGame)
            .Include(m => m.League).ThenInclude(l => l!.Season)
            .Include(m => m.CupRound).ThenInclude(cr => cr!.Cup).ThenInclude(c => c.Season)
            .Include(m => m.SuperCup).ThenInclude(sc => sc!.Season)
            .Where(m =>
                m.Status == MatchStatus.Played &&
                (
                    (m.League != null && m.League.Season.VaultId == vaultId) ||
                    (m.CupRound != null && m.CupRound.Cup.Season.VaultId == vaultId) ||
                    (m.SuperCup != null && m.SuperCup.Season.VaultId == vaultId)
                )
            )
            .OrderByDescending(m => m.PlayedAt);

    private static IQueryable<Match> ApplyCompetitionFilter(IQueryable<Match> query, string? competitionType)
        => competitionType switch
        {
            "League" => query.Where(m => m.LeagueId != null),
            "Cup" => query.Where(m => m.CupRoundId != null),
            "SuperCup" => query.Where(m => m.SuperCupId != null),
            _ => query
        };

    public async Task AddPlayerAsync(VaultPlayer player)
    {
        Context.Set<VaultPlayer>().Add(player);
        await SaveChangesAsync();
    }
}
