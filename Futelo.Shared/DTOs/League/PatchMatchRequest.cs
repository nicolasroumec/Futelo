namespace Futelo.Shared.DTOs.League;

public class PatchMatchRequest
{
    public int? HomeTeamId { get; set; }
    public int? AwayTeamId { get; set; }
    public int? VideoGameId { get; set; }
    public DateTime? ScheduledDate { get; set; }
}
