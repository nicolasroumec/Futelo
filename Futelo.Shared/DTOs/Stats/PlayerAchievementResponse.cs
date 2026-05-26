using Futelo.Shared.Enums;

namespace Futelo.Shared.DTOs.Stats;

public class PlayerAchievementResponse
{
    public AchievementType Type { get; set; }
    public DateTime UnlockedAt { get; set; }
    public int? SeasonId { get; set; }
}
