using Futelo.Shared.DTOs.League;

namespace Futelo.Server.Services.League;

public interface ILeagueService
{
    Task<LeagueResponse> GetByIdAsync(int leagueId, string userId);
    Task GenerateFixtureAsync(int leagueId, string userId);
    Task RegenerateFixtureAsync(int leagueId, string userId);
    Task StartManualAsync(int leagueId, string userId);
    Task AddMatchManuallyAsync(int leagueId, AddLeagueMatchRequest request, string userId);
    Task<RecordResultResponse> RecordResultAsync(int leagueId, int matchId, int homeScore, int awayScore, string userId);
    Task PatchMatchAsync(int leagueId, int matchId, int? homeTeamId, int? awayTeamId, int? videoGameId, DateTime? scheduledDate, string userId);
    Task PatchDatesAsync(int leagueId, string userId, DateTime? startDate, DateTime? endDate);
}
