namespace Futelo.Client.Services.Users;

public interface IUserService
{
    Task UploadAvatarAsync(byte[] data);
    Task DeleteAvatarAsync();
    Task<List<string>> GetUsersWithAvatarAsync(CancellationToken ct = default);
}
