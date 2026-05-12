using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class SkeletonTable
{
    [Parameter] public int Rows { get; set; } = 5;
    [Parameter] public int Columns { get; set; } = 3;
}
