using Futelo.Server.Models;

namespace Futelo.Server.Repositories.Cup;

public class CupMatchResultData
{
    public int MatchId { get; init; }
    public int CupId { get; init; }
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
    public string? TieWinnerId { get; init; }
    public int? AdvanceToMatchId { get; init; }
    public bool AdvanceAsHome { get; init; }
    public int? AdvanceToLeg2MatchId { get; init; }
    public bool CupFinished { get; init; }
    public string? ChampionId { get; init; }
    public Dictionary<string, int> FinalCupPositions { get; init; } = [];
    public int? VideoGameId { get; init; }
    public int? HomeTeamId { get; init; }
    public int? AwayTeamId { get; init; }
}
