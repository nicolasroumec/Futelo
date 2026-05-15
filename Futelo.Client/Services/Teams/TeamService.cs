using Futelo.Shared.DTOs.Team;

namespace Futelo.Client.Services.Teams;

public class TeamService(HttpClient http) : ApiService(http), ITeamService
{
    public Task<List<TeamResponse>> GetAllAsync(CancellationToken ct = default)
        => GetListAsync<TeamResponse>("api/teams", ct);

    public Task<TeamResponse> CreateAsync(CreateTeamRequest request)
        => PostAsync<TeamResponse>("api/teams", request);

    public Task UpdateAsync(int id, CreateTeamRequest request)
        => PutAsync($"api/teams/{id}", request);

    public Task DeleteAsync(int id)
        => DeleteAsync($"api/teams/{id}");
}
