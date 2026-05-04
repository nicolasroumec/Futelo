using Futelo.Client.Services.VideoGames;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Games
{
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;

    private List<VideoGameResponse> games = [];
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        games = await VideoGameService.GetAllAsync();
        isLoading = false;
    }
}
