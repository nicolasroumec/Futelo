namespace Futelo.Shared.DTOs.Stats;

public class TopScoringMatchResponse
{
    public string HomePlayerDisplayName { get; set; } = string.Empty;
    public string AwayPlayerDisplayName { get; set; } = string.Empty;
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public int TotalGoals { get; set; }
    public DateTime? PlayedAt { get; set; }
    public string SeasonName { get; set; } = string.Empty;
}
