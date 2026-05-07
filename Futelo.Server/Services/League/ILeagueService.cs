using Futelo.Shared.DTOs.League;

namespace Futelo.Server.Services.League;

public interface ILeagueService
{
    Task<LeagueResponse> GetByIdAsync(int leagueId, string userId);
    Task GenerateFixtureAsync(int leagueId, string userId);
    Task RegenerateFixtureAsync(int leagueId, string userId);
    Task<RecordResultResponse> RecordResultAsync(int leagueId, int matchId, int homeScore, int awayScore, string userId);
    Task<List<StandingRow>> GetStandingsAsync(int leagueId, string userId);
}
