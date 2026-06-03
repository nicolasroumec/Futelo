using Futelo.Client.Services.Stats;
using Futelo.Client.Services.Users;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Stats;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Stats;

public partial class Ranking : LocalizedComponentBase
{
    [Parameter] public int VaultId { get; set; }
    [Parameter] public int SeasonId { get; set; }
    [Inject] private IStatsService StatsService { get; set; } = null!;
    [Inject] private AvatarDirectory Avatars { get; set; } = null!;

    private List<RankingRow> rows = [];
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var rankingTask = StatsService.GetRankingAsync(SeasonId, VaultId, ComponentToken);
            await Task.WhenAll(rankingTask, Avatars.EnsureLoadedAsync());
            rows = rankingTask.Result;
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
