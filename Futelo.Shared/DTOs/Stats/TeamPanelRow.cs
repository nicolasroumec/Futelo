namespace Futelo.Shared.DTOs.Stats;

public class TeamPanelRow
{
    public string TeamName { get; set; } = string.Empty;
    public int TotalUsed { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public List<PlayerTeamUsageRow> Players { get; set; } = [];
}

public class PlayerTeamUsageRow
{
    public string PlayerId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int TimesUsed { get; set; }
}
