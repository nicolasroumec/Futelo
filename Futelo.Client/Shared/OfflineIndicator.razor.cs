using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Futelo.Client.Shared;

public partial class OfflineIndicator : IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private bool _isOnline = true;
    private DotNetObjectReference<OfflineIndicator>? _selfRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        _selfRef = DotNetObjectReference.Create(this);
        _isOnline = await JS.InvokeAsync<bool>("offlineIndicator.register", _selfRef);
        StateHasChanged();
    }

    [JSInvokable]
    public void OnOnline()
    {
        _isOnline = true;
        StateHasChanged();
    }

    [JSInvokable]
    public void OnOffline()
    {
        _isOnline = false;
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        _selfRef?.Dispose();
        await ValueTask.CompletedTask;
    }
}
