using Futelo.Shared.Enums;

namespace Futelo.Server.Models;

public class Cup
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string Name { get; set; } = "Cup";
    public bool IsHomeAndAway { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public CupSeedingMode SeedingMode { get; set; } = CupSeedingMode.SeasonElo;
    public bool AwayGoalRule { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.NotStarted;

    public string? ChampionId { get; set; }

    public Season Season { get; set; } = null!;
    public AppUser? Champion { get; set; }
    public ICollection<CupPlayer> Players { get; set; } = [];
    public ICollection<CupRound> Rounds { get; set; } = [];
}
