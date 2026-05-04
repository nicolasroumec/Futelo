using System.Net.Http.Json;
using Futelo.Shared.DTOs.Team;

namespace Futelo.Client.Services.Teams;

public class TeamService(HttpClient http) : ITeamService
{
    public async Task<List<TeamResponse>> GetAllAsync()
        => await http.GetFromJsonAsync<List<TeamResponse>>("api/teams") ?? [];

    public async Task<TeamResponse> CreateAsync(CreateTeamRequest request)
    {
        var response = await http.PostAsJsonAsync("api/teams", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TeamResponse>()
            ?? throw new InvalidOperationException("Invalid server response.");
    }

    public async Task UpdateAsync(int id, CreateTeamRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/teams/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/teams/{id}");
        response.EnsureSuccessStatusCode();
    }
}
