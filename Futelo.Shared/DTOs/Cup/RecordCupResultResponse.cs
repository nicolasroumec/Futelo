namespace Futelo.Shared.DTOs.Cup;

public class RecordCupResultResponse
{
    public CupEloChangeResult Home { get; set; } = null!;
    public CupEloChangeResult Away { get; set; } = null!;
    public bool TieDecided { get; set; }
    public string? TieWinnerId { get; set; }
}
