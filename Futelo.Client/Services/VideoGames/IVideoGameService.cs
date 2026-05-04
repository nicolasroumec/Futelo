using Futelo.Shared.DTOs.VideoGame;

namespace Futelo.Client.Services.VideoGames;

public interface IVideoGameService
{
    Task<List<VideoGameResponse>> GetAllAsync();
}
