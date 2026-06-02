namespace Futelo.Server.Repositories.Users;

public interface IUserRepository
{
    Task<byte[]?> GetAvatarAsync(string userId);
    Task SetAvatarAsync(string userId, byte[] data);
    Task DeleteAvatarAsync(string userId);
    Task<List<string>> GetUserIdsWithAvatarAsync();
}
