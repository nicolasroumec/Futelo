using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class PlayerAvatar
{
    [Parameter] public string Name { get; set; } = string.Empty;
    [Parameter] public int Size { get; set; } = 32;

    private string Initials => GetInitials(Name);
    private string BgColor => GetColor(Name);

    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
            : name.Length >= 2 ? name[..2].ToUpper() : name.ToUpper();
    }

    private static readonly string[] Palette =
    [
        "#22C55E", "#3B82F6", "#F59E0B", "#EF4444",
        "#8B5CF6", "#EC4899", "#14B8A6", "#F97316"
    ];

    private static string GetColor(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Palette[0];
        var hash = name.Aggregate(0, (acc, c) => acc + c);
        return Palette[Math.Abs(hash) % Palette.Length];
    }
}
