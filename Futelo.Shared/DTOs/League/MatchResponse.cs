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
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PlayedAt { get; set; }
    public int? HomeTeamId { get; set; }
    public string? HomeTeamName { get; set; }
    public int? AwayTeamId { get; set; }
    public string? AwayTeamName { get; set; }
    public int? VideoGameId { get; set; }
    public string? VideoGameName { get; set; }
}
