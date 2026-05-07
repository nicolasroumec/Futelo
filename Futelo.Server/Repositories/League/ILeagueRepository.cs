using Futelo.Server.Models;

namespace Futelo.Server.Repositories.League;

public interface ILeagueRepository
{
    Task<Models.League?> GetByIdAsync(int id);
    Task UpdateStatusAsync(int leagueId, TournamentStatus status);
    Task SetFixtureAsync(int leagueId, List<LeaguePlayer> players, List<Match> matches);
    Task ClearFixtureAsync(int leagueId);
}
