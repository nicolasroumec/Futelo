using Futelo.Server.Repositories.VideoGames;
using Futelo.Shared.DTOs.VideoGame;

namespace Futelo.Server.Services.VideoGames;

public class VideoGameService(IVideoGameRepository repository) : IVideoGameService
{
    public async Task<List<VideoGameResponse>> GetAllAsync()
    {
        var games = await repository.GetAllAsync();
        return games.Select(g => new VideoGameResponse { Id = g.Id, Name = g.Name }).ToList();
    }
}
