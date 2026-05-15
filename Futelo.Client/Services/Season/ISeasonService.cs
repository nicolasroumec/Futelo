using Futelo.Shared.DTOs.Season;

namespace Futelo.Client.Services.Season;

public interface ISeasonService
{
    Task<List<SeasonResponse>> GetByVaultAsync(int vaultId, CancellationToken ct = default);
    Task<SeasonResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<SeasonResponse> CreateAsync(CreateSeasonRequest request);
    Task ConfigureAsync(int id, ConfigureSeasonRequest request);
    Task ActivateAsync(int id);
    Task FinishAsync(int id);
    Task PatchVideoGameAsync(int id, int? videoGameId);
    Task SetPlayerTeamAsync(int id, string playerId, int? teamId);
    Task DeleteAsync(int id);
}
