using Futelo.Server.Models;
using Futelo.Shared.Enums;

namespace Futelo.Server.Repositories.League;

public interface ILeagueRepository
{
    Task<Models.League?> GetByIdAsync(int id);
    Task UpdateStatusAsync(int leagueId, TournamentStatus status);
    Task SetFixtureAsync(int leagueId, List<LeaguePlayer> players, List<Match> matches);
    Task SaveMatchResultAsync(MatchResultData data);
    Task PatchMatchAsync(int matchId, int? homeTeamId, int? awayTeamId, int? videoGameId, DateTime? scheduledDate);
    Task PatchDatesAsync(int leagueId, DateTime? startDate, DateTime? endDate);
}
