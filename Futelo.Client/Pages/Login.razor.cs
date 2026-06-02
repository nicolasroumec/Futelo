using Futelo.Client.Services.Auth;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Login : LocalizedComponentBase
{
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    [SupplyParameterFromQuery(Name = "returnUrl")]
    private string? ReturnUrl { get; set; }

    private LoginRequest model = new();
    private string? errorMessage;
    private bool isLoading;

    private async Task HandleLogin()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            await AuthService.LoginAsync(model);
            Nav.NavigateTo(ReturnUrl ?? "/dashboard");
        }
        catch (Exception)
        {
            errorMessage = Lang.Get("auth.login.error");
        }
        finally
        {
            isLoading = false;
        }
    }
}
