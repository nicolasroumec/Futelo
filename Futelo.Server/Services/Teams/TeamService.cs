using Futelo.Server.Repositories.Teams;
using Futelo.Shared.DTOs.Team;

namespace Futelo.Server.Services.Teams;

public class TeamService(ITeamRepository repository) : ITeamService
{
    public async Task<List<TeamResponse>> GetAllAsync()
    {
        var teams = await repository.GetAllAsync();
        return teams.Select(t => new TeamResponse { Id = t.Id, Name = t.Name }).ToList();
    }
}
