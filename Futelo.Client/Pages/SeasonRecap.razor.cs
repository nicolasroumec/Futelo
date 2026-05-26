using Futelo.Client.Services.Season;
using Futelo.Client.Services.Toast;
using Futelo.Shared.DTOs.Season;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Futelo.Client.Pages;

public partial class SeasonRecap
{
    [Parameter] public int Id { get; set; }
    [Inject] private ISeasonService SeasonService { get; set; } = null!;
    [Inject] private IToastService Toast { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private SeasonRecapResponse? recap;
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            recap = await SeasonService.GetRecapAsync(Id);
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

    private async Task CopyLink()
    {
        await JS.InvokeVoidAsync("navigator.clipboard.writeText", Nav.Uri);
        Toast.Show("Link copied!");
    }
}
