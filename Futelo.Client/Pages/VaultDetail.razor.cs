using Futelo.Client.Services.Season;
using Futelo.Client.Services.Vault;
using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.DTOs.Season;
using Futelo.Shared.DTOs.Vault;
using Futelo.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Futelo.Client.Pages;

public partial class VaultDetail
{
    [Parameter] public int Id { get; set; }
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private ISeasonService SeasonService { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    private VaultResponse? vault;
    private List<SeasonResponse> seasons = [];
    private bool isOwner;
    private bool isAdmin;
    private bool isLoading = true;
    private string? errorMessage;

    private InviteRequest inviteModel = new();
    private bool isInviting;
    private string? inviteMessage;
    private string inviteAlertClass = "alert-success";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthStateTask;
            var userId = authState.User.FindFirst("sub")?.Value;
            vault = await VaultService.GetByIdAsync(Id);
            isOwner = vault.OwnerId == userId;
            isAdmin = vault.Players.Any(p => p.PlayerId == userId && p.Role == VaultRole.Admin);
            seasons = await SeasonService.GetByVaultAsync(Id);
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

    private static string RoleBadgeClass(VaultRole role) => role switch
    {
        VaultRole.Admin => "bg-primary",
        VaultRole.Editor => "bg-warning text-dark",
        _ => "bg-secondary"
    };

    private async Task HandleInvite()
    {
        isInviting = true;
        inviteMessage = null;

        try
        {
            await VaultService.InviteAsync(Id, inviteModel);
            inviteMessage = $"Invitation sent to {inviteModel.Email}.";
            inviteAlertClass = "alert-success";
            inviteModel = new();
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
}
