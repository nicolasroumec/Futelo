using Futelo.Server.Models;

namespace Futelo.Server.Repositories.Teams;

public interface ITeamRepository
{
    Task<IEnumerable<Team>> GetAllAsync();
    Task<Team?> GetByIdAsync(int id);
    Task<Team> CreateAsync(Team team);
    Task UpdateAsync(Team team);
    Task DeleteAsync(Team team);
    Task<byte[]?> GetShieldAsync(int teamId);
    Task SetShieldAsync(int teamId, byte[] data);
    Task DeleteShieldAsync(int teamId);
    Task<List<int>> GetTeamIdsWithShieldAsync();
}
