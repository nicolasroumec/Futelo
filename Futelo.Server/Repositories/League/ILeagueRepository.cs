using Futelo.Server.Models;
using Futelo.Shared.Enums;

namespace Futelo.Server.Repositories.League;

public interface ILeagueRepository
{
    Task<Models.League?> GetByIdAsync(int id);
    Task UpdateStatusAsync(int leagueId, TournamentStatus status);
    Task SetFixtureAsync(int leagueId, List<LeaguePlayer> players, List<Match> matches);
    Task SaveMatchResultAsync(MatchResultData data);
}
