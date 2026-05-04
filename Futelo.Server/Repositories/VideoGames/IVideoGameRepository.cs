using Futelo.Server.Models;

namespace Futelo.Server.Repositories.VideoGames;

public interface IVideoGameRepository
{
    Task<IEnumerable<VideoGame>> GetAllAsync();
    Task<VideoGame?> GetByIdAsync(int id);
    Task<VideoGame> CreateAsync(VideoGame game);
    Task UpdateAsync(VideoGame game);
    Task DeleteAsync(VideoGame game);
}
