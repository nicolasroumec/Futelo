using Futelo.Client.Services.Stats;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Stats;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Stats;

public partial class VaultRecords : LocalizedComponentBase
{
    [Parameter] public int VaultId { get; set; }
    [Inject] private IStatsService StatsService { get; set; } = null!;

    private VaultRecordsResponse? records;
    private TopScoringMatchResponse? topScoringMatch;
    private List<AllTimeStandingRow> standings = [];
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var recordsTask     = StatsService.GetVaultRecordsAsync(VaultId, ComponentToken);
            var topMatchTask    = StatsService.GetTopScoringMatchAsync(VaultId, ComponentToken);
            var standingsTask   = StatsService.GetAllTimeStandingsAsync(VaultId, ComponentToken);
            await Task.WhenAll(recordsTask, topMatchTask, standingsTask);
            records         = recordsTask.Result;
            topScoringMatch = topMatchTask.Result;
            standings       = standingsTask.Result;
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
