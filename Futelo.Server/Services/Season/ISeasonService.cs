using Futelo.Shared.DTOs.Season;

namespace Futelo.Server.Services.Season;

public interface ISeasonService
{
    Task<List<SeasonResponse>> GetByVaultAsync(int vaultId, string userId);
    Task<SeasonResponse> GetByIdAsync(int id, string userId);
    Task<SeasonResponse> CreateAsync(string userId, CreateSeasonRequest request);
    Task ConfigureAsync(int id, string userId, ConfigureSeasonRequest request);
    Task ActivateAsync(int id, string userId);
    Task FinishAsync(int id, string userId);
    Task PatchVideoGameAsync(int id, string userId, int? videoGameId);
    Task PatchDatesAsync(int id, string userId, DateTime? startDate, DateTime? endDate);
    Task SetPlayerTeamAsync(int id, string playerId, string userId, int? teamId);
    Task DeleteAsync(int id, string userId);
}
