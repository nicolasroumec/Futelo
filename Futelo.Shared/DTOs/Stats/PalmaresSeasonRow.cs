namespace Futelo.Shared.DTOs.Stats;

public class PalmaresSeasonRow
{
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? LeagueChampion { get; set; }
    public string? CupChampion { get; set; }
    public string? SuperCupChampion { get; set; }
}
