namespace Futelo.Shared.DTOs.Stats;

public class ScorerRow
{
    public int Position { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Goals { get; set; }
}
