using Futelo.Client.Services.Cup;
using Futelo.Client.Services.Teams;
using Futelo.Client.Services.Toast;
using Futelo.Client.Services.VideoGames;
using Futelo.Client.Shared;
using Futelo.Shared;
using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.Cup;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.DTOs.Team;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Cup;

public partial class CupView : LocalizedComponentBase
{
    [Parameter] public int Id { get; set; }
    [Inject] private ICupService CupService { get; set; } = null!;
    [Inject] private ITeamService TeamService { get; set; } = null!;
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;
    [Inject] private IToastService Toast { get; set; } = null!;

    private CupResponse? cup;
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
    private RecordCupResultResponse? lastEloResult;

    private int? editingMatchId;
    private List<TeamResponse> teams = [];
    private List<VideoGameResponse> videoGames = [];

    private bool editingDates;
    private DateOnly? editStartDate;
    private DateOnly? editEndDate;
    private bool isSavingDates;

    private bool addingRound;
    private string newRoundName = string.Empty;
    private int newRoundNumber = 1;
    private bool isAddingRound;

    private int? addingMatchToRoundId;
    private string newMatchHomePlayerId = string.Empty;
    private string newMatchAwayPlayerId = string.Empty;
    private int newMatchLeg = 1;
    private bool isAddingMatch;

    private string SeedingModeKey => cup!.SeedingMode switch
    {
        Futelo.Shared.Enums.CupSeedingMode.LeaguePosition => "cup.seedingMode.leaguePosition",
        Futelo.Shared.Enums.CupSeedingMode.Random => "cup.seedingMode.random",
        _ => "cup.seedingMode.seasonElo"
    };

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
            cup = await CupService.GetByIdAsync(Id, ComponentToken);
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

    private void SelectMatch(int matchId, string homeId, string awayId)
    {
        if (recordingMatchId == matchId)
        {
            recordingMatchId = null;
            return;
        }

        recordingMatchId = matchId;
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
    }

    private async Task HandleStart()
    {
        isWorking = true;
        try
        {
            await CupService.StartAsync(Id);
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

    private async Task HandleStartManual()
    {
        isWorking = true;
        try
        {
            await CupService.StartManualAsync(Id);
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

    private void BeginAddRound()
    {
        var maxRound = cup?.Rounds.Count > 0 ? cup.Rounds.Max(r => r.RoundNumber) : 0;
        newRoundNumber = maxRound + 1;
        newRoundName = string.Empty;
        addingRound = true;
    }

    private async Task HandleAddRound()
    {
        if (string.IsNullOrWhiteSpace(newRoundName)) return;
        isAddingRound = true;
        try
        {
            await CupService.AddRoundAsync(Id, new AddCupRoundRequest
            {
                Name = newRoundName,
                RoundNumber = newRoundNumber
            });
            addingRound = false;
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isAddingRound = false;
        }
    }

    private void BeginAddMatch(int roundId)
    {
        addingMatchToRoundId = roundId;
        newMatchHomePlayerId = string.Empty;
        newMatchAwayPlayerId = string.Empty;
        newMatchLeg = 1;
    }

    private async Task HandleAddMatch()
    {
        if (addingMatchToRoundId == null) return;
        if (string.IsNullOrEmpty(newMatchHomePlayerId) || string.IsNullOrEmpty(newMatchAwayPlayerId)) return;
        isAddingMatch = true;
        try
        {
            await CupService.AddMatchAsync(Id, addingMatchToRoundId.Value, new AddCupMatchRequest
            {
                HomePlayerId = newMatchHomePlayerId,
                AwayPlayerId = newMatchAwayPlayerId,
                Leg = newMatchLeg
            });
            addingMatchToRoundId = null;
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isAddingMatch = false;
        }
    }

    private async Task HandleRecordResult(MatchResultInput input)
    {
        if (recordingMatchId == null) return;
        isRecording = true;
        var matchId = recordingMatchId.Value;
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
            lastEloResult = await CupService.RecordResultAsync(Id, matchId, request);
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

    private async Task HandleMatchClick(int matchId)
    {
        if (!cup!.CanEdit || cup.Status == CompetitionStatus.NotStarted) return;
        var match = cup.Rounds.SelectMany(r => r.Matches).First(m => m.Id == matchId);
        if (match.Status == MatchStatus.Pending && cup.Status == CompetitionStatus.Active
            && !string.IsNullOrEmpty(match.HomePlayerId) && !string.IsNullOrEmpty(match.AwayPlayerId))
            SelectMatch(matchId, match.HomePlayerId, match.AwayPlayerId);
        else
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

    private async Task HandlePatchMatch(PatchMatchRequest request)
    {
        if (editingMatchId == null) return;
        try
        {
            await CupService.PatchMatchAsync(Id, editingMatchId.Value, request);
            editingMatchId = null;
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
    }

    private void BeginEditDates()
    {
        editStartDate = cup?.StartDate is { } s ? DateOnly.FromDateTime(s) : null;
        editEndDate = cup?.EndDate is { } e ? DateOnly.FromDateTime(e) : null;
        editingDates = true;
    }

    private async Task HandlePatchDates()
    {
        isSavingDates = true;
        try
        {
            await CupService.PatchDatesAsync(Id, new PatchDatesRequest
            {
                StartDate = editStartDate is { } s ? s.ToDateTime(TimeOnly.MinValue) : null,
                EndDate = editEndDate is { } e ? e.ToDateTime(TimeOnly.MinValue) : null,
            });
            editingDates = false;
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

}
