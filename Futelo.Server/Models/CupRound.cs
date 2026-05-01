namespace Futelo.Server.Models;

public class CupRound
{
    public int Id { get; set; }
    public int CupId { get; set; }
    public int RoundNumber { get; set; }
    public string Name { get; set; } = string.Empty;

    public Cup Cup { get; set; } = null!;
    public ICollection<Match> Matches { get; set; } = [];
}
