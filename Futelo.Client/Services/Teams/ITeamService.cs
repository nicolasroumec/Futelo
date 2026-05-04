using Futelo.Shared.DTOs.Team;

namespace Futelo.Client.Services.Teams;

public interface ITeamService
{
    Task<List<TeamResponse>> GetAllAsync();
}
