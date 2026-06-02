namespace Futelo.Client.Services.Teams;

/// <summary>
/// Caches the set of team IDs that have an uploaded shield so the UI can skip
/// the <c>/api/teams/{id}/shield</c> request (and its 404) for teams without
/// one. Until the directory has loaded it answers optimistically (returns the
/// URL), so behaviour is never worse than before it is ready.
/// </summary>
public class ShieldDirectory(ITeamService teamService)
{
    private HashSet<int>? _ids;
    private Task? _loading;

    /// <summary>Loads the directory once; concurrent callers share the task.</summary>
    public Task EnsureLoadedAsync() => _loading ??= LoadAsync();

    private async Task LoadAsync()
    {
        try
        {
            var ids = await teamService.GetTeamsWithShieldAsync();
            _ids = [.. ids];
        }
        catch
        {
            // Stay in optimistic mode and allow a later retry.
            _loading = null;
        }
    }

    /// <summary>
    /// The shield URL for a team, or <c>null</c> when the team is known to have
    /// no shield (so the caller shows the abbreviation without hitting the API).
    /// Returns the URL optimistically until the directory has loaded.
    /// </summary>
    public string? UrlFor(int? teamId)
    {
        if (teamId is null) return null;
        if (_ids is null) return $"/api/teams/{teamId}/shield";
        return _ids.Contains(teamId.Value) ? $"/api/teams/{teamId}/shield" : null;
    }

    /// <summary>Keeps the directory current after an upload/delete without a refetch.</summary>
    public void SetHasShield(int teamId, bool hasShield)
    {
        if (_ids is null) return;
        if (hasShield) _ids.Add(teamId);
        else _ids.Remove(teamId);
    }
}
