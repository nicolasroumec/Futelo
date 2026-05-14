using Futelo.Client.Services.Language;
using Futelo.Client.Services.SuperCup;
using Futelo.Client.Shared;
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
    private bool isRecording;
    private string? errorMessage;

    private int? recordingMatchId;
    private string recordingHomeId = string.Empty;
    private string recordingAwayId = string.Empty;
    private bool recordingIsLeg2;
    private int? otherLegHomeScore;
    private int? otherLegAwayScore;
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
        lastResult = null;
        recordingHomeId = homeId;
        recordingAwayId = awayId;
        recordingIsLeg2 = superCup!.IsHomeAndAway && leg == 2;
        otherLegHomeScore = null;
        otherLegAwayScore = null;
        if (recordingIsLeg2 && superCup.Matches.Count >= 2)
        {
            var leg1 = superCup.Matches.OrderBy(m => m.Id).First();
            otherLegHomeScore = leg1.HomeScore;
            otherLegAwayScore = leg1.AwayScore;
        }
        errorMessage = null;
    }

    private async Task HandleStart()
    {
        isLoading = true;
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
            isLoading = false;
        }
    }

    private async Task HandleRecordResult(MatchResultInput input)
    {
        if (recordingMatchId == null) return;
        isRecording = true;
        errorMessage = null;
        try
        {
            var request = new RecordSuperCupResultRequest
            {
                HomeScore = input.HomeScore,
                AwayScore = input.AwayScore,
                WonOnPenaltiesId = input.WonOnPenaltiesId,
                HomePenaltyScore = input.HomePenaltyScore,
                AwayPenaltyScore = input.AwayPenaltyScore
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
            isRecording = false;
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
