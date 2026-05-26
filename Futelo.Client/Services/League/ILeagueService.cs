using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.League;

namespace Futelo.Client.Services.League;

public interface ILeagueService
{
    Task<LeagueResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task StartAsync(int id);
    Task StartManualAsync(int id);
    Task AddMatchAsync(int leagueId, AddLeagueMatchRequest request);
    Task ReshuffleAsync(int id);
    Task<RecordResultResponse> RecordResultAsync(int leagueId, int matchId, RecordResultRequest request);
    Task PatchMatchAsync(int leagueId, int matchId, PatchMatchRequest request);
    Task PatchDatesAsync(int id, PatchDatesRequest request);
}
