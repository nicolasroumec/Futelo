using Futelo.Client.Services.Language;
using Futelo.Client.Services.Stats;
using Futelo.Shared.DTOs.Stats;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Player;

public partial class HeadToHead : IDisposable
{
    [Parameter] public int VaultId { get; set; }
    [Parameter] public string Player1Id { get; set; } = string.Empty;
    [Parameter] public string Player2Id { get; set; } = string.Empty;
    [Inject] private IStatsService StatsService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;

    private HeadToHeadResponse? h2h;
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        Lang.OnChange += HandleLanguageChange;
        try
        {
            h2h = await StatsService.GetHeadToHeadAsync(Player1Id, Player2Id, VaultId);
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

    private void HandleLanguageChange() => InvokeAsync(StateHasChanged);

    public void Dispose() => Lang.OnChange -= HandleLanguageChange;
}
