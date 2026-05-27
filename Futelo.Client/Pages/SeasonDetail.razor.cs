using Futelo.Client.Services.Season;
using Futelo.Client.Services.Teams;
using Futelo.Client.Services.Toast;
using Futelo.Client.Services.Vault;
using Futelo.Client.Services.VideoGames;
using Futelo.Client.Shared;
using Futelo.Shared;
using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.Season;
using Futelo.Shared.DTOs.Team;
using Futelo.Shared.DTOs.Vault;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Futelo.Client.Pages;

public partial class SeasonDetail : LocalizedComponentBase
{
    [Parameter] public int Id { get; set; }
    [Inject] private ISeasonService SeasonService { get; set; } = null!;
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;
    [Inject] private ITeamService TeamService { get; set; } = null!;
    [Inject] private IToastService Toast { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    private SeasonResponse? season;
    private List<VaultPlayerResponse> vaultPlayers = [];
    private List<VideoGameResponse> videoGames = [];
    private List<TeamResponse> teams = [];
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
    private bool showDeleteModal;
    private string? errorMessage;

    private bool editingSeasonDates;
    private DateOnly? editSeasonStartDate;
    private DateOnly? editSeasonEndDate;
    private bool isSavingSeasonDates;

    private bool HasRightContent => season != null &&
        (season.TopStandings.Count > 0 || season.RecentMatches.Count > 0
            || (isOwner && season.Status == CompetitionStatus.Draft));

    private bool CanFinish => season != null &&
        (!season.HasLeague || season.LeagueStatus == CompetitionStatus.Finished) &&
        (!season.HasCup || season.CupStatus == CompetitionStatus.Finished) &&
        (!season.HasSuperCup || season.SuperCupStatus == CompetitionStatus.Finished);

    private bool StepCompDone => season != null && (season.HasLeague || season.HasCup);
    private bool StepPlayersDone => season != null && season.Players.Count >= 2;
    private bool StepReadyToActivate => StepCompDone && StepPlayersDone;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthStateTask;
            var userId = authState.User.FindFirst("sub")?.Value;

            season = await SeasonService.GetByIdAsync(Id, ComponentToken);

            var vault = await VaultService.GetByIdAsync(season.VaultId, ComponentToken);
            vaultPlayers = vault.Players;
            isOwner = vault.OwnerId == userId;

            videoGames = await VideoGameService.GetAllAsync(ComponentToken);
            teams = await TeamService.GetAllAsync(ComponentToken);
            selectedVideoGameId = season.VideoGameId;
            selectedPlayerIds = season.Players.Select(p => p.PlayerId).ToHashSet();
            configureModel.HasLeague = season.HasLeague;
            configureModel.LeagueName = season.LeagueName;
            configureModel.LeagueIsHomeAndAway = season.LeagueIsHomeAndAway;
            configureModel.HasCup = season.HasCup;
            configureModel.CupName = season.CupName;
            configureModel.HasSuperCup = season.HasSuperCup;
            configureModel.SuperCupName = season.SuperCupName;
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

    private void BeginEditSeasonDates()
    {
        editSeasonStartDate = season?.StartDate is { } s ? DateOnly.FromDateTime(s) : null;
        editSeasonEndDate = season?.EndDate is { } e ? DateOnly.FromDateTime(e) : null;
        editingSeasonDates = true;
    }

    private async Task HandlePatchSeasonDates()
    {
        isSavingSeasonDates = true;
        try
        {
            await SeasonService.PatchDatesAsync(Id, new PatchDatesRequest
            {
                StartDate = editSeasonStartDate is { } s ? s.ToDateTime(TimeOnly.MinValue) : null,
                EndDate = editSeasonEndDate is { } e ? e.ToDateTime(TimeOnly.MinValue) : null,
            });
            editingSeasonDates = false;
            season = await SeasonService.GetByIdAsync(Id);
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isSavingSeasonDates = false;
        }
    }

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
            showDeleteModal = false;
        }
    }

    private async Task HandleSetPlayerTeam(string playerId, int? teamId)
    {
        try
        {
            await SeasonService.SetPlayerTeamAsync(Id, playerId, teamId);
            season = await SeasonService.GetByIdAsync(Id);
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
    }

    private async Task HandlePatchVideoGame()
    {
        isPatchingVideoGame = true;
        try
        {
            await SeasonService.PatchVideoGameAsync(Id, selectedVideoGameId);
            season = await SeasonService.GetByIdAsync(Id);
            Toast.Show(Lang.Get("season.videoGameUpdated"));
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isPatchingVideoGame = false;
        }
    }

    private async Task HandleFinish()
    {
        isFinishing = true;
        try
        {
            await SeasonService.FinishAsync(Id);
            season = await SeasonService.GetByIdAsync(Id);
            Toast.Show(Lang.Get("season.finishSuccess"));
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isFinishing = false;
        }
    }

    private async Task HandleActivate()
    {
        isActivating = true;
        try
        {
            await SeasonService.ActivateAsync(Id);
            season = await SeasonService.GetByIdAsync(Id);
            Toast.Show(Lang.Get("season.activateSuccess"));
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isActivating = false;
        }
    }

    private async Task CopyRecapLink()
    {
        await JS.InvokeVoidAsync("navigator.clipboard.writeText", $"{Nav.BaseUri}seasons/{Id}/recap");
        Toast.Show("Link copied!");
    }

    private async Task HandleConfigure()
    {
        isConfiguring = true;
        try
        {
            configureModel.PlayerIds = selectedPlayerIds.ToList();
            await SeasonService.ConfigureAsync(Id, configureModel);
            season = await SeasonService.GetByIdAsync(Id);
            Toast.Show(Lang.Get("season.configSaved"));
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isConfiguring = false;
        }
    }
}
