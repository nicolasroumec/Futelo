namespace Futelo.Shared.DTOs.Season;

public class SeasonRecapResponse
{
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public int Year { get; set; }

    public string? LeagueChampion { get; set; }
    public string? CupChampion { get; set; }
    public string? SuperCupChampion { get; set; }

    public string? TopScorerName { get; set; }
    public int TopScorerGoals { get; set; }

    public string? MostImprovedName { get; set; }
    public int MostImprovedEloGain { get; set; }

    public string? BiggestWinHome { get; set; }
    public string? BiggestWinAway { get; set; }
    public int BiggestWinHomeScore { get; set; }
    public int BiggestWinAwayScore { get; set; }

    public string? LongestStreakPlayer { get; set; }
    public int LongestStreak { get; set; }

    public int TotalMatches { get; set; }
    public int TotalGoals { get; set; }
}
