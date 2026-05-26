namespace Futelo.Shared.DTOs.Stats;

public class GlobalEloHistoryResponse
{
    public List<GlobalEloHistoryPoint> Points { get; set; } = [];
    public List<EloSeasonAnnotation> Seasons { get; set; } = [];
}

public class GlobalEloHistoryPoint
{
    public DateTime Date { get; set; }
    public int Elo { get; set; }
    public string CompetitionType { get; set; } = "";
    public string SeasonName { get; set; } = "";
}

public class EloSeasonAnnotation
{
    public string Name { get; set; } = "";
    public int FirstPointIndex { get; set; }
}
