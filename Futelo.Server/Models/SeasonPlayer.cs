namespace Futelo.Server.Models;

public class SeasonPlayer
{
    public int SeasonId { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public int SeasonElo { get; set; } = 1500;
    public int? TeamId { get; set; }

    public Season Season { get; set; } = null!;
    public AppUser Player { get; set; } = null!;
    public Team? Team { get; set; }
}
