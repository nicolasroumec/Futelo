using Futelo.Client.Services.Cup;
using Futelo.Client.Services.Language;
using Futelo.Shared.DTOs.Cup;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Cup;

public partial class CupView : IDisposable
{
    [Parameter] public int Id { get; set; }
    [Inject] private ICupService CupService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;

    private CupResponse? cup;
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
    private bool recordingIsLeg2HomeAndAway;
    private int? otherLegHomeScore;
    private int? otherLegAwayScore;
    private RecordCupResultResponse? lastResult;

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
            cup = await CupService.GetByIdAsync(Id);
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

    private void SelectMatch(int matchId, string homeId, string awayId)
    {
        if (recordingMatchId == matchId)
        {
            recordingMatchId = null;
        }
        else
        {
            recordingMatchId = matchId;
            homeScore = 0;
            awayScore = 0;
            wonOnPenaltiesId = string.Empty;
            homePenaltyScore = null;
            awayPenaltyScore = null;
            lastResult = null;

            var match = cup!.Rounds
                .SelectMany(r => r.Matches)
                .First(m => m.Id == matchId);
            recordingHomeId = homeId;
            recordingHomeName = match.HomePlayerName;
            recordingAwayId = awayId;
            recordingAwayName = match.AwayPlayerName;

            recordingIsLeg2HomeAndAway = cup.IsHomeAndAway && match.Leg == 2;
            otherLegHomeScore = null;
            otherLegAwayScore = null;
            if (recordingIsLeg2HomeAndAway)
            {
                var round = cup.Rounds.First(r => r.Matches.Any(m => m.Id == matchId));
                var ordered = round.Matches.OrderBy(m => m.Id).ToList();
                int idx = ordered.FindIndex(m => m.Id == matchId);
                var leg1 = ordered[idx - 1];
                otherLegHomeScore = leg1.HomeScore;
                otherLegAwayScore = leg1.AwayScore;
            }
        }
        errorMessage = null;
    }

    private bool ShowPenaltyFields
    {
        get
        {
            if (!cup!.IsHomeAndAway)
                return homeScore == awayScore;

            if (!recordingIsLeg2HomeAndAway) return false;
            if (otherLegHomeScore == null) return false;
            int aGoals = otherLegHomeScore.Value + awayScore;
            int bGoals = (otherLegAwayScore ?? 0) + homeScore;
            return aGoals == bGoals;
        }
    }

    private async Task HandleStart()
    {
        isWorking = true;
        errorMessage = null;
        try
        {
            await CupService.StartAsync(Id);
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
            var request = new RecordCupResultRequest
            {
                HomeScore = homeScore,
                AwayScore = awayScore,
                WonOnPenaltiesId = hasPenalties ? wonOnPenaltiesId : null,
                HomePenaltyScore = hasPenalties ? homePenaltyScore : null,
                AwayPenaltyScore = hasPenalties ? awayPenaltyScore : null
            };
            lastResult = await CupService.RecordResultAsync(Id, recordingMatchId.Value, request);
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

    private List<List<CupMatchResponse>> GetTies(CupRoundResponse round)
    {
        var ordered = round.Matches.OrderBy(m => m.Id).ToList();
        var ties = new List<List<CupMatchResponse>>();

        if (!cup!.IsHomeAndAway)
        {
            ties.AddRange(ordered.Select(m => new List<CupMatchResponse> { m }));
        }
        else
        {
            for (int i = 0; i < ordered.Count; i += 2)
            {
                var tie = new List<CupMatchResponse> { ordered[i] };
                if (i + 1 < ordered.Count)
                    tie.Add(ordered[i + 1]);
                ties.Add(tie);
            }
        }

        return ties;
    }

    private static (string homeName, int homeGoals, int awayGoals, string awayName) ComputeAggregate(
        List<CupMatchResponse> tie)
    {
        var leg1 = tie.First(m => m.Leg == 1);
        var leg2 = tie.First(m => m.Leg == 2);
        int homeGoals = (leg1.HomeScore ?? 0) + (leg2.AwayScore ?? 0);
        int awayGoals = (leg1.AwayScore ?? 0) + (leg2.HomeScore ?? 0);
        return (leg1.HomePlayerName, homeGoals, awayGoals, leg1.AwayPlayerName);
    }

    private static string FormatEloChange(CupEloChangeResult p)
    {
        string arrow = p.RankAfter < p.RankBefore ? "↑" : p.RankAfter > p.RankBefore ? "↓" : "→";
        string sign = p.EloChange >= 0 ? "+" : "";
        return $"{p.DisplayName}   {p.EloBefore} → {p.EloAfter} ({sign}{p.EloChange})  {arrow} #{p.RankAfter}";
    }

    public void Dispose() => Lang.OnChange -= HandleLanguageChange;
}
