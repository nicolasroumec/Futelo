using Futelo.Shared.Enums;

namespace Futelo.Server.Models;

public class Season
{
    public int Id { get; set; }
    public int VaultId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public SeasonStatus Status { get; set; } = SeasonStatus.Draft;
    public int? VideoGameId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public Vault Vault { get; set; } = null!;
    public VideoGame? VideoGame { get; set; }
    public ICollection<SeasonPlayer> Players { get; set; } = [];
    public League? League { get; set; }
    public Cup? Cup { get; set; }
    public SuperCup? SuperCup { get; set; }
}
