namespace Futelo.Shared.DTOs.Season;

public class ConfigureSeasonRequest
{
    public bool HasLeague { get; set; }
    public string LeagueName { get; set; } = "League";
    public bool LeagueIsHomeAndAway { get; set; }
    public bool HasCup { get; set; }
    public string CupName { get; set; } = "Cup";
    public bool HasSuperCup { get; set; }
    public string SuperCupName { get; set; } = "SuperCup";
    public List<string> PlayerIds { get; set; } = [];
}
