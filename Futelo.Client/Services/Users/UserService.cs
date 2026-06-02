using Futelo.Client.Services;

namespace Futelo.Client.Services.Users;

public class UserService(HttpClient http) : ApiService(http), IUserService
{
    public async Task UploadAvatarAsync(byte[] data)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(data), "file", "avatar.webp");
        var res = await Http.PutAsync("api/users/me/avatar", content);
        await res.EnsureSuccessAsync();
    }

    public Task DeleteAvatarAsync() => DeleteAsync("api/users/me/avatar");

    public Task<List<string>> GetUsersWithAvatarAsync(CancellationToken ct = default)
        => GetListAsync<string>("api/users/with-avatars", ct);
}
