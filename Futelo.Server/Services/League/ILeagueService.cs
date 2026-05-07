namespace Futelo.Server.Services.League;

public interface ILeagueService
{
    Task GenerateFixtureAsync(int leagueId, string userId);
    Task RegenerateFixtureAsync(int leagueId, string userId);
}
