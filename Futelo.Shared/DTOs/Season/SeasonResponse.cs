using Futelo.Shared.DTOs.League;

namespace Futelo.Shared.DTOs.Season;

public class SeasonRecentMatchRow
{
    public string HomePlayerName { get; set; } = string.Empty;
    public string AwayPlayerName { get; set; } = string.Empty;
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string Competition { get; set; } = string.Empty;
}

public class SeasonResponse
{
    public int Id { get; set; }
    public int VaultId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool HasLeague { get; set; }
    public int? LeagueId { get; set; }
    public string LeagueName { get; set; } = "League";
    public bool LeagueIsHomeAndAway { get; set; }
    public string? LeagueStatus { get; set; }
    public bool HasCup { get; set; }
    public int? CupId { get; set; }
    public string CupName { get; set; } = "Cup";
    public string? CupStatus { get; set; }
    public bool HasSuperCup { get; set; }
    public int? SuperCupId { get; set; }
    public string SuperCupName { get; set; } = "SuperCup";
    public string? SuperCupStatus { get; set; }
    public int? VideoGameId { get; set; }
    public string? VideoGameName { get; set; }
    public List<SeasonPlayerResponse> Players { get; set; } = [];
    public List<SeasonRecentMatchRow> RecentMatches { get; set; } = [];
    public List<StandingRow> TopStandings { get; set; } = [];
}
