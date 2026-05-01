namespace Futelo.Server.Models;

public class SuperCup
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public bool IsHomeAndAway { get; set; }
    public string? Player1Id { get; set; }
    public string? Player2Id { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.NotStarted;

    public Season Season { get; set; } = null!;
    public AppUser? Player1 { get; set; }
    public AppUser? Player2 { get; set; }
    public ICollection<Match> Matches { get; set; } = [];
}
