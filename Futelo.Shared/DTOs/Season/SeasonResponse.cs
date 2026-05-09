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
    public string LeagueName { get; set; } = "League";
    public bool LeagueIsHomeAndAway { get; set; }
    public bool HasCup { get; set; }
    public int? CupId { get; set; }
    public string CupName { get; set; } = "Cup";
    public bool HasSuperCup { get; set; }
    public string SuperCupName { get; set; } = "SuperCup";
    public List<SeasonPlayerResponse> Players { get; set; } = [];
}
