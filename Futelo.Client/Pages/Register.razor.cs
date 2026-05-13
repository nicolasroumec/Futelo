using Futelo.Client.Services.Auth;
using Futelo.Client.Services.Language;
using Futelo.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Register : IDisposable
{
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    [SupplyParameterFromQuery(Name = "returnUrl")]
    private string? ReturnUrl { get; set; }

    private RegisterRequest model = new();
    private string? errorMessage;
    private bool isLoading;

    protected override void OnInitialized()
    {
        Lang.OnChange += HandleLanguageChange;
    }

    private void HandleLanguageChange() => InvokeAsync(StateHasChanged);

    private async Task HandleRegister()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            await AuthService.RegisterAsync(model);
            Nav.NavigateTo(ReturnUrl ?? "/");
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

    public void Dispose()
    {
        Lang.OnChange -= HandleLanguageChange;
    }
}
