using Futelo.Server.Models;

namespace Futelo.Server.Repositories.League;

public interface ILeagueRepository
{
    Task<Models.League?> GetByIdAsync(int id);
    Task UpdateAsync(Models.League league);
    Task AddMatchesAsync(IEnumerable<Match> matches);
}
