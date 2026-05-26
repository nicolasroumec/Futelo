using Futelo.Shared.Enums;

namespace Futelo.Server.Models;

public class PlayerAchievement
{
    public int Id { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public int VaultId { get; set; }
    public AchievementType Type { get; set; }
    public DateTime UnlockedAt { get; set; }
    public int? SeasonId { get; set; }

    public AppUser Player { get; set; } = null!;
}
