using Futelo.Client.Services.Stats;
using Futelo.Client.Services.Teams;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Stats;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Stats;

public partial class TeamPanel : LocalizedComponentBase
{
    [Parameter] public int VaultId { get; set; }
    [Inject] private IStatsService StatsService { get; set; } = null!;
    [Inject] private ShieldDirectory Shields { get; set; } = null!;

    private List<TeamPanelRow> rows = [];
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var rowsTask = StatsService.GetTeamPanelAsync(VaultId, ComponentToken);
            await Task.WhenAll(rowsTask, Shields.EnsureLoadedAsync());
            rows = rowsTask.Result;
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
