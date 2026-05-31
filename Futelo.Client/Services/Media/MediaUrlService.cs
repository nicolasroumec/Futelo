namespace Futelo.Client.Services.Media;

/// <summary>
/// Resolves root-relative API media paths (e.g. "/api/users/x/avatar") to
/// absolute URLs against the API origin. The browser resolves an
/// &lt;img src&gt; against the page origin, not the HttpClient base address, so
/// when the client and API are served from different origins a relative path
/// would point at the static host (404). Resolving here keeps a single source
/// of the API base for image tags.
/// </summary>
public class MediaUrlService(string apiBase)
{
    private readonly string _base = apiBase.TrimEnd('/');

    /// <summary>
    /// Returns <paramref name="url"/> unchanged when it is null, empty, a data
    /// URL, or already absolute; prepends the API origin when it is a
    /// root-relative path starting with '/'.
    /// </summary>
    public string? Resolve(string? url)
    {
        if (string.IsNullOrEmpty(url)) return url;
        return url.StartsWith('/') ? _base + url : url;
    }
}
