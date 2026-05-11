namespace Futelo.Shared.DTOs.Season;

public class SeasonPlayerResponse
{
    public string PlayerId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int SeasonElo { get; set; }
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
}
