namespace Futelo.Client.Services.Theme;

public interface IThemeService
{
    string CurrentTheme { get; }
    Task InitializeAsync();
    Task SetThemeAsync(string theme);
    event Action? OnChange;
}
