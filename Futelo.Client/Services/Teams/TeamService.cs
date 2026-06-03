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

    public async Task UploadShieldAsync(int teamId, byte[] data)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(data), "file", "shield.webp");
        var res = await Http.PutAsync($"api/teams/{teamId}/shield", content);
        await res.EnsureSuccessAsync();
    }

    public Task DeleteShieldAsync(int teamId)
        => DeleteAsync($"api/teams/{teamId}/shield");

    public Task<List<int>> GetTeamsWithShieldAsync(CancellationToken ct = default)
        => GetListAsync<int>("api/teams/with-shields", ct);
}
