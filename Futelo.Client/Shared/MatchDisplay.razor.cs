using Futelo.Client.Services.Teams;
using Futelo.Client.Services.Users;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class MatchDisplay : ComponentBase
{
    [Inject] private AvatarDirectory Avatars { get; set; } = null!;
    [Inject] private ShieldDirectory Shields { get; set; } = null!;

    [Parameter] public string HomePlayerName { get; set; } = string.Empty;
    [Parameter] public string AwayPlayerName { get; set; } = string.Empty;
    [Parameter] public string? HomePlayerId { get; set; }
    [Parameter] public string? AwayPlayerId { get; set; }
    [Parameter] public string? HomeTeamName { get; set; }
    [Parameter] public string? AwayTeamName { get; set; }
    [Parameter] public int? HomeTeamId { get; set; }
    [Parameter] public int? AwayTeamId { get; set; }
    [Parameter] public string? VideoGameName { get; set; }
    [Parameter] public int? HomeScore { get; set; }
    [Parameter] public int? AwayScore { get; set; }
    [Parameter] public bool IsPlayed { get; set; }
    [Parameter] public bool HomePlayerMuted { get; set; }
    [Parameter] public bool AwayPlayerMuted { get; set; }
    [Parameter] public string? WonOnPenaltiesId { get; set; }
    [Parameter] public int? HomePenaltyScore { get; set; }
    [Parameter] public int? AwayPenaltyScore { get; set; }

    private bool HasGame => VideoGameName != null;
    private bool HasTeams => HomeTeamName != null || AwayTeamName != null;
}
