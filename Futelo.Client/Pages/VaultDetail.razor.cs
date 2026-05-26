using Futelo.Client.Services.Season;
using Futelo.Client.Services.Toast;
using Futelo.Client.Services.Vault;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.DTOs.Season;
using Futelo.Shared.DTOs.Vault;
using Futelo.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Futelo.Client.Pages;

public partial class VaultDetail : LocalizedComponentBase
{
    [Parameter] public int Id { get; set; }
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private ISeasonService SeasonService { get; set; } = null!;
    [Inject] private IToastService Toast { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    private VaultResponse? vault;
    private List<SeasonResponse> seasons = [];
    private List<FeedEventDto> feed = [];
    private bool isOwner;
    private bool isAdmin;
    private bool isLoading = true;
    private string? errorMessage;

    private bool isEditingName;
    private string editName = string.Empty;
    private bool isSavingName;

    private InviteRequest inviteModel = new();
    private bool isInviting;
    private string? inviteLink;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthStateTask;
            var userId = authState.User.FindFirst("sub")?.Value;
            vault = await VaultService.GetByIdAsync(Id, ComponentToken);
            isOwner = vault.OwnerId == userId;
            isAdmin = vault.Players.Any(p => p.PlayerId == userId && p.Role == VaultRole.Admin);
            var seasonsTask = SeasonService.GetByVaultAsync(Id, ComponentToken);
            var feedTask = VaultService.GetFeedAsync(Id, 5, ComponentToken);
            await Task.WhenAll(seasonsTask, feedTask);
            seasons = seasonsTask.Result;
            feed = feedTask.Result;
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

    private void StartEditName()
    {
        editName = vault!.Name;
        isEditingName = true;
    }

    private void CancelEditName()
    {
        isEditingName = false;
    }

    private async Task SaveNameAsync()
    {
        if (string.IsNullOrWhiteSpace(editName) || editName.Length < 3 || editName.Length > 50)
            return;

        isSavingName = true;
        try
        {
            await VaultService.UpdateAsync(Id, new UpdateVaultRequest { Name = editName });
            vault!.Name = editName;
            isEditingName = false;
            Toast.Show(Lang.Get("vault.renameSuccess"));
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isSavingName = false;
        }
    }

    private static string RoleBadgeClass(VaultRole role) => role switch
    {
        VaultRole.Admin => "bg-primary",
        VaultRole.Editor => "bg-warning text-dark",
        _ => "bg-secondary"
    };

    private static string EloChangeClass(int change) => change >= 0 ? "text-success" : "text-danger";
    private static string FormatEloChange(int change) => change >= 0 ? $"+{change}" : $"{change}";
    private static string RankChange(int before, int after) => before > 0 ? $" · #{before}→#{after}" : "";

    private async Task HandleInvite()
    {
        isInviting = true;
        inviteLink = null;
        try
        {
            var result = await VaultService.InviteAsync(Id, inviteModel);
            inviteLink = $"{Navigation.BaseUri}invite/join?token={result.Token}";
        }
        catch (Exception ex)
        {
            Toast.Show(ex.Message, ToastType.Error);
        }
        finally
        {
            isInviting = false;
        }
    }

    private async Task CopyInviteLink()
    {
        if (inviteLink is not null)
        {
            await JS.InvokeVoidAsync("navigator.clipboard.writeText", inviteLink);
            Toast.Show(Lang.Get("vault.inviteLinkCopied"));
        }
    }
}
