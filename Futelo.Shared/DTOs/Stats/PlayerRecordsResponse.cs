namespace Futelo.Shared.DTOs.Stats;

public class PlayerRecordsResponse
{
    public MatchRecordEntry? BestWin { get; set; }
    public MatchRecordEntry? WorstDefeat { get; set; }
    public int PeakElo { get; set; }
    public int BestEloGain { get; set; }
}

public class MatchRecordEntry
{
    public string OpponentName { get; set; } = string.Empty;
    public int MyScore { get; set; }
    public int OpponentScore { get; set; }
    public DateTime? PlayedAt { get; set; }
}
