using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.VideoGames;

public class VideoGameRepository(FuteloContext context) : BaseRepository<VideoGame>(context), IVideoGameRepository
{
    public async Task<IEnumerable<VideoGame>> GetAllAsync()
        => await FindAll().OrderBy(g => g.Name).ToListAsync();
}
