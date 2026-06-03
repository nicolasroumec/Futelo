namespace Futelo.Client.Services.Users;

/// <summary>
/// Caches the set of user IDs that have an uploaded avatar so the UI can skip
/// the <c>/api/users/{id}/avatar</c> request (and its 404) for users without
/// one. Until the directory has loaded it answers optimistically (returns the
/// URL), so behaviour is never worse than before it is ready.
/// </summary>
public class AvatarDirectory(IUserService userService)
{
    private HashSet<string>? _ids;
    private Task? _loading;

    /// <summary>Loads the directory once; concurrent callers share the task.</summary>
    public Task EnsureLoadedAsync() => _loading ??= LoadAsync();

    private async Task LoadAsync()
    {
        try
        {
            var ids = await userService.GetUsersWithAvatarAsync();
            _ids = [.. ids];
        }
        catch
        {
            // Stay in optimistic mode and allow a later retry.
            _loading = null;
        }
    }

    /// <summary>
    /// The avatar URL for a user, or <c>null</c> when the user is known to have
    /// no avatar (so the caller shows initials without hitting the API).
    /// Returns the URL optimistically until the directory has loaded.
    /// </summary>
    public string? UrlFor(string? userId)
    {
        if (string.IsNullOrEmpty(userId)) return null;
        if (_ids is null) return $"/api/users/{userId}/avatar";
        return _ids.Contains(userId) ? $"/api/users/{userId}/avatar" : null;
    }

    /// <summary>Keeps the directory current after an upload/delete without a refetch.</summary>
    public void SetHasAvatar(string userId, bool hasAvatar)
    {
        if (_ids is null) return;
        if (hasAvatar) _ids.Add(userId);
        else _ids.Remove(userId);
    }
}
