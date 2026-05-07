using Futelo.Client.Services.League;
using Futelo.Shared.DTOs.League;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.League;

public partial class LeagueView
{
    [Parameter] public int Id { get; set; }
    [Inject] private ILeagueService LeagueService { get; set; } = null!;

    private LeagueResponse? league;
    private bool isLoading = true;
    private bool isWorking;
    private string? errorMessage;

    private int? recordingMatchId;
    private int homeScore;
    private int awayScore;
    private RecordResultResponse? lastResult;

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
            league = await LeagueService.GetByIdAsync(Id);
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

    private void SelectMatch(int matchId)
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
            lastResult = null;
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

    private async Task HandleRecordResult()
    {
        if (recordingMatchId == null) return;
        isWorking = true;
        errorMessage = null;
        try
        {
            var request = new RecordResultRequest { HomeScore = homeScore, AwayScore = awayScore };
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
            isWorking = false;
        }
    }

    private static string FormatEloChange(EloChangeResult p)
    {
        string arrow = p.RankAfter < p.RankBefore ? "↑" : p.RankAfter > p.RankBefore ? "↓" : "→";
        string sign = p.EloChange >= 0 ? "+" : "";
        return $"{p.DisplayName}   {p.EloBefore} → {p.EloAfter} ({sign}{p.EloChange})  {arrow} #{p.RankAfter}";
    }
}
