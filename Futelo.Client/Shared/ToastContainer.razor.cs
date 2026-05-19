using Futelo.Client.Services.Toast;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class ToastContainer : IDisposable
{
    [Inject] private IToastService ToastService { get; set; } = null!;

    private readonly List<ToastEntry> _toasts = [];

    protected override void OnInitialized()
    {
        ToastService.OnShow += HandleShow;
    }

    private void HandleShow(ToastMessage message)
    {
        var entry = new ToastEntry(message);
        InvokeAsync(async () =>
        {
            _toasts.Add(entry);
            StateHasChanged();

            await Task.Delay(message.DurationMs);

            entry.Exiting = true;
            StateHasChanged();

            await Task.Delay(300);

            _toasts.Remove(entry);
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        ToastService.OnShow -= HandleShow;
    }

    private sealed class ToastEntry(ToastMessage message)
    {
        public ToastMessage Message { get; } = message;
        public bool Exiting { get; set; }

        public string CssClass => Message.Type switch
        {
            ToastType.Error   => "error",
            ToastType.Warning => "warning",
            ToastType.Info    => "info",
            _                 => "success"
        };

        public string Icon => Message.Type switch
        {
            ToastType.Error   => "✕",
            ToastType.Warning => "⚠",
            ToastType.Info    => "ℹ",
            _                 => "✓"
        };
    }
}
