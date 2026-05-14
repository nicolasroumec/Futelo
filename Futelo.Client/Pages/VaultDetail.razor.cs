using Futelo.Client.Services.Language;
using Futelo.Client.Services.Season;
using Futelo.Client.Services.Vault;
using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.DTOs.Season;
using Futelo.Shared.DTOs.Vault;
using Futelo.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Futelo.Client.Pages;

public partial class VaultDetail : IDisposable
{
    [Parameter] public int Id { get; set; }
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private ISeasonService SeasonService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    private VaultResponse? vault;
    private List<SeasonResponse> seasons = [];
    private List<RecentMatchResponse> recentMatches = [];
    private bool isOwner;
    private bool isAdmin;
    private bool isLoading = true;
    private string? errorMessage;

    private InviteRequest inviteModel = new();
    private bool isInviting;
    private string? inviteLink;
    private string? inviteMessage;
    private string inviteAlertClass = "alert-success";

    protected override async Task OnInitializedAsync()
    {
        Lang.OnChange += HandleLanguageChange;

        try
        {
            var authState = await AuthStateTask;
            var userId = authState.User.FindFirst("sub")?.Value;
            vault = await VaultService.GetByIdAsync(Id);
            isOwner = vault.OwnerId == userId;
            isAdmin = vault.Players.Any(p => p.PlayerId == userId && p.Role == VaultRole.Admin);
            var seasonsTask = SeasonService.GetByVaultAsync(Id);
            var recentMatchesTask = VaultService.GetRecentMatchesAsync(Id, 5);
            await Task.WhenAll(seasonsTask, recentMatchesTask);
            seasons = seasonsTask.Result;
            recentMatches = recentMatchesTask.Result;
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

    private static string RoleBadgeClass(VaultRole role) => role switch
    {
        VaultRole.Admin => "bg-primary",
        VaultRole.Editor => "bg-warning text-dark",
        _ => "bg-secondary"
    };

    private static string MatchScore(RecentMatchResponse m)
    {
        var score = $"{m.HomeScore} - {m.AwayScore}";
        if (m.WonOnPenaltiesId is not null)
            score += $" ({m.HomePenaltyScore}-{m.AwayPenaltyScore} pen.)";
        return score;
    }

    private async Task HandleInvite()
    {
        isInviting = true;
        inviteLink = null;
        inviteMessage = null;

        try
        {
            var result = await VaultService.InviteAsync(Id, inviteModel);
            inviteLink = $"{Navigation.BaseUri}invitations/{result.Token}/accept";
        }
        catch (Exception ex)
        {
            inviteMessage = ex.Message;
            inviteAlertClass = "alert-danger";
        }
        finally
        {
            isInviting = false;
        }
    }

    private async Task CopyInviteLink()
    {
        if (inviteLink is not null)
            await JS.InvokeVoidAsync("navigator.clipboard.writeText", inviteLink);
    }

    public void Dispose()
    {
        Lang.OnChange -= HandleLanguageChange;
    }
}
