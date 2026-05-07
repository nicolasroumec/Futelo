namespace Futelo.Shared.DTOs.League;

public class MatchResponse
{
    public int Id { get; set; }
    public string HomePlayerId { get; set; } = string.Empty;
    public string HomePlayerName { get; set; } = string.Empty;
    public string AwayPlayerId { get; set; } = string.Empty;
    public string AwayPlayerName { get; set; } = string.Empty;
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Matchday { get; set; }
    public DateTime? PlayedAt { get; set; }
}
