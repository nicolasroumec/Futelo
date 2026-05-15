using Futelo.Client.Services.Language;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public abstract class LocalizedComponentBase : ComponentBase, IDisposable
{
    [Inject] protected ILanguageService Lang { get; set; } = null!;

    private readonly CancellationTokenSource _cts = new();
    protected CancellationToken ComponentToken => _cts.Token;

    protected override void OnInitialized()
    {
        Lang.OnChange += OnLanguageChanged;
    }

    private void OnLanguageChanged() => InvokeAsync(StateHasChanged);

    public virtual void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        Lang.OnChange -= OnLanguageChanged;
    }
}
