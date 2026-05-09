namespace Futelo.Shared.DTOs.SuperCup;

public class SuperCupEloChangeResult
{
    public string PlayerId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int EloBefore { get; set; }
    public int EloAfter { get; set; }
    public int EloChange { get; set; }
    public int RankBefore { get; set; }
    public int RankAfter { get; set; }
}
