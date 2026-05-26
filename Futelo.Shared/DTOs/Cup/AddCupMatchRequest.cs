namespace Futelo.Shared.DTOs.Cup;

public class AddCupMatchRequest
{
    public string HomePlayerId { get; set; } = string.Empty;
    public string AwayPlayerId { get; set; } = string.Empty;
    public int Leg { get; set; } = 1;
}
