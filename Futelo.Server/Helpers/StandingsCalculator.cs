using Futelo.Server.Models;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.Enums;

namespace Futelo.Server.Helpers;

public static class StandingsCalculator
{
    public static List<StandingRow> Compute(
        List<Match> played,
        IEnumerable<LeaguePlayer> leaguePlayers,
        TiebreakerRule tiebreaker = TiebreakerRule.GoalDifference)
    {
        var rows = leaguePlayers.Select(lp =>
        {
            string pid = lp.PlayerId;
            var home = played.Where(m => m.HomePlayerId == pid).ToList();
            var away = played.Where(m => m.AwayPlayerId == pid).ToList();

            int wins  = home.Count(m => m.HomeScore > m.AwayScore) + away.Count(m => m.AwayScore > m.HomeScore);
            int draws = home.Count(m => m.HomeScore == m.AwayScore) + away.Count(m => m.HomeScore == m.AwayScore);
            int p  = home.Count + away.Count;
            int gf = home.Sum(m => m.HomeScore ?? 0) + away.Sum(m => m.AwayScore ?? 0);
            int ga = home.Sum(m => m.AwayScore ?? 0) + away.Sum(m => m.HomeScore ?? 0);

            return new StandingRow
            {
                PlayerId       = pid,
                DisplayName    = lp.Player?.DisplayName ?? pid,
                Played         = p,
                Won            = wins,
                Drawn          = draws,
                Lost           = p - wins - draws,
                GoalsFor       = gf,
                GoalsAgainst   = ga,
                GoalDifference = gf - ga,
                Points         = wins * 3 + draws
            };
        }).ToList();

        // Group by points, then apply the chosen tiebreaker within each group.
        var result = new List<StandingRow>();
        foreach (var group in rows.GroupBy(r => r.Points).OrderByDescending(g => g.Key))
        {
            var tied = group.ToList();
            result.AddRange(tied.Count == 1 ? tied : BreakTie(tied, played, tiebreaker));
        }

        return result;
    }

    private static List<StandingRow> BreakTie(
        List<StandingRow> tied,
        List<Match> allPlayed,
        TiebreakerRule tiebreaker)
    {
        return tiebreaker switch
        {
            TiebreakerRule.HeadToHead                  => SortByH2H(tied, allPlayed, overallGdFallback: false),
            TiebreakerRule.HeadToHeadThenGoalDifference => SortByH2H(tied, allPlayed, overallGdFallback: true),
            _                                           => SortByGoalDifference(tied)
        };
    }

    private static List<StandingRow> SortByGoalDifference(List<StandingRow> tied) =>
        tied.OrderByDescending(r => r.GoalDifference)
            .ThenByDescending(r => r.GoalsFor)
            .ThenBy(r => r.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();

    // Computes a H2H sub-table among the tied players.
    // overallGdFallback = true  → after H2H pts, use overall GD (HeadToHeadThenGoalDifference)
    // overallGdFallback = false → after H2H pts, use H2H GD   (HeadToHead)
    private static List<StandingRow> SortByH2H(
        List<StandingRow> tied,
        List<Match> allPlayed,
        bool overallGdFallback)
    {
        var tiedIds   = tied.Select(r => r.PlayerId).ToHashSet();
        var h2hMatches = allPlayed
            .Where(m => tiedIds.Contains(m.HomePlayerId!) && tiedIds.Contains(m.AwayPlayerId!))
            .ToList();

        var h2h = tied.ToDictionary(r => r.PlayerId, r =>
        {
            var pid  = r.PlayerId;
            var home = h2hMatches.Where(m => m.HomePlayerId == pid).ToList();
            var away = h2hMatches.Where(m => m.AwayPlayerId == pid).ToList();

            int pts = home.Sum(m => m.HomeScore > m.AwayScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0)
                    + away.Sum(m => m.AwayScore > m.HomeScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0);
            int gf  = home.Sum(m => m.HomeScore ?? 0) + away.Sum(m => m.AwayScore ?? 0);
            int ga  = home.Sum(m => m.AwayScore ?? 0) + away.Sum(m => m.HomeScore ?? 0);

            return (Pts: pts, GD: gf - ga, GF: gf);
        });

        if (overallGdFallback)
        {
            // H2H pts → overall GD → overall GF → alphabetical
            return tied
                .OrderByDescending(r => h2h[r.PlayerId].Pts)
                .ThenByDescending(r => r.GoalDifference)
                .ThenByDescending(r => r.GoalsFor)
                .ThenBy(r => r.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        else
        {
            // H2H pts → H2H GD → H2H GF → overall GD → overall GF → alphabetical
            return tied
                .OrderByDescending(r => h2h[r.PlayerId].Pts)
                .ThenByDescending(r => h2h[r.PlayerId].GD)
                .ThenByDescending(r => h2h[r.PlayerId].GF)
                .ThenByDescending(r => r.GoalDifference)
                .ThenByDescending(r => r.GoalsFor)
                .ThenBy(r => r.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
