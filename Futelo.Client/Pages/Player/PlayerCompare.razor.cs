using Futelo.Client.Services.Stats;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Player;

public partial class PlayerCompare : LocalizedComponentBase
{
    [Parameter] public int VaultId { get; set; }
    [Parameter] public string Player1Id { get; set; } = string.Empty;
    [Parameter] public string Player2Id { get; set; } = string.Empty;
    [Inject] private IStatsService StatsService { get; set; } = null!;

    private PlayerStatsResponse? p1;
    private PlayerStatsResponse? p2;
    private HeadToHeadResponse? h2h;
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var t1 = StatsService.GetPlayerStatsAsync(Player1Id, VaultId, ComponentToken);
            var t2 = StatsService.GetPlayerStatsAsync(Player2Id, VaultId, ComponentToken);
            var th = StatsService.GetHeadToHeadAsync(Player1Id, Player2Id, VaultId, ComponentToken);
            await Task.WhenAll(t1, t2, th);
            p1 = t1.Result;
            p2 = t2.Result;
            h2h = th.Result;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }

    private static int WinRate(PlayerStatsResponse p) =>
        p.Played > 0 ? p.Won * 100 / p.Played : 0;

    private static string WinnerClass(int myVal, int otherVal, bool lowerIsBetter = false)
    {
        if (myVal == otherVal) return "";
        bool isBetter = lowerIsBetter ? myVal < otherVal : myVal > otherVal;
        return isBetter ? "compare-winner" : "compare-loser";
    }

    private static string StreakLabel(PlayerStatsResponse p) =>
        p.CurrentStreak == 0 ? "—" : p.CurrentStreakType switch
        {
            StreakType.Win  => $"{p.CurrentStreak}W",
            StreakType.Loss => $"{p.CurrentStreak}L",
            StreakType.Draw => $"{p.CurrentStreak}D",
            _               => "—"
        };

    private static string StreakBadgeClass(PlayerStatsResponse p) =>
        p.CurrentStreak == 0 ? "bg-secondary" : p.CurrentStreakType switch
        {
            StreakType.Win  => "bg-success",
            StreakType.Loss => "bg-danger",
            StreakType.Draw => "bg-warning text-dark",
            _               => "bg-secondary"
        };

    private static string SignedInt(int n) => n > 0 ? $"+{n}" : n.ToString();

    private static int H2DP1Width(HeadToHeadResponse h)
    {
        int decisive = h.Player1Wins + h.Player2Wins;
        return decisive == 0 ? 50 : h.Player1Wins * 100 / decisive;
    }
}
