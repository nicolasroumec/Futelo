using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class ErrorFallback
{
    [Inject] private NavigationManager Nav { get; set; } = default!;

    /// <summary>The exception captured by the surrounding <see cref="ErrorBoundary"/>.</summary>
    [Parameter] public Exception? Exception { get; set; }

    private void GoHome() => Nav.NavigateTo("/", forceLoad: true);

    private void Reload() => Nav.NavigateTo(Nav.Uri, forceLoad: true);
}
