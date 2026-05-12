using Futelo.Shared.Enums;

namespace Futelo.Shared.DTOs.Stats;

public class RecentFormEntry
{
    public MatchResult Result { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public string OpponentDisplayName { get; set; } = string.Empty;
    public DateTime? PlayedAt { get; set; }
}
