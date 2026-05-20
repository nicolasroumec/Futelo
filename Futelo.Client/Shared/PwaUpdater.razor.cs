using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Futelo.Client.Shared;

public partial class PwaUpdater : IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private bool _updateAvailable;
    private DotNetObjectReference<PwaUpdater>? _selfRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        _selfRef = DotNetObjectReference.Create(this);
        await JS.InvokeVoidAsync("pwaUpdater.register", _selfRef);
    }

    [JSInvokable]
    public void OnUpdateAvailable()
    {
        _updateAvailable = true;
        StateHasChanged();
    }

    private async Task ApplyUpdateAsync()
    {
        await JS.InvokeVoidAsync("pwaUpdater.applyUpdate");
    }

    private void Dismiss()
    {
        _updateAvailable = false;
    }

    public async ValueTask DisposeAsync()
    {
        _selfRef?.Dispose();
        await ValueTask.CompletedTask;
    }
}
