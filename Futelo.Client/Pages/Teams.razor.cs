using Futelo.Client.Services.Teams;
using Futelo.Shared.DTOs.Team;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Teams
{
    [Inject] private ITeamService TeamService { get; set; } = null!;

    private List<TeamResponse> teams = [];
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        teams = await TeamService.GetAllAsync();
        isLoading = false;
    }
}
