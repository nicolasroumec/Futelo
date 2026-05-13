using Futelo.Client.Services.Language;
using Futelo.Client.Services.SuperCup;
using Futelo.Shared.DTOs.SuperCup;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.SuperCup;

public partial class SuperCupView : IDisposable
{
    [Parameter] public int Id { get; set; }
    [Inject] private ISuperCupService SuperCupService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;

    private SuperCupResponse? superCup;
    private bool isLoading = true;
    private bool isWorking;
    private string? errorMessage;

    private int? recordingMatchId;
    private int homeScore;
    private int awayScore;
    private string wonOnPenaltiesId = string.Empty;
    private int? homePenaltyScore;
    private int? awayPenaltyScore;
    private string recordingHomeId = string.Empty;
    private string recordingHomeName = string.Empty;
    private string recordingAwayId = string.Empty;
    private string recordingAwayName = string.Empty;
    private bool recordingIsLeg2;
    private RecordSuperCupResultResponse? lastResult;

    protected override async Task OnInitializedAsync()
    {
        Lang.OnChange += HandleLanguageChange;
        await LoadAsync();
    }

    private void HandleLanguageChange() => InvokeAsync(StateHasChanged);

    private async Task LoadAsync()
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            superCup = await SuperCupService.GetByIdAsync(Id);
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

    private void SelectMatch(int matchId, string homeId, string homeName, string awayId, string awayName, int leg)
    {
        if (recordingMatchId == matchId)
        {
            recordingMatchId = null;
            return;
        }

        recordingMatchId = matchId;
        homeScore = 0;
        awayScore = 0;
        wonOnPenaltiesId = string.Empty;
        homePenaltyScore = null;
        awayPenaltyScore = null;
        lastResult = null;
        recordingHomeId = homeId;
        recordingHomeName = homeName;
        recordingAwayId = awayId;
        recordingAwayName = awayName;
        recordingIsLeg2 = superCup!.IsHomeAndAway && leg == 2;
        errorMessage = null;
    }

    private bool ShowPenaltyFields
    {
        get
        {
            if (!superCup!.IsHomeAndAway)
                return homeScore == awayScore;

            if (!recordingIsLeg2) return false;

            var ordered = superCup.Matches.OrderBy(m => m.Id).ToList();
            if (ordered.Count < 2) return false;
            var leg1 = ordered[0];
            if (leg1.HomeScore == null) return false;

            int p1Goals = (leg1.HomeScore ?? 0) + awayScore;
            int p2Goals = (leg1.AwayScore ?? 0) + homeScore;
            return p1Goals == p2Goals;
        }
    }

    private async Task HandleStart()
    {
        isWorking = true;
        errorMessage = null;
        try
        {
            await SuperCupService.StartAsync(Id);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isWorking = false;
        }
    }

    private async Task HandleRecordResult()
    {
        if (recordingMatchId == null) return;
        isWorking = true;
        errorMessage = null;
        try
        {
            bool hasPenalties = ShowPenaltyFields && !string.IsNullOrEmpty(wonOnPenaltiesId);
            var request = new RecordSuperCupResultRequest
            {
                HomeScore = homeScore,
                AwayScore = awayScore,
                WonOnPenaltiesId = hasPenalties ? wonOnPenaltiesId : null,
                HomePenaltyScore = hasPenalties ? homePenaltyScore : null,
                AwayPenaltyScore = hasPenalties ? awayPenaltyScore : null
            };
            lastResult = await SuperCupService.RecordResultAsync(Id, recordingMatchId.Value, request);
            recordingMatchId = null;
            await LoadAsync();
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isWorking = false;
        }
    }

    private (int p1Goals, int p2Goals) ComputeAggregate()
    {
        var ordered = superCup!.Matches.OrderBy(m => m.Id).ToList();
        var leg1 = ordered[0];
        var leg2 = ordered[1];
        int p1Goals = (leg1.HomeScore ?? 0) + (leg2.AwayScore ?? 0);
        int p2Goals = (leg1.AwayScore ?? 0) + (leg2.HomeScore ?? 0);
        return (p1Goals, p2Goals);
    }

    private static string FormatEloChange(SuperCupEloChangeResult p)
    {
        string arrow = p.RankAfter < p.RankBefore ? "↑" : p.RankAfter > p.RankBefore ? "↓" : "→";
        string sign = p.EloChange >= 0 ? "+" : "";
        return $"{p.DisplayName}   {p.EloBefore} → {p.EloAfter} ({sign}{p.EloChange})  {arrow} #{p.RankAfter}";
    }

    public void Dispose() => Lang.OnChange -= HandleLanguageChange;
}
