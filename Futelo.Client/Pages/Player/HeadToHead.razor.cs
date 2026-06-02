using Futelo.Client.Services.Stats;
using Futelo.Client.Services.Users;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Stats;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Player;

public partial class HeadToHead : LocalizedComponentBase
{
    [Parameter] public int VaultId { get; set; }
    [Parameter] public string Player1Id { get; set; } = string.Empty;
    [Parameter] public string Player2Id { get; set; } = string.Empty;
    [Inject] private IStatsService StatsService { get; set; } = null!;
    [Inject] private AvatarDirectory Avatars { get; set; } = null!;

    private HeadToHeadResponse? h2h;
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var h2hTask = StatsService.GetHeadToHeadAsync(Player1Id, Player2Id, VaultId, ComponentToken);
            await Task.WhenAll(h2hTask, Avatars.EnsureLoadedAsync());
            h2h = h2hTask.Result;
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
