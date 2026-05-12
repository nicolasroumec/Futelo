using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class SkeletonList
{
    [Parameter] public int Items { get; set; } = 4;
    [Parameter] public string MaxWidth { get; set; } = "400px";
}
