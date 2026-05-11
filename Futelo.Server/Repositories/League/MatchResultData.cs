using Futelo.Server.Models;

namespace Futelo.Server.Repositories.League;

public class MatchResultData
{
    public int MatchId { get; init; }
    public int HomeScore { get; init; }
    public int AwayScore { get; init; }
    public int LeagueId { get; init; }
    public int SeasonId { get; init; }
    public string HomePlayerId { get; init; } = string.Empty;
    public int HomeNewSeasonElo { get; init; }
    public int HomeNewHistoricalElo { get; init; }
    public string AwayPlayerId { get; init; } = string.Empty;
    public int AwayNewSeasonElo { get; init; }
    public int AwayNewHistoricalElo { get; init; }
    public List<EloHistory> EloHistories { get; init; } = [];
    public bool LeagueFinished { get; init; }
    public string? ChampionId { get; init; }
    public Dictionary<string, int> FinalLeaguePositions { get; init; } = [];
    public int? VideoGameId { get; init; }
    public int? HomeTeamId { get; init; }
    public int? AwayTeamId { get; init; }
}
