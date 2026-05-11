namespace Futelo.Shared.DTOs.Stats;

public class HeadToHeadResponse
{
    public string Player1Id { get; set; } = string.Empty;
    public string Player1DisplayName { get; set; } = string.Empty;
    public string Player2Id { get; set; } = string.Empty;
    public string Player2DisplayName { get; set; } = string.Empty;
    public int Played { get; set; }
    public int Player1Wins { get; set; }
    public int Draws { get; set; }
    public int Player2Wins { get; set; }
    public List<H2HMatchRow> Matches { get; set; } = [];
}

public class H2HMatchRow
{
    public int MatchId { get; set; }
    public string HomePlayerId { get; set; } = string.Empty;
    public string HomePlayerDisplayName { get; set; } = string.Empty;
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string AwayPlayerId { get; set; } = string.Empty;
    public string AwayPlayerDisplayName { get; set; } = string.Empty;
    public string? VideoGameName { get; set; }
    public DateTime? PlayedAt { get; set; }
}
