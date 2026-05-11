using Futelo.Client.Services.Stats;
using Futelo.Client.Services.Vault;
using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.DTOs.Vault;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Futelo.Client.Pages.Player;

public partial class PlayerProfile
{
    [Parameter] public int VaultId { get; set; }
    [Parameter] public string PlayerId { get; set; } = string.Empty;
    [Inject] private IStatsService StatsService { get; set; } = null!;
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private PlayerStatsResponse? stats;
    private List<EloHistoryPoint> eloHistory = [];
    private List<VaultPlayerResponse> opponents = [];
    private string selectedOpponentId = string.Empty;
    private bool isLoading = true;
    private bool chartRendered = false;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            stats = await StatsService.GetPlayerStatsAsync(PlayerId, VaultId);
            eloHistory = await StatsService.GetEloHistoryAsync(VaultId, PlayerId);
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!isLoading && !chartRendered && eloHistory.Count > 0)
        {
            chartRendered = true;
            var labels = eloHistory.Select(p => p.Date.ToString("dd/MM/yy")).ToArray();
            var data = eloHistory.Select(p => p.Elo).ToArray();
            await JS.InvokeVoidAsync("renderEloChart", "elo-chart", labels, data);
        }
    }

    private void NavigateToH2H()
    {
        if (!string.IsNullOrEmpty(selectedOpponentId))
            Navigation.NavigateTo($"/vaults/{VaultId}/h2h/{PlayerId}/{selectedOpponentId}");
    }

    private string StreakLabel()
    {
        if (stats == null || stats.CurrentStreak == 0) return "—";
        return stats.CurrentStreak > 0
            ? $"{stats.CurrentStreak}W"
            : $"{Math.Abs(stats.CurrentStreak)}L";
    }

    private string StreakBadgeClass()
    {
        if (stats == null || stats.CurrentStreak == 0) return "bg-secondary";
        return stats.CurrentStreak > 0 ? "bg-success" : "bg-danger";
    }
}
