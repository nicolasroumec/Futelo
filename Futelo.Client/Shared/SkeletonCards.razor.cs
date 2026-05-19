using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class SkeletonCards
{
    [Parameter] public int Count { get; set; } = 3;
    [Parameter] public int Columns { get; set; } = 3;
}
