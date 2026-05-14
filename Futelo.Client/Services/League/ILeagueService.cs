using Futelo.Shared.DTOs.League;

namespace Futelo.Client.Services.League;

public interface ILeagueService
{
    Task<LeagueResponse> GetByIdAsync(int id);
    Task StartAsync(int id);
    Task ReshuffleAsync(int id);
    Task<RecordResultResponse> RecordResultAsync(int leagueId, int matchId, RecordResultRequest request);
    Task PatchMatchAsync(int leagueId, int matchId, PatchMatchRequest request);
}
