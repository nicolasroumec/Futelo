using Futelo.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Users;

public class UserRepository(FuteloContext context) : IUserRepository
{
    public Task<byte[]?> GetAvatarAsync(string userId)
        => context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Avatar)
            .FirstOrDefaultAsync();

    public async Task SetAvatarAsync(string userId, byte[] data)
        => await context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.Avatar, data));

    public async Task DeleteAvatarAsync(string userId)
        => await context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.Avatar, (byte[]?)null));
}
