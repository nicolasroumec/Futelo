using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Vault;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class SeasonPlayerManager : LocalizedComponentBase
{
    [Parameter] public List<VaultPlayerResponse> VaultPlayers { get; set; } = [];
    [Parameter] public HashSet<string> SelectedPlayerIds { get; set; } = [];

    private void Toggle(string playerId, bool selected)
    {
        if (selected) SelectedPlayerIds.Add(playerId);
        else SelectedPlayerIds.Remove(playerId);
    }
}
