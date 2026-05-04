using Futelo.Server.Models;

namespace Futelo.Server.Repositories.VideoGames;

public interface IVideoGameRepository
{
    Task<IEnumerable<VideoGame>> GetAllAsync();
}
