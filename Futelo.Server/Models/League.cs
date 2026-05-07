using Futelo.Shared.Enums;

namespace Futelo.Server.Models;

public class League
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string Name { get; set; } = "League";
    public bool IsHomeAndAway { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.NotStarted;

    public string? ChampionId { get; set; }

    public Season Season { get; set; } = null!;
    public AppUser? Champion { get; set; }
    public ICollection<LeaguePlayer> Players { get; set; } = [];
    public ICollection<Match> Matches { get; set; } = [];
}
