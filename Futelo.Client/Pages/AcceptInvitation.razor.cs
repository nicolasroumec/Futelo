using Futelo.Client.Services.Invitation;
using Futelo.Client.Shared;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class AcceptInvitation : LocalizedComponentBase
{
    [Parameter] public string Token { get; set; } = string.Empty;
    [Inject] private IInvitationService InvitationService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await InvitationService.AcceptAsync(Token);
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
