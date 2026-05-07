namespace Futelo.Shared.DTOs.League;

public class RecordResultResponse
{
    public EloChangeResult Home { get; set; } = null!;
    public EloChangeResult Away { get; set; } = null!;
}
