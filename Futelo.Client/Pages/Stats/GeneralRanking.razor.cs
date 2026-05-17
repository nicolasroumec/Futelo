using Futelo.Client.Services.Stats;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Stats;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Stats;

public partial class GeneralRanking : LocalizedComponentBase
{
    [Parameter] public int VaultId { get; set; }
    [Inject] private IStatsService StatsService { get; set; } = null!;

    private List<RankingRow> rows = [];
    private List<ScorerRow> scorers = [];
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var rankingTask = StatsService.GetGeneralRankingAsync(VaultId, ComponentToken);
            var scorersTask = StatsService.GetScorersAsync(VaultId, ComponentToken);
            await Task.WhenAll(rankingTask, scorersTask);
            rows = rankingTask.Result;
            scorers = scorersTask.Result;
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
}
