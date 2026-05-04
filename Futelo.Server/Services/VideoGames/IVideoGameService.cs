using Futelo.Shared.DTOs.VideoGame;

namespace Futelo.Server.Services.VideoGames;

public interface IVideoGameService
{
    Task<List<VideoGameResponse>> GetAllAsync();
}
