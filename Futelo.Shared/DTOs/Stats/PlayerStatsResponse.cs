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
    public List<TeamUsageRow> TopTeams { get; set; } = [];
    public List<VideoGameUsageRow> TopGames { get; set; } = [];
}

public class TeamUsageRow
{
    public string TeamName { get; set; } = string.Empty;
    public int TimesUsed { get; set; }
}

public class VideoGameUsageRow
{
    public string VideoGameName { get; set; } = string.Empty;
    public int TimesPlayed { get; set; }
}
