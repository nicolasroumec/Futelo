using Futelo.Server.Models;
using Futelo.Server.Repositories.VideoGames;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.Extensions.Caching.Memory;

namespace Futelo.Server.Services.VideoGames;

using static ErrorMessages;

public class VideoGameService(IVideoGameRepository repository, IMemoryCache cache) : IVideoGameService
{
    private const string CacheKey = "videogames_all";

    public async Task<List<VideoGameResponse>> GetAllAsync()
    {
        if (cache.TryGetValue(CacheKey, out List<VideoGameResponse>? cached))
            return cached!;

        var games = await repository.GetAllAsync();
        var result = games.Select(g => new VideoGameResponse { Id = g.Id, Name = g.Name }).ToList();
        cache.Set(CacheKey, result, TimeSpan.FromMinutes(30));
        return result;
    }

    public async Task<VideoGameResponse> CreateAsync(CreateVideoGameRequest request)
    {
        var game = new VideoGame { Name = request.Name };
        await repository.CreateAsync(game);
        cache.Remove(CacheKey);
        return new VideoGameResponse { Id = game.Id, Name = game.Name };
    }

    public async Task UpdateAsync(int id, CreateVideoGameRequest request)
    {
        var game = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"VideoGame {id} not found.");
        game.Name = request.Name;
        await repository.UpdateAsync(game);
        cache.Remove(CacheKey);
    }

    public async Task DeleteAsync(int id)
    {
        var game = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"VideoGame {id} not found.");
        await repository.DeleteAsync(game);
        cache.Remove(CacheKey);
    }
}
