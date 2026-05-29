using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class TeamShield : ComponentBase
{
    [Parameter] public string Name { get; set; } = string.Empty;
    [Parameter] public int Size { get; set; } = 32;
    [Parameter] public string? ShieldUrl { get; set; }

    private bool _imgFailed;
    private string? _lastUrl;

    protected override void OnParametersSet()
    {
        if (_lastUrl != ShieldUrl)
        {
            _imgFailed = false;
            _lastUrl = ShieldUrl;
        }
    }

    private void OnImgError() => _imgFailed = true;

    private string Abbr => Name.Length >= 2 ? Name[..2].ToUpper() : Name.ToUpper();

    private string BgColor => GetColor(Name);

    private static readonly string[] Palette =
    [
        "#22C55E", "#3B82F6", "#F59E0B", "#EF4444",
        "#8B5CF6", "#EC4899", "#14B8A6", "#F97316"
    ];

    private static string GetColor(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Palette[0];
        var hash = name.Aggregate(0, (acc, c) => acc + c * 3);
        return Palette[Math.Abs(hash) % Palette.Length];
    }
}
