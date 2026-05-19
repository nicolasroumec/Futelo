using Futelo.Shared.DTOs.Season;

namespace Futelo.Client.Services.Season;

public class SeasonService(HttpClient http) : ApiService(http), ISeasonService
{
    public Task<List<SeasonResponse>> GetByVaultAsync(int vaultId, CancellationToken ct = default)
        => GetListAsync<SeasonResponse>($"api/seasons?vaultId={vaultId}", ct);

    public Task<SeasonResponse> GetByIdAsync(int id, CancellationToken ct = default)
        => GetAsync<SeasonResponse>($"api/seasons/{id}", ct);

    public Task<SeasonResponse> CreateAsync(CreateSeasonRequest request)
        => PostAsync<SeasonResponse>("api/seasons", request);

    public Task ActivateAsync(int id)
        => PutAsync($"api/seasons/{id}/activate");

    public Task FinishAsync(int id)
        => PutAsync($"api/seasons/{id}/finish");

    public Task DeleteAsync(int id)
        => DeleteAsync($"api/seasons/{id}");

    public Task SetPlayerTeamAsync(int id, string playerId, int? teamId)
        => PatchAsync($"api/seasons/{id}/players/{playerId}/team", new { TeamId = teamId });

    public Task PatchVideoGameAsync(int id, int? videoGameId)
        => PatchAsync($"api/seasons/{id}/video-game", new { VideoGameId = videoGameId });

    public Task ConfigureAsync(int id, ConfigureSeasonRequest request)
        => PutAsync($"api/seasons/{id}/configure", request);
}
