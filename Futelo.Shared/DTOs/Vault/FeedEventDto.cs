namespace Futelo.Shared.DTOs.Vault;

public class FeedEventDto
{
    public int MatchId { get; set; }
    public DateTime OccurredAt { get; set; }
    public string CompetitionType { get; set; } = "";
    public string CompetitionName { get; set; } = "";
    public string SeasonName { get; set; } = "";

    public string HomePlayerId { get; set; } = "";
    public string HomePlayerName { get; set; } = "";
    public string AwayPlayerId { get; set; } = "";
    public string AwayPlayerName { get; set; } = "";
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? WonOnPenaltiesId { get; set; }
    public int? HomePenaltyScore { get; set; }
    public int? AwayPenaltyScore { get; set; }
    public string? HomeTeamName { get; set; }
    public string? AwayTeamName { get; set; }
    public int? HomeTeamId { get; set; }
    public int? AwayTeamId { get; set; }
    public string? VideoGameName { get; set; }

    public int HomeEloChange { get; set; }
    public int AwayEloChange { get; set; }
    public int HomeNewElo { get; set; }
    public int AwayNewElo { get; set; }
    public int HomeRankBefore { get; set; }
    public int HomeRankAfter { get; set; }
    public int AwayRankBefore { get; set; }
    public int AwayRankAfter { get; set; }
}
