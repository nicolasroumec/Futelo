using Futelo.Client.Services.Language;
using Futelo.Client.Services.Season;
using Futelo.Client.Services.Teams;
using Futelo.Client.Services.Vault;
using Futelo.Client.Services.VideoGames;
using Futelo.Shared.DTOs.Season;
using Futelo.Shared.DTOs.Team;
using Futelo.Shared.DTOs.Vault;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Futelo.Client.Pages;

public partial class SeasonDetail : IDisposable
{
    [Parameter] public int Id { get; set; }
    [Inject] private ISeasonService SeasonService { get; set; } = null!;
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;
    [Inject] private ITeamService TeamService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    private SeasonResponse? season;
    private List<VaultPlayerResponse> vaultPlayers = [];
    private List<VideoGameResponse> videoGames = [];
    private List<TeamResponse> teams = [];
    private Dictionary<string, int?> playerTeamSelections = [];
    private HashSet<string> selectedPlayerIds = [];
    private ConfigureSeasonRequest configureModel = new();
    private int? selectedVideoGameId;
    private bool isOwner;
    private bool isLoading = true;
    private bool isConfiguring;
    private bool isActivating;
    private bool isFinishing;
    private bool isPatchingVideoGame;
    private bool isDeleting;
    private bool confirmDelete;
    private string? errorMessage;
    private string? configureMessage;
    private string configureAlertClass = "alert-success";
    private string? activateMessage;
    private string activateAlertClass = "alert-success";
    private string? finishMessage;
    private string finishAlertClass = "alert-success";
    private string? videoGameMessage;
    private string videoGameAlertClass = "alert-success";

    private bool CanFinish => season != null &&
        (!season.HasLeague || season.LeagueStatus == "Finished") &&
        (!season.HasCup || season.CupStatus == "Finished") &&
        (!season.HasSuperCup || season.SuperCupStatus == "Finished");

    protected override async Task OnInitializedAsync()
    {
        Lang.OnChange += HandleLanguageChange;

        try
        {
            var authState = await AuthStateTask;
            var userId = authState.User.FindFirst("sub")?.Value;

            season = await SeasonService.GetByIdAsync(Id);

            var vault = await VaultService.GetByIdAsync(season.VaultId);
            vaultPlayers = vault.Players;
            isOwner = vault.OwnerId == userId;

            videoGames = await VideoGameService.GetAllAsync();
            teams = await TeamService.GetAllAsync();
            selectedVideoGameId = season.VideoGameId;
            selectedPlayerIds = season.Players.Select(p => p.PlayerId).ToHashSet();
            playerTeamSelections = season.Players.ToDictionary(p => p.PlayerId, p => p.TeamId);
            configureModel.HasLeague = season.HasLeague;
            configureModel.LeagueName = season.LeagueName;
            configureModel.LeagueIsHomeAndAway = season.LeagueIsHomeAndAway;
            configureModel.HasCup = season.HasCup;
            configureModel.CupName = season.CupName;
            configureModel.HasSuperCup = season.HasSuperCup;
            configureModel.SuperCupName = season.SuperCupName;
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

    private async Task HandleDelete()
    {
        isDeleting = true;
        try
        {
            await SeasonService.DeleteAsync(Id);
            Nav.NavigateTo($"/vaults/{season!.VaultId}");
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            isDeleting = false;
            confirmDelete = false;
        }
    }

    private async Task HandleSetPlayerTeam(string playerId)
    {
        try
        {
            var teamId = playerTeamSelections.TryGetValue(playerId, out var t) ? t : null;
            await SeasonService.SetPlayerTeamAsync(Id, playerId, teamId);
            season = await SeasonService.GetByIdAsync(Id);
            playerTeamSelections = season.Players.ToDictionary(p => p.PlayerId, p => p.TeamId);
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }

    private async Task HandlePatchVideoGame()
    {
        isPatchingVideoGame = true;
        videoGameMessage = null;

        try
        {
            await SeasonService.PatchVideoGameAsync(Id, selectedVideoGameId);
            season = await SeasonService.GetByIdAsync(Id);
            videoGameMessage = Lang.Get("season.videoGameUpdated");
            videoGameAlertClass = "alert-success";
        }
        catch (Exception ex)
        {
            videoGameMessage = ex.Message;
            videoGameAlertClass = "alert-danger";
        }
        finally
        {
            isPatchingVideoGame = false;
        }
    }

    private void TogglePlayer(string playerId, bool selected)
    {
        if (selected) selectedPlayerIds.Add(playerId);
        else selectedPlayerIds.Remove(playerId);
    }

    private async Task HandleFinish()
    {
        isFinishing = true;
        finishMessage = null;

        try
        {
            await SeasonService.FinishAsync(Id);
            season = await SeasonService.GetByIdAsync(Id);
            finishMessage = Lang.Get("season.finishSuccess");
            finishAlertClass = "alert-success";
        }
        catch (Exception ex)
        {
            finishMessage = ex.Message;
            finishAlertClass = "alert-danger";
        }
        finally
        {
            isFinishing = false;
        }
    }

    private async Task HandleActivate()
    {
        isActivating = true;
        activateMessage = null;

        try
        {
            await SeasonService.ActivateAsync(Id);
            season = await SeasonService.GetByIdAsync(Id);
            activateMessage = Lang.Get("season.activateSuccess");
            activateAlertClass = "alert-success";
        }
        catch (Exception ex)
        {
            activateMessage = ex.Message;
            activateAlertClass = "alert-danger";
        }
        finally
        {
            isActivating = false;
        }
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
            configureMessage = Lang.Get("season.configSaved");
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

    public void Dispose()
    {
        Lang.OnChange -= HandleLanguageChange;
    }
}
