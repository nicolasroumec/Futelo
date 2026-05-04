using Futelo.Server.Models;

namespace Futelo.Server.Repositories.Teams;

public interface ITeamRepository
{
    Task<IEnumerable<Team>> GetAllAsync();
}
