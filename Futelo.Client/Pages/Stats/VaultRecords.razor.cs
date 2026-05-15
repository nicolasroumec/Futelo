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
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            records = await StatsService.GetVaultRecordsAsync(VaultId);
            topScoringMatch = await StatsService.GetTopScoringMatchAsync(VaultId);
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
}
