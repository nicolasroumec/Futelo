using Futelo.Client.Services.Season;
using Futelo.Client.Services.Vault;
using Futelo.Shared.DTOs.Season;
using Futelo.Shared.DTOs.Vault;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Futelo.Client.Pages;

public partial class SeasonDetail
{
    [Parameter] public int Id { get; set; }
    [Inject] private ISeasonService SeasonService { get; set; } = null!;
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    private SeasonResponse? season;
    private List<VaultPlayerResponse> vaultPlayers = [];
    private HashSet<string> selectedPlayerIds = [];
    private ConfigureSeasonRequest configureModel = new();
    private bool isOwner;
    private bool isLoading = true;
    private bool isConfiguring;
    private string? errorMessage;
    private string? configureMessage;
    private string configureAlertClass = "alert-success";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthStateTask;
            var userId = authState.User.FindFirst("sub")?.Value;

            season = await SeasonService.GetByIdAsync(Id);

            var vault = await VaultService.GetByIdAsync(season.VaultId);
            vaultPlayers = vault.Players;
            isOwner = vault.OwnerId == userId;

            selectedPlayerIds = season.Players.Select(p => p.PlayerId).ToHashSet();
            configureModel.HasLeague = season.HasLeague;
            configureModel.HasCup = season.HasCup;
            configureModel.HasSuperCup = season.HasSuperCup;
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

    private void TogglePlayer(string playerId, bool selected)
    {
        if (selected) selectedPlayerIds.Add(playerId);
        else selectedPlayerIds.Remove(playerId);
    }

    private async Task HandleConfigure()
    {
        isConfiguring = true;
        configureMessage = null;

        try
        {
            configureModel.PlayerIds = selectedPlayerIds.ToList();
            await SeasonService.ConfigureAsync(Id, configureModel);
            season = await SeasonService.GetByIdAsync(Id);
            configureMessage = "Configuration saved.";
            configureAlertClass = "alert-success";
        }
        catch (Exception ex)
        {
            configureMessage = ex.Message;
            configureAlertClass = "alert-danger";
        }
        finally
        {
            isConfiguring = false;
        }
    }
}
