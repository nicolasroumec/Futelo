using Futelo.Client.Services.Language;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private ILanguageService Lang { get; set; } = default!;

    private bool collapseNavMenu = true;

    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    protected override void OnInitialized()
    {
        Lang.OnChange += HandleLanguageChange;
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
