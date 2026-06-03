namespace Futelo.Server.Services.Users;

public interface IUserService
{
    Task<byte[]?> GetAvatarAsync(string userId);
    Task SetAvatarAsync(string userId, byte[] data);
    Task DeleteAvatarAsync(string userId);
    Task<List<string>> GetUserIdsWithAvatarAsync();
}
