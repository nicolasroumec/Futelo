using Futelo.Client.Services.Cup;
using Futelo.Client.Services.Language;
using Futelo.Client.Services.Teams;
using Futelo.Client.Services.VideoGames;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Cup;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.DTOs.Team;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Cup;

public partial class CupView : IDisposable
{
    [Parameter] public int Id { get; set; }
    [Inject] private ICupService CupService { get; set; } = null!;
    [Inject] private ITeamService TeamService { get; set; } = null!;
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;

    private CupResponse? cup;
    private bool isLoading = true;
    private bool isRecording;
    private string? errorMessage;

    private int? recordingMatchId;
    private string recordingHomeId = string.Empty;
    private string recordingAwayId = string.Empty;
    private bool recordingIsLeg2;
    private int? otherLegHomeScore;
    private int? otherLegAwayScore;
    private RecordCupResultResponse? lastResult;

    private int? editingMatchId;
    private List<TeamResponse> teams = [];
    private List<VideoGameResponse> videoGames = [];

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
            return;
        }

        recordingMatchId = matchId;
        lastResult = null;
        recordingHomeId = homeId;
        recordingAwayId = awayId;

        var match = cup!.Rounds.SelectMany(r => r.Matches).First(m => m.Id == matchId);
        recordingIsLeg2 = cup.IsHomeAndAway && match.Leg == 2;
        otherLegHomeScore = null;
        otherLegAwayScore = null;

        if (recordingIsLeg2)
        {
            var round = cup.Rounds.First(r => r.Matches.Any(m => m.Id == matchId));
            var ordered = round.Matches.OrderBy(m => m.Id).ToList();
            int idx = ordered.FindIndex(m => m.Id == matchId);
            var leg1 = ordered[idx - 1];
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
            await CupService.StartAsync(Id);
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
            var request = new RecordCupResultRequest
            {
                HomeScore = input.HomeScore,
                AwayScore = input.AwayScore,
                WonOnPenaltiesId = input.WonOnPenaltiesId,
                HomePenaltyScore = input.HomePenaltyScore,
                AwayPenaltyScore = input.AwayPenaltyScore
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
            isRecording = false;
        }
    }

    private async Task ToggleEditMatch(int matchId)
    {
        if (editingMatchId == matchId)
        {
            editingMatchId = null;
            return;
        }

        if (teams.Count == 0 || videoGames.Count == 0)
        {
            try
            {
                var t = TeamService.GetAllAsync();
                var vg = VideoGameService.GetAllAsync();
                await Task.WhenAll(t, vg);
                teams = t.Result;
                videoGames = vg.Result;
            }
            catch { }
        }

        editingMatchId = matchId;
        recordingMatchId = null;
        errorMessage = null;
    }

    private async Task HandlePatchMatch(PatchMatchRequest request)
    {
        if (editingMatchId == null) return;
        errorMessage = null;
        try
        {
            await CupService.PatchMatchAsync(Id, editingMatchId.Value, request);
            editingMatchId = null;
            await LoadAsync();
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
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
