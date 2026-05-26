using Futelo.Shared.DTOs.Season;

namespace Futelo.Server.Services.Season;

public interface ISeasonRecapService
{
    Task<SeasonRecapResponse> GetRecapAsync(int seasonId);
}
