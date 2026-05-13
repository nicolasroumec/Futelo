using Futelo.Client.Services.Language;
using Futelo.Client.Services.Stats;
using Futelo.Shared.DTOs.Stats;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Stats;

public partial class Scorers : IDisposable
{
    [Parameter] public int VaultId { get; set; }
    [Inject] private IStatsService StatsService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;

    private List<ScorerRow> rows = [];
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        Lang.OnChange += HandleLanguageChange;
        try
        {
            rows = await StatsService.GetScorersAsync(VaultId);
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
