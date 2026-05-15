using Futelo.Server.Models;
using Futelo.Shared.DTOs.League;

namespace Futelo.Server.Helpers;

public static class StandingsCalculator
{
    public static List<StandingRow> Compute(List<Match> played, IEnumerable<LeaguePlayer> leaguePlayers)
    {
        var rows = leaguePlayers.Select(lp =>
        {
            string pid = lp.PlayerId;
            var home = played.Where(m => m.HomePlayerId == pid).ToList();
            var away = played.Where(m => m.AwayPlayerId == pid).ToList();

            int wins = home.Count(m => m.HomeScore > m.AwayScore) + away.Count(m => m.AwayScore > m.HomeScore);
            int draws = home.Count(m => m.HomeScore == m.AwayScore) + away.Count(m => m.HomeScore == m.AwayScore);
            int p = home.Count + away.Count;
            int gf = home.Sum(m => m.HomeScore ?? 0) + away.Sum(m => m.AwayScore ?? 0);
            int ga = home.Sum(m => m.AwayScore ?? 0) + away.Sum(m => m.HomeScore ?? 0);

            return new StandingRow
            {
                PlayerId = pid,
                DisplayName = lp.Player?.DisplayName ?? pid,
                Played = p,
                Won = wins,
                Drawn = draws,
                Lost = p - wins - draws,
                GoalsFor = gf,
                GoalsAgainst = ga,
                GoalDifference = gf - ga,
                Points = wins * 3 + draws
            };
        }).ToList();

        rows.Sort((a, b) =>
        {
            if (a.Points != b.Points) return b.Points.CompareTo(a.Points);
            if (a.GoalDifference != b.GoalDifference) return b.GoalDifference.CompareTo(a.GoalDifference);
            if (a.GoalsFor != b.GoalsFor) return b.GoalsFor.CompareTo(a.GoalsFor);

            var h2h = played.Where(m =>
                (m.HomePlayerId == a.PlayerId && m.AwayPlayerId == b.PlayerId) ||
                (m.HomePlayerId == b.PlayerId && m.AwayPlayerId == a.PlayerId)).ToList();

            if (h2h.Count > 0)
            {
                int aPts = h2h.Sum(m => m.HomePlayerId == a.PlayerId
                    ? (m.HomeScore > m.AwayScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0)
                    : (m.AwayScore > m.HomeScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0));
                int bPts = h2h.Sum(m => m.HomePlayerId == b.PlayerId
                    ? (m.HomeScore > m.AwayScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0)
                    : (m.AwayScore > m.HomeScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0));
                if (aPts != bPts) return bPts.CompareTo(aPts);
            }

            return string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase);
        });

        return rows;
    }
}
