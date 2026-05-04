using Futelo.Client.Services.Auth;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Logout
{
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await AuthService.LogoutAsync();
        Nav.NavigateTo("/login");
    }
}
