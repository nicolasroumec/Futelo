using Futelo.Shared.DTOs.Team;

namespace Futelo.Server.Services.Teams;

public interface ITeamService
{
    Task<List<TeamResponse>> GetAllAsync();
    Task<TeamResponse> CreateAsync(CreateTeamRequest request);
    Task UpdateAsync(int id, CreateTeamRequest request);
    Task DeleteAsync(int id);
    Task<byte[]?> GetShieldAsync(int teamId);
    Task SetShieldAsync(int teamId, byte[] data);
    Task DeleteShieldAsync(int teamId);
    Task<List<int>> GetTeamIdsWithShieldAsync();
}
