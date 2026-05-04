using Futelo.Client.Services.Auth;
using Futelo.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Register
{
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private RegisterRequest model = new();
    private string? errorMessage;
    private bool isLoading;

    private async Task HandleRegister()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            await AuthService.RegisterAsync(model);
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
