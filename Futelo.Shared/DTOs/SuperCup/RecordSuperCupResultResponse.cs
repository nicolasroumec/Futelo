namespace Futelo.Shared.DTOs.SuperCup;

public class RecordSuperCupResultResponse
{
    public SuperCupEloChangeResult Home { get; set; } = null!;
    public SuperCupEloChangeResult Away { get; set; } = null!;
    public bool Finished { get; set; }
    public string? ChampionId { get; set; }
}
