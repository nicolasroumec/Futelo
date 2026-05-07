namespace Futelo.Shared.DTOs.Season;

public class ConfigureSeasonRequest
{
    public bool HasLeague { get; set; }
    public bool LeagueIsHomeAndAway { get; set; }
    public bool HasCup { get; set; }
    public bool HasSuperCup { get; set; }
    public List<string> PlayerIds { get; set; } = [];
}
