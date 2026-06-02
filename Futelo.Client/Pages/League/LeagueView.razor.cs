using Futelo.Client.Services.League;
using Futelo.Client.Services.Teams;
using Futelo.Client.Services.Toast;
using Futelo.Client.Services.Users;
using Futelo.Client.Services.VideoGames;
using Futelo.Client.Shared;
using Futelo.Shared;
using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.DTOs.Team;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Futelo.Client.Pages.League;

public partial class LeagueView : LocalizedComponentBase
{
    [Parameter] public int Id { get; set; }
    [Inject] private ILeagueService LeagueService { get; set; } = null!;
    [Inject] private ITeamService TeamService { get; set; } = null!;
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;
    [Inject] private IToastService Toast { get; set; } = null!;
    [Inject] private AvatarDirectory Avatars { get; set; } = null!;
    [Inject] private ShieldDirectory Shields { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    private LeagueResponse? league;
    private string? currentUserId;
    private bool isLoading = true;
    private bool isWorking;
    private bool confirmReshuffle;
    private string? errorMessage;

    private int? recordingMatchId;
    private bool isRecording;
    private int? lastResultMatchId;
    private RecordResultResponse? lastEloResult;

    private int selectedMatchday;
    private bool _matchdayInitialized;

    private int? editingMatchId;
    private int? correctingMatchId;
    private bool isCorrecting;
    private List<TeamResponse> teams = [];
    private List<VideoGameResponse> videoGames = [];

    private bool editingDates;
    private DateOnly? editStartDate;
    private DateOnly? editEndDate;
    private bool isSavingDates;

    private bool addingMatch;
    private int newMatchday = 1;
    private string newHomePlayerId = string.Empty;
    private string newAwayPlayerId = string.Empty;
    private bool isAddingMatch;

    private string TiebreakerSummary => string.Join(" › ",
        league!.TiebreakerCriteria.Select(c => Lang.Get(CriterionNameKey(c))));

    private string FinalTiebreakerKey => league!.FinalTiebreaker switch
    {
        Futelo.Shared.Enums.FinalTiebreaker.DrawingOfLots => "season.tiebreaker.final.drawingOfLots",
        Futelo.Shared.Enums.FinalTiebreaker.Playoff       => "season.tiebreaker.final.playoff",
        _                                                  => "season.tiebreaker.final.alphabetical"
    };

    private static string CriterionNameKey(Futelo.Shared.Enums.TiebreakerCriterion c) => c switch
    {
        Futelo.Shared.Enums.TiebreakerCriterion.HeadToHeadPoints         => "season.tiebreaker.h2hPoints",
        Futelo.Shared.Enums.TiebreakerCriterion.HeadToHeadWins           => "season.tiebreaker.h2hWins",
        Futelo.Shared.Enums.TiebreakerCriterion.HeadToHeadGoalDifference => "season.tiebreaker.h2hGD",
        Futelo.Shared.Enums.TiebreakerCriterion.HeadToHeadGoalsFor       => "season.tiebreaker.h2hGF",
        Futelo.Shared.Enums.TiebreakerCriterion.GoalDifference           => "season.tiebreaker.goalDifference",
        Futelo.Shared.Enums.TiebreakerCriterion.GoalsFor                 => "season.tiebreaker.goalsFor",
        Futelo.Shared.Enums.TiebreakerCriterion.Wins                     => "season.tiebreaker.wins",
        _                                                                 => c.ToString()
    };

    private List<int> Matchdays => league?.Matches
        .Select(m => m.Matchday)
        .Distinct()
        .OrderBy(d => d)
        .ToList() ?? [];

    private List<(string Name, int Count)> PendingByPlayer => league?.Matches
        .Where(m => m.Status == MatchStatus.Pending)
        .SelectMany(m => new[] { m.HomePlayerName, m.AwayPlayerName })
        .GroupBy(n => n)
        .Select(g => (g.Key, g.Count()))
        .OrderByDescending(x => x.Item2)
        .ToList() ?? [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateTask;
        currentUserId = authState.User.FindFirst("sub")?.Value;
        await Task.WhenAll(Avatars.EnsureLoadedAsync(), Shields.EnsureLoadedAsync());
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            league = await LeagueService.GetByIdAsync(Id, ComponentToken);
            if (!_matchdayInitialized)
            {
                AutoSelectMatchday();
                _matchdayInitialized = true;
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

    private void AutoSelectMatchday()
    {
        if (league == null) return;
        var days = Matchdays;
        if (days.Count == 0) return;

        var firstWithPending = days.FirstOrDefault(d =>
            league.Matches.Any(m => m.Matchday == d && m.Status == MatchStatus.Pending));

        selectedMatchday = firstWithPending != 0 ? firstWithPending : days[^1];
    }

    private void SelectMatch(int matchId)
    {
        if (recordingMatchId == matchId)
            recordingMatchId = null;
        else
        {
            recordingMatchId = matchId;
            editingMatchId = null;
            lastEloResult = null;
            lastResultMatchId = null;
        }
    }

    private async Task HandleStart()
    {
        isWorking = true;
        try
        {
            await LeagueService.StartAsync(Id);
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
            await LeagueService.StartManualAsync(Id);
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

    private void BeginAddMatch()
    {
        newMatchday = Matchdays.Count > 0 ? Matchdays.Max() + 1 : 1;
        newHomePlayerId = string.Empty;
        newAwayPlayerId = string.Empty;
        addingMatch = true;
    }

    private async Task HandleAddMatch()
    {
        if (string.IsNullOrEmpty(newHomePlayerId) || string.IsNullOrEmpty(newAwayPlayerId)) return;
        isAddingMatch = true;
        try
        {
            await LeagueService.AddMatchAsync(Id, new AddLeagueMatchRequest
            {
                Matchday = newMatchday,
                HomePlayerId = newHomePlayerId,
                AwayPlayerId = newAwayPlayerId
            });
            addingMatch = false;
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

    private async Task HandleReshuffle()
    {
        confirmReshuffle = false;
        isWorking = true;
        try
        {
            await LeagueService.ReshuffleAsync(Id);
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
            var request = new RecordResultRequest { HomeScore = input.HomeScore, AwayScore = input.AwayScore };
            lastEloResult = await LeagueService.RecordResultAsync(Id, matchId, request);
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
        if (!league!.CanEdit || league.Status == CompetitionStatus.NotStarted) return;
        var match = league.Matches.First(m => m.Id == matchId);
        if (match.Status == MatchStatus.Pending && league.Status == CompetitionStatus.Active)
            SelectMatch(matchId);
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

    private void HandleRequestCorrection(int matchId)
    {
        correctingMatchId = matchId;
        editingMatchId = null;
        recordingMatchId = null;
        lastEloResult = null;
    }

    private async Task HandleCorrectResult(MatchResultInput input)
    {
        if (correctingMatchId == null) return;
        isCorrecting = true;
        var matchId = correctingMatchId.Value;
        try
        {
            var request = new RecordResultRequest { HomeScore = input.HomeScore, AwayScore = input.AwayScore };
            lastEloResult = await LeagueService.RecordResultAsync(Id, matchId, request);
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
            await LeagueService.PatchMatchAsync(Id, editingMatchId.Value, request);
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
        editStartDate = league?.StartDate is { } s ? DateOnly.FromDateTime(s) : null;
        editEndDate = league?.EndDate is { } e ? DateOnly.FromDateTime(e) : null;
        editingDates = true;
    }

    private async Task HandlePatchDates()
    {
        isSavingDates = true;
        try
        {
            await LeagueService.PatchDatesAsync(Id, new PatchDatesRequest
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

}
