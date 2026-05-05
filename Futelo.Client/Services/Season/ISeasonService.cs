using Futelo.Shared.DTOs.Season;

namespace Futelo.Client.Services.Season;

public interface ISeasonService
{
    Task<List<SeasonResponse>> GetByVaultAsync(int vaultId);
    Task<SeasonResponse> GetByIdAsync(int id);
    Task<SeasonResponse> CreateAsync(CreateSeasonRequest request);
    Task ConfigureAsync(int id, ConfigureSeasonRequest request);
}
