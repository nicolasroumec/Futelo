using Futelo.Server.Models;
using Futelo.Shared.Enums;

namespace Futelo.Server.Helpers;

public static class FixtureGenerator
{
    public static List<Match> Build(List<string> playerIds, int leagueId, bool isHomeAndAway)
    {
        var slots = new List<string?>(playerIds.Cast<string?>());
        if (slots.Count % 2 != 0)
            slots.Add(null);

        int n = slots.Count;
        int totalRounds = n - 1;
        var matches = new List<Match>();

        for (int round = 0; round < totalRounds; round++)
        {
            int matchday = round + 1;
            for (int i = 0; i < n / 2; i++)
            {
                string? home = slots[i];
                string? away = slots[n - 1 - i];

                if (home != null && away != null)
                {
                    matches.Add(new Match
                    {
                        LeagueId = leagueId,
                        HomePlayerId = home,
                        AwayPlayerId = away,
                        Status = MatchStatus.Pending,
                        Leg = matchday
                    });
                }
            }

            var last = slots[n - 1];
            for (int i = n - 1; i > 1; i--)
                slots[i] = slots[i - 1];
            slots[1] = last;
        }

        if (isHomeAndAway)
        {
            var returnMatches = matches
                .Select(m => new Match
                {
                    LeagueId = leagueId,
                    HomePlayerId = m.AwayPlayerId,
                    AwayPlayerId = m.HomePlayerId,
                    Status = MatchStatus.Pending,
                    Leg = m.Leg + totalRounds
                })
                .ToList();
            matches.AddRange(returnMatches);
        }

        return matches;
    }
}
