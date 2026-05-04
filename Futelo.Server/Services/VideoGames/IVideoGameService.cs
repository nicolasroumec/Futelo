using Futelo.Shared.DTOs.VideoGame;

namespace Futelo.Server.Services.VideoGames;

public interface IVideoGameService
{
    Task<List<VideoGameResponse>> GetAllAsync();
    Task<VideoGameResponse> CreateAsync(CreateVideoGameRequest request);
    Task UpdateAsync(int id, CreateVideoGameRequest request);
    Task DeleteAsync(int id);
}
