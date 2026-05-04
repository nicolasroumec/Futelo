using Futelo.Server.Models;
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

    public async Task<TeamResponse> CreateAsync(CreateTeamRequest request)
    {
        var team = new Team { Name = request.Name };
        await repository.CreateAsync(team);
        return new TeamResponse { Id = team.Id, Name = team.Name };
    }

    public async Task UpdateAsync(int id, CreateTeamRequest request)
    {
        var team = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Team {id} not found.");
        team.Name = request.Name;
        await repository.UpdateAsync(team);
    }

    public async Task DeleteAsync(int id)
    {
        var team = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Team {id} not found.");
        await repository.DeleteAsync(team);
    }
}
