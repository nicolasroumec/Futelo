namespace Futelo.Shared.DTOs.League;

public class AddLeagueMatchRequest
{
    public int Matchday { get; set; }
    public string HomePlayerId { get; set; } = string.Empty;
    public string AwayPlayerId { get; set; } = string.Empty;
}
