using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class EmptyState
{
    [Parameter, EditorRequired] public string Message { get; set; } = string.Empty;
}
