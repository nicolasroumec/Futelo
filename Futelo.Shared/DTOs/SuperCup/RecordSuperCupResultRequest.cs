namespace Futelo.Shared.DTOs.SuperCup;

public class RecordSuperCupResultRequest
{
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string? WonOnPenaltiesId { get; set; }
    public int? HomePenaltyScore { get; set; }
    public int? AwayPenaltyScore { get; set; }
}
