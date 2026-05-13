using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace Futelo.Client.Services.Language;

public class LanguageService(HttpClient http, IJSRuntime js) : ILanguageService
{
    private const string StorageKey = "futelo_language";
    private const string DefaultLanguage = "es";

    private Dictionary<string, string> _strings = [];
    private bool _initialized;

    public string CurrentLanguage { get; private set; } = DefaultLanguage;

    public event Action? OnChange;

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        var saved = await js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        CurrentLanguage = saved ?? DefaultLanguage;

        await LoadStringsAsync(CurrentLanguage);
        _initialized = true;
    }

    public string Get(string key) =>
        _strings.TryGetValue(key, out var value) ? value : key;

    public async Task SetLanguageAsync(string language)
    {
        if (CurrentLanguage == language) return;

        CurrentLanguage = language;
        await js.InvokeVoidAsync("localStorage.setItem", StorageKey, language);
        await LoadStringsAsync(language);
        OnChange?.Invoke();
    }

    private async Task LoadStringsAsync(string language)
    {
        var dict = await http.GetFromJsonAsync<Dictionary<string, string>>($"i18n/{language}.json");
        _strings = dict ?? [];
    }
}
