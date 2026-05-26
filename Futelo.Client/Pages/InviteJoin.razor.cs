using Futelo.Client.Services.Invitation;
using Futelo.Shared.DTOs.Invitation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Futelo.Client.Pages;

public partial class InviteJoin
{
    [SupplyParameterFromQuery(Name = "token")]
    private string? Token { get; set; }

    [Inject] private IInvitationService InvitationService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    private InvitationResponse? preview;
    private bool isLoading = true;
    private string? errorMessage;

    private string LoginUrl => $"/login?returnUrl={Uri.EscapeDataString($"/invitations/{Token}/accept")}";
    private string RegisterUrl => $"/register?returnUrl={Uri.EscapeDataString($"/invitations/{Token}/accept")}";

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            errorMessage = "Invalid invitation link.";
            isLoading = false;
            return;
        }

        try
        {
            preview = await InvitationService.GetPreviewAsync(Token);

            var authState = await AuthStateTask;
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                await InvitationService.AcceptAsync(Token);
                Nav.NavigateTo($"/vaults/{preview.VaultId}");
                return;
            }
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
