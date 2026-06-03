using Futelo.Client.Services.Language;
using Futelo.Client.Services.Vault;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Futelo.Client.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private ILanguageService Lang { get; set; } = default!;
    [Inject] private IVaultService VaultService { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private bool collapseNavMenu = true;
    private string? profileUrl;
    private string? currentUserId;

    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    protected override void OnInitialized()
    {
        Lang.OnChange += HandleLanguageChange;
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            currentUserId = authState.User.FindFirst("sub")?.Value;
            if (currentUserId == null) return;

            var vaults = await VaultService.GetAllAsync();
            var firstVault = vaults.FirstOrDefault();
            if (firstVault != null)
                profileUrl = $"/vaults/{firstVault.Id}/players/{currentUserId}";
        }
        catch { /* best effort */ }
    }

    private void HandleLanguageChange() => InvokeAsync(StateHasChanged);

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }

    public void Dispose()
    {
        Lang.OnChange -= HandleLanguageChange;
    }
}
