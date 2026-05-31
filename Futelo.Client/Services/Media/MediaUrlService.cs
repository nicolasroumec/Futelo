namespace Futelo.Client.Services.Media;

/// <summary>
/// Resolves root-relative API media paths (e.g. "/api/users/x/avatar") to
/// absolute URLs against the API origin. The browser resolves an
/// &lt;img src&gt; against the page origin, not the HttpClient base address, so
/// when the client and API are served from different origins a relative path
/// would point at the static host (404). Resolving here keeps a single source
/// of the API base for image tags.
///
/// It also carries a global cache-busting token: when an image is replaced or
/// removed, <see cref="Bump"/> changes the token so every media URL produced
/// afterwards is distinct, forcing the browser to fetch the new image instead
/// of reusing a cached copy of the same URL.
/// </summary>
public class MediaUrlService(string apiBase)
{
    private readonly string _base = apiBase.TrimEnd('/');
    private string? _bust;

    /// <summary>
    /// Returns <paramref name="url"/> unchanged when it is null, empty, a data
    /// URL, or already absolute; otherwise prepends the API origin and appends
    /// the current cache-busting token (if any) to a root-relative path.
    /// </summary>
    public string? Resolve(string? url)
    {
        if (string.IsNullOrEmpty(url)) return url;
        if (!url.StartsWith('/')) return url;

        var abs = _base + url;
        if (string.IsNullOrEmpty(_bust)) return abs;
        return abs + (abs.Contains('?') ? '&' : '?') + "v=" + _bust;
    }

    /// <summary>
    /// Advances the cache-busting token. Call after an avatar/shield upload or
    /// delete so subsequently rendered media URLs bypass the browser cache.
    /// </summary>
    public void Bump() => _bust = DateTimeOffset.UtcNow.Ticks.ToString();
}
