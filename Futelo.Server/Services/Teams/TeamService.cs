using Futelo.Server.Models;
using Futelo.Server.Repositories.Teams;
using Futelo.Shared.DTOs.Team;
using Microsoft.Extensions.Caching.Memory;

namespace Futelo.Server.Services.Teams;

using static ErrorMessages;

public class TeamService(ITeamRepository repository, IMemoryCache cache, ILogger<TeamService> logger) : ITeamService
{
    private const string CacheKey = "teams_all";

    public async Task<List<TeamResponse>> GetAllAsync()
    {
        if (cache.TryGetValue(CacheKey, out List<TeamResponse>? cached))
            return cached!;

        var teams = await repository.GetAllAsync();
        var result = teams.Select(t => new TeamResponse { Id = t.Id, Name = t.Name }).ToList();
        cache.Set(CacheKey, result, TimeSpan.FromMinutes(30));
        return result;
    }

    public async Task<TeamResponse> CreateAsync(CreateTeamRequest request)
    {
        var team = new Team { Name = request.Name };
        await repository.CreateAsync(team);
        cache.Remove(CacheKey);
        return new TeamResponse { Id = team.Id, Name = team.Name };
    }

    public async Task UpdateAsync(int id, CreateTeamRequest request)
    {
        var team = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Team {id} not found.");
        team.Name = request.Name;
        await repository.UpdateAsync(team);
        cache.Remove(CacheKey);
    }

    public async Task DeleteAsync(int id)
    {
        var team = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Team {id} not found.");
        await repository.DeleteAsync(team);
        cache.Remove(CacheKey);
    }
}
