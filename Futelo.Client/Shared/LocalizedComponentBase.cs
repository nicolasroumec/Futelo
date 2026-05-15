using Futelo.Client.Services.Language;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public abstract class LocalizedComponentBase : ComponentBase, IDisposable
{
    [Inject] protected ILanguageService Lang { get; set; } = null!;

    protected override void OnInitialized()
    {
        Lang.OnChange += OnLanguageChanged;
    }

    private void OnLanguageChanged() => InvokeAsync(StateHasChanged);

    public virtual void Dispose()
    {
        Lang.OnChange -= OnLanguageChanged;
    }
}
