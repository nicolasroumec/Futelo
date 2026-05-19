namespace Futelo.Shared.DTOs.Vault;

public class RecentMatchResponse
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
    public string? HomeTeamName { get; set; }
    public string? AwayTeamName { get; set; }
    public string? VideoGameName { get; set; }
    public DateTime? PlayedAt { get; set; }
    public string CompetitionType { get; set; } = string.Empty;
    public string CompetitionName { get; set; } = string.Empty;
    public string SeasonName { get; set; } = string.Empty;
}
