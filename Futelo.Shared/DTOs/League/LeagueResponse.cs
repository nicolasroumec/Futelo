namespace Futelo.Shared.DTOs.League;

public class LeagueResponse
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsHomeAndAway { get; set; }
    public List<MatchResponse> Matches { get; set; } = [];
    public List<StandingRow> Standings { get; set; } = [];
}
