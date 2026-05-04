using System.Net.Http.Json;
using Futelo.Shared.DTOs.Team;

namespace Futelo.Client.Services.Teams;

public class TeamService(HttpClient http) : ITeamService
{
    public async Task<List<TeamResponse>> GetAllAsync()
        => await http.GetFromJsonAsync<List<TeamResponse>>("api/team") ?? [];
}
