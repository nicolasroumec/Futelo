using Futelo.Shared.DTOs.Team;

namespace Futelo.Server.Services.Teams;

public interface ITeamService
{
    Task<List<TeamResponse>> GetAllAsync();
}
