namespace Futelo.Shared.DTOs.SuperCup;

public class SuperCupMatchResponse
{
    public int Id { get; set; }
    public string HomePlayerId { get; set; } = string.Empty;
    public string HomePlayerName { get; set; } = string.Empty;
    public string AwayPlayerId { get; set; } = string.Empty;
    public string AwayPlayerName { get; set; } = string.Empty;
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? WonOnPenaltiesId { get; set; }
    public int? HomePenaltyScore { get; set; }
    public int? AwayPenaltyScore { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Leg { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PlayedAt { get; set; }
    public int? HomeTeamId { get; set; }
    public string? HomeTeamName { get; set; }
    public int? AwayTeamId { get; set; }
    public string? AwayTeamName { get; set; }
    public int? VideoGameId { get; set; }
    public string? VideoGameName { get; set; }
    public bool IsLastPlayed { get; set; }
}
