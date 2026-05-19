using Futelo.Client.Services.Language;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class LanguageSwitcher : IDisposable
{
    [Inject] private ILanguageService Lang { get; set; } = default!;

    protected override void OnInitialized()
    {
        Lang.OnChange += HandleLanguageChange;
    }

    private void HandleLanguageChange() => InvokeAsync(StateHasChanged);

    private Task SetSpanishAsync() => Lang.SetLanguageAsync("es");
    private Task SetEnglishAsync() => Lang.SetLanguageAsync("en");

    private string GetButtonClass(string language) =>
        language == Lang.CurrentLanguage ? "lang-btn lang-btn--active" : "lang-btn";

    public void Dispose()
    {
        Lang.OnChange -= HandleLanguageChange;
    }
}
