namespace Futelo.Server.Models;

public class League
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public bool IsHomeAndAway { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.NotStarted;

    public Season Season { get; set; } = null!;
    public ICollection<LeaguePlayer> Players { get; set; } = [];
    public ICollection<Match> Matches { get; set; } = [];
}
