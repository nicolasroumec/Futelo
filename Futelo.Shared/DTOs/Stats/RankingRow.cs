namespace Futelo.Shared.DTOs.Stats;

public class RankingRow
{
    public int Position { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int SeasonElo { get; set; }
    public int HistoricalElo { get; set; }
}
