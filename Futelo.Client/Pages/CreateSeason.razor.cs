using Futelo.Client.Services.Season;
using Futelo.Client.Services.VideoGames;
using Futelo.Shared.DTOs.Season;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class CreateSeason
{
    [Parameter] public int VaultId { get; set; }
    [Inject] private ISeasonService SeasonService { get; set; } = null!;
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private CreateSeasonRequest model = new();
    private List<VideoGameResponse> videoGames = [];
    private string? errorMessage;
    private bool isLoading;

    protected override async Task OnParametersSetAsync()
    {
        model.VaultId = VaultId;
        model.Year = DateTime.UtcNow.Year;
        videoGames = await VideoGameService.GetAllAsync();
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
