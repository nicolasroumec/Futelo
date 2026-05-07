namespace Futelo.Shared.DTOs.Season;

public class SeasonResponse
{
    public int Id { get; set; }
    public int VaultId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool HasLeague { get; set; }
    public int? LeagueId { get; set; }
    public bool HasCup { get; set; }
    public bool HasSuperCup { get; set; }
    public List<SeasonPlayerResponse> Players { get; set; } = [];
}
