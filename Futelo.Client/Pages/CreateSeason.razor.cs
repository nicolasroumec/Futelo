using Futelo.Client.Services.Season;
using Futelo.Shared.DTOs.Season;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class CreateSeason
{
    [Parameter] public int VaultId { get; set; }
    [Inject] private ISeasonService SeasonService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private CreateSeasonRequest model = new();
    private string? errorMessage;
    private bool isLoading;

    protected override void OnParametersSet()
    {
        model.VaultId = VaultId;
        model.Year = DateTime.UtcNow.Year;
    }

    private async Task HandleCreate()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var season = await SeasonService.CreateAsync(model);
            Nav.NavigateTo($"/seasons/{season.Id}");
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
