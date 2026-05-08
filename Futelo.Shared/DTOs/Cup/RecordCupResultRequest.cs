namespace Futelo.Shared.DTOs.Cup;

public class RecordCupResultRequest
{
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string? WonOnPenaltiesId { get; set; }
}
