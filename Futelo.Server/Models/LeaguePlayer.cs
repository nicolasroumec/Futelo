namespace Futelo.Server.Models;

public class LeaguePlayer
{
    public int LeagueId { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public int? LeaguePosition { get; set; }

    public League League { get; set; } = null!;
    public AppUser Player { get; set; } = null!;
}
