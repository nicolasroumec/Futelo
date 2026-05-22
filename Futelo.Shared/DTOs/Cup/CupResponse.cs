using Futelo.Shared.Enums;

namespace Futelo.Shared.DTOs.Cup;

public class CupResponse
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Name { get; set; } = "Cup";
    public bool IsHomeAndAway { get; set; }
    public bool IsManual { get; set; }
    public CupSeedingMode SeedingMode { get; set; }
    public bool AwayGoalRule { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ChampionId { get; set; }
    public string? ChampionName { get; set; }
    public bool CanEdit { get; set; }
    public List<PlayerSummary> SeasonPlayers { get; set; } = [];
    public List<CupRoundResponse> Rounds { get; set; } = [];
}
