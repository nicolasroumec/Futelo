using Microsoft.JSInterop;

namespace Futelo.Client.Services.Theme;

public class ThemeService(IJSRuntime js) : IThemeService
{
    private const string DefaultTheme = "dark";

    private bool _initialized;

    public string CurrentTheme { get; private set; } = DefaultTheme;

    public event Action? OnChange;

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        var saved = await js.InvokeAsync<string?>("themeInterop.get");
        CurrentTheme = saved ?? DefaultTheme;

        await js.InvokeVoidAsync("themeInterop.apply", CurrentTheme);
        _initialized = true;
    }

    public async Task SetThemeAsync(string theme)
    {
        if (CurrentTheme == theme) return;

        CurrentTheme = theme;
        await js.InvokeVoidAsync("themeInterop.set", theme);
        OnChange?.Invoke();
    }
}
