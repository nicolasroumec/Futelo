namespace Futelo.Server.Models;

public class EloHistory
{
    public int Id { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public int SeasonId { get; set; }
    public int EloBefore { get; set; }
    public int EloAfter { get; set; }
    public int EloChange { get; set; }
    public int RankBefore { get; set; }
    public int RankAfter { get; set; }
    public bool IsSeasonElo { get; set; }
    public DateTime CreatedAt { get; set; }

    public AppUser Player { get; set; } = null!;
    public Match Match { get; set; } = null!;
    public Season Season { get; set; } = null!;
}
