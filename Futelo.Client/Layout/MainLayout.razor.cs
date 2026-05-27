using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Layout;

public partial class MainLayout : LayoutComponentBase
{
    protected string AppVersion { get; } =
        typeof(MainLayout).Assembly.GetName().Version?.ToString(3) ?? "–";
}
