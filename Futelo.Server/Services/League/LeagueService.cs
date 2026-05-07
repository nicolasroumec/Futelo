using Futelo.Server.Repositories.League;

namespace Futelo.Server.Services.League;

public class LeagueService(ILeagueRepository leagueRepository) : ILeagueService
{
    public Task GenerateFixtureAsync(int leagueId, string userId)
        => throw new NotImplementedException();

    public Task RegenerateFixtureAsync(int leagueId, string userId)
        => throw new NotImplementedException();
}
