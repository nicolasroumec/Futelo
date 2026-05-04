using Futelo.Client.Services.Auth;
using Futelo.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Login
{
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

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
            Nav.NavigateTo("/");
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
