using Futelo.Client.Services.Stats;
using Futelo.Client.Services.Vault;
using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.DTOs.Vault;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Player;

public partial class PlayerProfile
{
    [Parameter] public int VaultId { get; set; }
    [Parameter] public string PlayerId { get; set; } = string.Empty;
    [Inject] private IStatsService StatsService { get; set; } = null!;
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private PlayerStatsResponse? stats;
    private List<VaultPlayerResponse> opponents = [];
    private string selectedOpponentId = string.Empty;
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            stats = await StatsService.GetPlayerStatsAsync(PlayerId, VaultId);
            var vault = await VaultService.GetByIdAsync(VaultId);
            opponents = vault.Players.Where(p => p.PlayerId != PlayerId).ToList();
            if (opponents.Count > 0)
                selectedOpponentId = opponents[0].PlayerId;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }

    private void NavigateToH2H()
    {
        if (!string.IsNullOrEmpty(selectedOpponentId))
            Navigation.NavigateTo($"/vaults/{VaultId}/h2h/{PlayerId}/{selectedOpponentId}");
    }
}
