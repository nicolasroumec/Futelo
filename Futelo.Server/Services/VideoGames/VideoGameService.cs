using Futelo.Server.Models;
using Futelo.Server.Repositories.VideoGames;
using Futelo.Shared.DTOs.VideoGame;

namespace Futelo.Server.Services.VideoGames;

using static ErrorMessages;

public class VideoGameService(IVideoGameRepository repository, ILogger<VideoGameService> logger) : IVideoGameService
{
    public async Task<List<VideoGameResponse>> GetAllAsync()
    {
        var games = await repository.GetAllAsync();
        return games.Select(g => new VideoGameResponse { Id = g.Id, Name = g.Name }).ToList();
    }

    public async Task<VideoGameResponse> CreateAsync(CreateVideoGameRequest request)
    {
        var game = new VideoGame { Name = request.Name };
        await repository.CreateAsync(game);
        return new VideoGameResponse { Id = game.Id, Name = game.Name };
    }

    public async Task UpdateAsync(int id, CreateVideoGameRequest request)
    {
        var game = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"VideoGame {id} not found.");
        game.Name = request.Name;
        await repository.UpdateAsync(game);
    }

    public async Task DeleteAsync(int id)
    {
        var game = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"VideoGame {id} not found.");
        await repository.DeleteAsync(game);
    }
}
