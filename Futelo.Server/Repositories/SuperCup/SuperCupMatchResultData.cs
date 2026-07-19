using Futelo.Server.Models;

namespace Futelo.Server.Repositories.SuperCup;

public class SuperCupMatchResultData
{
    public int MatchId { get; init; }
    public int SuperCupId { get; init; }
    public int SeasonId { get; init; }
    public int VaultId { get; init; }
    public int HomeScore { get; init; }
    public int AwayScore { get; init; }
    public string? WonOnPenaltiesId { get; init; }
    public int? HomePenaltyScore { get; init; }
    public int? AwayPenaltyScore { get; init; }
    public string HomePlayerId { get; init; } = string.Empty;
    public int HomeNewSeasonElo { get; init; }
    public int HomeNewHistoricalElo { get; init; }
    public string AwayPlayerId { get; init; } = string.Empty;
    public int AwayNewSeasonElo { get; init; }
    public int AwayNewHistoricalElo { get; init; }
    public List<EloHistory> EloHistories { get; init; } = [];
    public bool Finished { get; init; }
    public string? ChampionId { get; init; }
    public int? VideoGameId { get; init; }
    public int? HomeTeamId { get; init; }
    public int? AwayTeamId { get; init; }
}
