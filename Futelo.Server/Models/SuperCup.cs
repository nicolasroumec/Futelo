using Futelo.Shared.Enums;

namespace Futelo.Server.Models;

public class SuperCup
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string Name { get; set; } = "SuperCup";
    public bool IsHomeAndAway { get; set; }
    public string? Player1Id { get; set; }
    public string? Player2Id { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.NotStarted;

    public string? ChampionId { get; set; }

    public Season Season { get; set; } = null!;
    public AppUser? Player1 { get; set; }
    public AppUser? Player2 { get; set; }
    public AppUser? Champion { get; set; }
    public ICollection<Match> Matches { get; set; } = [];
}
