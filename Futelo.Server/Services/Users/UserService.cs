using Futelo.Server.Repositories.Users;

namespace Futelo.Server.Services.Users;

public class UserService(IUserRepository repository) : IUserService
{
    public Task<byte[]?> GetAvatarAsync(string userId)
        => repository.GetAvatarAsync(userId);

    public Task SetAvatarAsync(string userId, byte[] data)
        => repository.SetAvatarAsync(userId, data);

    public Task DeleteAvatarAsync(string userId)
        => repository.DeleteAvatarAsync(userId);
}
