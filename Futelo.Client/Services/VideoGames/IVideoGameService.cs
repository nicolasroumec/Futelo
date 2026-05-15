using Futelo.Shared.DTOs.VideoGame;

namespace Futelo.Client.Services.VideoGames;

public interface IVideoGameService
{
    Task<List<VideoGameResponse>> GetAllAsync(CancellationToken ct = default);
    Task<VideoGameResponse> CreateAsync(CreateVideoGameRequest request);
    Task UpdateAsync(int id, CreateVideoGameRequest request);
    Task DeleteAsync(int id);
}
