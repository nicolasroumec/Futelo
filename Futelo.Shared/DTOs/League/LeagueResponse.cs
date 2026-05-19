namespace Futelo.Shared.DTOs.League;

public class LeagueResponse
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Name { get; set; } = "League";
    public bool IsHomeAndAway { get; set; }
    public string? ChampionId { get; set; }
    public string? ChampionName { get; set; }
    public bool CanEdit { get; set; }
    public List<MatchResponse> Matches { get; set; } = [];
    public List<StandingRow> Standings { get; set; } = [];
}
