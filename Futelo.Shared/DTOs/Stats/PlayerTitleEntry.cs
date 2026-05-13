namespace Futelo.Shared.DTOs.Stats;

public class PlayerTitleEntry
{
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Competition { get; set; } = string.Empty; // "League", "Cup", "SuperCup"
}
