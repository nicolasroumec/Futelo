namespace Futelo.Client.Services.Language;

public interface ILanguageService
{
    string CurrentLanguage { get; }
    string Get(string key);
    Task InitializeAsync();
    Task SetLanguageAsync(string language);
    event Action? OnChange;
}
