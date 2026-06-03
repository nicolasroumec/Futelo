namespace Futelo.Shared.DTOs.Stats;

public class AllTimeStandingRow
{
    public int Position { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points { get; set; }
    public double PointsAverage => Played > 0 ? (double)Points / Played : 0;
}
