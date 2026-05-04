using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Teams;

public class TeamRepository(FuteloContext context) : BaseRepository<Team>(context), ITeamRepository
{
    public async Task<IEnumerable<Team>> GetAllAsync()
        => await FindAll().OrderBy(t => t.Name).ToListAsync();
}
