namespace Futelo.Shared.DTOs.Stats;

public class PlayerStatsResponse
{
    public string PlayerId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int EloRating { get; set; }
    public int CurrentStreak { get; set; }
    public int BestWinStreak { get; set; }
    public int BestUnbeatenStreak { get; set; }
    public List<TeamUsageRow> TopTeams { get; set; } = [];
    public List<VideoGameStatsRow> GameStats { get; set; } = [];
}

public class TeamUsageRow
{
    public string TeamName { get; set; } = string.Empty;
    public int TimesUsed { get; set; }
}
