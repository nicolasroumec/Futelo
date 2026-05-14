namespace Futelo.Shared.DTOs.SuperCup;

public class SuperCupResponse
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string Name { get; set; } = "SuperCup";
    public string Status { get; set; } = string.Empty;
    public bool IsHomeAndAway { get; set; }
    public string? Player1Id { get; set; }
    public string? Player1Name { get; set; }
    public string? Player2Id { get; set; }
    public string? Player2Name { get; set; }
    public string? ChampionId { get; set; }
    public string? ChampionName { get; set; }
    public bool CanEdit { get; set; }
    public List<SuperCupMatchResponse> Matches { get; set; } = [];
}
