namespace Futelo.Shared.DTOs.Stats;

public class VaultRecordsResponse
{
    public RecordEntry? BestWinStreak { get; set; }
    public RecordEntry? BestUnbeatenStreak { get; set; }
    public RecordEntry? PeakElo { get; set; }
    public RecordEntry? BestEloGain { get; set; }
    public RecordEntry? LongestTop1Reign { get; set; }
    public RecordEntry? TotalMatchesAtTop1 { get; set; }
}

public class RecordEntry
{
    public string PlayerId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Value { get; set; }
}
