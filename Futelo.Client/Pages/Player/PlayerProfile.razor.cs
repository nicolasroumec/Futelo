using Futelo.Client.Services.Language;
using Futelo.Client.Services.Stats;
using Futelo.Client.Services.Vault;
using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.DTOs.Vault;
using Futelo.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Futelo.Client.Pages.Player;

public partial class PlayerProfile : IDisposable
{
    [Parameter] public int VaultId { get; set; }
    [Parameter] public string PlayerId { get; set; } = string.Empty;
    [Inject] private IStatsService StatsService { get; set; } = null!;
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private PlayerStatsResponse? stats;
    private List<EloHistoryPoint> eloHistory = [];
    private List<RecentFormEntry> recentForm = [];
    private List<VaultPlayerResponse> opponents = [];
    private string selectedOpponentId = string.Empty;
    private bool isLoading = true;
    private bool chartRendered = false;
    private bool countersAnimated = false;
    private string? errorMessage;

    private int _elo;
    private int _played, _won, _drawn, _lost, _gf, _ga;

    protected override async Task OnInitializedAsync()
    {
        Lang.OnChange += HandleLanguageChange;
        try
        {
            stats = await StatsService.GetPlayerStatsAsync(PlayerId, VaultId);
            eloHistory = await StatsService.GetEloHistoryAsync(VaultId, PlayerId);
            recentForm = await StatsService.GetRecentFormAsync(VaultId, PlayerId);
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

    private void HandleLanguageChange() => InvokeAsync(StateHasChanged);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!isLoading && stats != null && !countersAnimated)
        {
            countersAnimated = true;
            _ = AnimateCountersAsync();
        }

        if (!isLoading && !chartRendered && eloHistory.Count > 0)
        {
            chartRendered = true;
            var labels = eloHistory.Select(p => p.Date.ToString("dd/MM/yy")).ToArray();
            var data = eloHistory.Select(p => p.Elo).ToArray();
            await JS.InvokeVoidAsync("renderEloChart", "elo-chart", labels, data);
        }
    }

    private async Task AnimateCountersAsync()
    {
        const int steps = 30;
        const int delayMs = 16;

        for (int i = 1; i <= steps; i++)
        {
            var t = 1 - Math.Pow(1 - (double)i / steps, 3);
            _elo     = (int)(stats!.EloRating  * t);
            _played  = (int)(stats.Played      * t);
            _won     = (int)(stats.Won         * t);
            _drawn   = (int)(stats.Drawn       * t);
            _lost    = (int)(stats.Lost        * t);
            _gf      = (int)(stats.GoalsFor    * t);
            _ga      = (int)(stats.GoalsAgainst * t);
            await InvokeAsync(StateHasChanged);
            await Task.Delay(delayMs);
        }

        _elo    = stats!.EloRating;
        _played = stats.Played;
        _won    = stats.Won;
        _drawn  = stats.Drawn;
        _lost   = stats.Lost;
        _gf     = stats.GoalsFor;
        _ga     = stats.GoalsAgainst;
        await InvokeAsync(StateHasChanged);
    }

    private void NavigateToH2H()
    {
        if (!string.IsNullOrEmpty(selectedOpponentId))
            Navigation.NavigateTo($"/vaults/{VaultId}/h2h/{PlayerId}/{selectedOpponentId}");
    }

    private string StreakLabel()
    {
        if (stats == null || stats.CurrentStreak == 0) return "—";
        return stats.CurrentStreakType switch
        {
            StreakType.Win  => $"{stats.CurrentStreak}W",
            StreakType.Loss => $"{stats.CurrentStreak}L",
            StreakType.Draw => $"{stats.CurrentStreak}D",
            _ => "—"
        };
    }

    private string StreakBadgeClass()
    {
        if (stats == null || stats.CurrentStreak == 0) return "bg-secondary";
        return stats.CurrentStreakType switch
        {
            StreakType.Win  => "bg-success",
            StreakType.Loss => "bg-danger",
            StreakType.Draw => "bg-warning text-dark",
            _ => "bg-secondary"
        };
    }

    private static string FormBadgeClass(MatchResult result) => result switch
    {
        MatchResult.Win  => "bg-success",
        MatchResult.Loss => "bg-danger",
        _                => "bg-secondary"
    };

    private static string FormBadgeLabel(MatchResult result) => result switch
    {
        MatchResult.Win  => "W",
        MatchResult.Loss => "L",
        _                => "D"
    };

    public void Dispose() => Lang.OnChange -= HandleLanguageChange;
}
