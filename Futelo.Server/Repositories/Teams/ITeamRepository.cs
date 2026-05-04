using Futelo.Server.Models;

namespace Futelo.Server.Repositories.Teams;

public interface ITeamRepository
{
    Task<IEnumerable<Team>> GetAllAsync();
    Task<Team?> GetByIdAsync(int id);
    Task<Team> CreateAsync(Team team);
    Task UpdateAsync(Team team);
    Task DeleteAsync(Team team);
}
