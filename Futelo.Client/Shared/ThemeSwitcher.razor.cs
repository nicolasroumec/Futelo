using Futelo.Client.Services.Theme;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class ThemeSwitcher : IDisposable
{
    [Inject] private IThemeService Theme { get; set; } = default!;

    protected override void OnInitialized()
    {
        Theme.OnChange += HandleThemeChange;
    }

    private void HandleThemeChange() => InvokeAsync(StateHasChanged);

    private Task ToggleAsync() =>
        Theme.SetThemeAsync(Theme.CurrentTheme == "dark" ? "light" : "dark");

    private string Title => Theme.CurrentTheme == "dark" ? "Switch to light mode" : "Switch to dark mode";

    public void Dispose()
    {
        Theme.OnChange -= HandleThemeChange;
    }
}
