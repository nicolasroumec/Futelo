using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.VideoGames;

public class VideoGameRepository(FuteloContext context) : BaseRepository<VideoGame>(context), IVideoGameRepository
{
    public async Task<IEnumerable<VideoGame>> GetAllAsync()
        => await FindAll().OrderBy(g => g.Name).ToListAsync();

    public async Task<VideoGame?> GetByIdAsync(int id)
        => await FindByCondition(g => g.Id == id).FirstOrDefaultAsync();

    public async Task<VideoGame> CreateAsync(VideoGame game)
    {
        Create(game);
        await SaveChangesAsync();
        return game;
    }

    public async Task UpdateAsync(VideoGame game)
    {
        Update(game);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(VideoGame game)
    {
        Delete(game);
        await SaveChangesAsync();
    }
}
