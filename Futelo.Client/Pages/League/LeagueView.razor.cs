using Futelo.Client.Services.League;
using Futelo.Client.Services.Teams;
using Futelo.Client.Services.VideoGames;
using Futelo.Client.Shared;
using Futelo.Shared;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.DTOs.Team;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.League;

public partial class LeagueView : LocalizedComponentBase
{
    [Parameter] public int Id { get; set; }
    [Inject] private ILeagueService LeagueService { get; set; } = null!;
    [Inject] private ITeamService TeamService { get; set; } = null!;
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;

    private LeagueResponse? league;
    private bool isLoading = true;
    private bool isWorking;
    private string? errorMessage;

    private int? recordingMatchId;
    private bool isRecording;
    private RecordResultResponse? lastResult;

    private int selectedMatchday;

    private int? editingMatchId;
    private List<TeamResponse> teams = [];
    private List<VideoGameResponse> videoGames = [];

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
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            league = await LeagueService.GetByIdAsync(Id, ComponentToken);
            AutoSelectMatchday();
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
            lastResult = null;
            editingMatchId = null;
        }
        errorMessage = null;
    }

    private async Task HandleStart()
    {
        isWorking = true;
        errorMessage = null;
        try
        {
            await LeagueService.StartAsync(Id);
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

    private async Task HandleReshuffle()
    {
        isWorking = true;
        errorMessage = null;
        try
        {
            await LeagueService.ReshuffleAsync(Id);
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

    private async Task HandleRecordResult(MatchResultInput input)
    {
        if (recordingMatchId == null) return;
        isRecording = true;
        errorMessage = null;
        try
        {
            var request = new RecordResultRequest { HomeScore = input.HomeScore, AwayScore = input.AwayScore };
            lastResult = await LeagueService.RecordResultAsync(Id, recordingMatchId.Value, request);
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
        errorMessage = null;
    }

    private async Task HandlePatchMatch(PatchMatchRequest request)
    {
        if (editingMatchId == null) return;
        errorMessage = null;
        try
        {
            await LeagueService.PatchMatchAsync(Id, editingMatchId.Value, request);
            editingMatchId = null;
            await LoadAsync();
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }

    private static string FormatEloChange(EloChangeResult p)
    {
        string arrow = p.RankAfter < p.RankBefore ? "↑" : p.RankAfter > p.RankBefore ? "↓" : "→";
        string sign = p.EloChange >= 0 ? "+" : "";
        return $"{p.DisplayName}   {p.EloBefore} → {p.EloAfter} ({sign}{p.EloChange})  {arrow} #{p.RankAfter}";
    }
}
