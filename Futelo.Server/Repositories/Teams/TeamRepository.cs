using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Teams;

public class TeamRepository(FuteloContext context) : BaseRepository<Team>(context), ITeamRepository
{
    public async Task<IEnumerable<Team>> GetAllAsync()
        => await FindAll().OrderBy(t => t.Name).ToListAsync();

    public async Task<Team?> GetByIdAsync(int id)
        => await FindByCondition(t => t.Id == id).FirstOrDefaultAsync();

    public async Task<Team> CreateAsync(Team team)
    {
        Create(team);
        await SaveChangesAsync();
        return team;
    }

    public async Task UpdateAsync(Team team)
    {
        Update(team);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(Team team)
    {
        Delete(team);
        await SaveChangesAsync();
    }

    public Task<byte[]?> GetShieldAsync(int teamId)
        => Context.Set<Team>()
            .Where(t => t.Id == teamId)
            .Select(t => t.Shield)
            .FirstOrDefaultAsync();

    public async Task SetShieldAsync(int teamId, byte[] data)
        => await Context.Set<Team>()
            .Where(t => t.Id == teamId)
            .ExecuteUpdateAsync(t => t.SetProperty(x => x.Shield, data));

    public async Task DeleteShieldAsync(int teamId)
        => await Context.Set<Team>()
            .Where(t => t.Id == teamId)
            .ExecuteUpdateAsync(t => t.SetProperty(x => x.Shield, (byte[]?)null));

    public Task<List<int>> GetTeamIdsWithShieldAsync()
        => Context.Set<Team>()
            .Where(t => t.Shield != null)
            .Select(t => t.Id)
            .ToListAsync();
}
