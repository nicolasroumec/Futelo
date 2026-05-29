using Futelo.Shared.DTOs.Team;

namespace Futelo.Client.Services.Teams;

public interface ITeamService
{
    Task<List<TeamResponse>> GetAllAsync(CancellationToken ct = default);
    Task<TeamResponse> CreateAsync(CreateTeamRequest request);
    Task UpdateAsync(int id, CreateTeamRequest request);
    Task DeleteAsync(int id);
    Task UploadShieldAsync(int teamId, byte[] data);
    Task DeleteShieldAsync(int teamId);
}
