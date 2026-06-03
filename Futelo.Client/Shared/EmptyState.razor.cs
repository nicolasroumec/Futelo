using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class EmptyState
{
    [Parameter, EditorRequired] public string Message { get; set; } = string.Empty;
    [Parameter] public string? Title { get; set; }
    [Parameter] public RenderFragment? Icon { get; set; }
    [Parameter] public string? ActionLabel { get; set; }
    [Parameter] public string? ActionHref { get; set; }
    [Parameter] public EventCallback OnAction { get; set; }
}
