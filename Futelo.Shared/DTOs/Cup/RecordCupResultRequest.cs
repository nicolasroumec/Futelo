namespace Futelo.Shared.DTOs.Cup;

public class RecordCupResultRequest
{
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string? WonOnPenaltiesId { get; set; }
    public int? HomePenaltyScore { get; set; }
    public int? AwayPenaltyScore { get; set; }
}
