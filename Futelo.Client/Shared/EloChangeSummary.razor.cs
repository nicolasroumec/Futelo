using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class EloChangeSummary : ComponentBase
{
    [Parameter] public string HomeName { get; set; } = string.Empty;
    [Parameter] public int HomeEloChange { get; set; }
    [Parameter] public int HomeEloBefore { get; set; }
    [Parameter] public int HomeEloAfter { get; set; }
    [Parameter] public int HomeRankBefore { get; set; }
    [Parameter] public int HomeRankAfter { get; set; }

    [Parameter] public string AwayName { get; set; } = string.Empty;
    [Parameter] public int AwayEloChange { get; set; }
    [Parameter] public int AwayEloBefore { get; set; }
    [Parameter] public int AwayEloAfter { get; set; }
    [Parameter] public int AwayRankBefore { get; set; }
    [Parameter] public int AwayRankAfter { get; set; }

    private static string ChangeClass(int change) =>
        change > 0 ? "text-success" : change < 0 ? "text-danger" : "text-muted";

    private static string ChangeLabel(int change) =>
        change > 0 ? $"+{change}" : change.ToString();

    private static string RankArrow(int before, int after) =>
        after < before ? "↑" : after > before ? "↓" : "–";

    private static string RankArrowClass(int before, int after) =>
        after < before ? "text-success" : after > before ? "text-danger" : "text-muted";
}
