using Futelo.Client.Services.Stats;
using Futelo.Client.Services.SuperCup;
using Futelo.Client.Services.Teams;
using Futelo.Client.Services.Toast;
using Futelo.Client.Services.VideoGames;
using Futelo.Client.Shared;
using Futelo.Shared;
using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.DTOs.SuperCup;
using Futelo.Shared.DTOs.Team;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.SuperCup;

public partial class SuperCupView : LocalizedComponentBase
{
    [Parameter] public int Id { get; set; }
    [Inject] private ISuperCupService SuperCupService { get; set; } = null!;
    [Inject] private IStatsService StatsService { get; set; } = null!;
    [Inject] private ITeamService TeamService { get; set; } = null!;
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;
    [Inject] private IToastService Toast { get; set; } = null!;

    private SuperCupResponse? superCup;
    private HeadToHeadResponse? h2h;
    private bool isLoading = true;
    private bool isWorking;
    private bool isRecording;
    private string? errorMessage;

    private int? recordingMatchId;
    private string recordingHomeId = string.Empty;
    private string recordingAwayId = string.Empty;
    private bool recordingIsLeg2;
    private int? otherLegHomeScore;
    private int? otherLegAwayScore;
    private int? lastResultMatchId;
    private RecordSuperCupResultResponse? lastEloResult;

    private int? editingMatchId;
    private int? correctingMatchId;
    private bool isCorrecting;
    private bool correctingIsLeg2;
    private int? correctingOtherLegHomeScore;
    private int? correctingOtherLegAwayScore;
    private List<TeamResponse> teams = [];
    private List<VideoGameResponse> videoGames = [];

    private bool editingDates;
    private DateOnly? editStartDate;
    private DateOnly? editEndDate;
    private bool isSavingDates;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            superCup = await SuperCupService.GetByIdAsync(Id, ComponentToken);
            if (superCup.Player1Id != null && superCup.Player2Id != null)
            {
                h2h = await StatsService.GetHeadToHeadAsync(superCup.Player1Id, superCup.Player2Id, superCup.VaultId, ComponentToken);
            }
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

    private void SelectMatch(int matchId, string homeId, string homeName, string awayId, string awayName, int leg)
    {
        if (recordingMatchId == matchId)
        {
            recordingMatchId = null;
            return;
        }

        recordingMatchId = matchId;
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
    }

    private async Task HandleStart()
    {
        isWorking = true;
        try
        {
            await SuperCupService.StartAsync(Id);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isWorking = false;
        }
    }

    private async Task HandleRecordResult(MatchResultInput input)
    {
        if (recordingMatchId == null) return;
        isRecording = true;
        var matchId = recordingMatchId.Value;
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
            lastEloResult = await SuperCupService.RecordResultAsync(Id, matchId, request);
            lastResultMatchId = matchId;
            recordingMatchId = null;
            Toast.Show(Lang.Get("common.resultRecorded"), ToastType.Success);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isRecording = false;
        }
    }

    private void BeginEditDates()
    {
        editStartDate = superCup?.StartDate is { } s ? DateOnly.FromDateTime(s) : null;
        editEndDate = superCup?.EndDate is { } e ? DateOnly.FromDateTime(e) : null;
        editingDates = true;
    }

    private async Task HandlePatchDates()
    {
        isSavingDates = true;
        try
        {
            await SuperCupService.PatchDatesAsync(Id, new PatchDatesRequest
            {
                StartDate = editStartDate is { } s ? s.ToDateTime(TimeOnly.MinValue) : null,
                EndDate = editEndDate is { } e ? e.ToDateTime(TimeOnly.MinValue) : null,
            });
            editingDates = false;
            Toast.Show(Lang.Get("common.saved"), ToastType.Success);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isSavingDates = false;
        }
    }

    private async Task HandleMatchClick(int matchId)
    {
        if (!superCup!.CanEdit || superCup.Status == CompetitionStatus.NotStarted) return;
        var match = superCup.Matches.First(m => m.Id == matchId);
        if (match.Status == MatchStatus.Pending && superCup.Status == CompetitionStatus.Active) return;
        await ToggleEditMatch(matchId);
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
    }

    private void HandleRequestCorrection(int matchId)
    {
        correctingMatchId = matchId;
        editingMatchId = null;
        recordingMatchId = null;
        lastEloResult = null;

        var match = superCup!.Matches.First(m => m.Id == matchId);
        correctingIsLeg2 = superCup.IsHomeAndAway && match.Leg == 2;
        correctingOtherLegHomeScore = null;
        correctingOtherLegAwayScore = null;

        if (correctingIsLeg2 && superCup.Matches.Count >= 2)
        {
            var leg1 = superCup.Matches.OrderBy(m => m.Id).First();
            correctingOtherLegHomeScore = leg1.HomeScore;
            correctingOtherLegAwayScore = leg1.AwayScore;
        }
    }

    private async Task HandleCorrectResult(MatchResultInput input)
    {
        if (correctingMatchId == null) return;
        isCorrecting = true;
        var matchId = correctingMatchId.Value;
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
            lastEloResult = await SuperCupService.RecordResultAsync(Id, matchId, request);
            lastResultMatchId = matchId;
            correctingMatchId = null;
            Toast.Show(Lang.Get("common.resultRecorded"), ToastType.Success);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isCorrecting = false;
        }
    }

    private async Task HandlePatchMatch(PatchMatchRequest request)
    {
        if (editingMatchId == null) return;
        try
        {
            await SuperCupService.PatchMatchAsync(Id, editingMatchId.Value, request);
            editingMatchId = null;
            Toast.Show(Lang.Get("common.saved"), ToastType.Success);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
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

}
