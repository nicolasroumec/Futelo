namespace Futelo.Shared.DTOs.Cup;

public class CupRoundResponse
{
    public int Id { get; set; }
    public int RoundNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<CupMatchResponse> Matches { get; set; } = [];
}
