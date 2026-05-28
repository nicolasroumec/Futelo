using Futelo.Server.Models;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.Enums;

namespace Futelo.Server.Helpers;

public static class StandingsCalculator
{
    public static List<StandingRow> Compute(
        List<Match> played,
        IEnumerable<LeaguePlayer> leaguePlayers,
        List<TiebreakerCriterion>? criteria = null,
        bool randomFallback = false)
    {
        var effectiveCriteria = criteria ?? League.DefaultCriteria();

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

        var result = new List<StandingRow>();
        foreach (var group in rows.GroupBy(r => r.Points).OrderByDescending(g => g.Key))
        {
            var tied = group.ToList();
            result.AddRange(tied.Count == 1 ? tied : BreakTie(tied, played, effectiveCriteria, randomFallback));
        }

        return result;
    }

    private static List<StandingRow> BreakTie(
        List<StandingRow> tied,
        List<Match> allPlayed,
        List<TiebreakerCriterion> criteria,
        bool randomFallback)
    {
        var tiedIds = tied.Select(r => r.PlayerId).ToHashSet();
        var h2hMatches = allPlayed
            .Where(m => tiedIds.Contains(m.HomePlayerId!) && tiedIds.Contains(m.AwayPlayerId!))
            .ToList();

        var h2h = tied.ToDictionary(r => r.PlayerId, r =>
        {
            var pid  = r.PlayerId;
            var home = h2hMatches.Where(m => m.HomePlayerId == pid).ToList();
            var away = h2hMatches.Where(m => m.AwayPlayerId == pid).ToList();

            int pts  = home.Sum(m => m.HomeScore > m.AwayScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0)
                     + away.Sum(m => m.AwayScore > m.HomeScore ? 3 : m.HomeScore == m.AwayScore ? 1 : 0);
            int wins = home.Count(m => m.HomeScore > m.AwayScore) + away.Count(m => m.AwayScore > m.HomeScore);
            int gf   = home.Sum(m => m.HomeScore ?? 0) + away.Sum(m => m.AwayScore ?? 0);
            int ga   = home.Sum(m => m.AwayScore ?? 0) + away.Sum(m => m.HomeScore ?? 0);

            return (Pts: pts, Wins: wins, GD: gf - ga, GF: gf);
        });

        IOrderedEnumerable<StandingRow>? ordered = null;

        foreach (var criterion in criteria)
        {
            if (ordered == null)
            {
                ordered = criterion switch
                {
                    TiebreakerCriterion.HeadToHeadPoints         => tied.OrderByDescending(r => h2h[r.PlayerId].Pts),
                    TiebreakerCriterion.HeadToHeadWins           => tied.OrderByDescending(r => h2h[r.PlayerId].Wins),
                    TiebreakerCriterion.HeadToHeadGoalDifference => tied.OrderByDescending(r => h2h[r.PlayerId].GD),
                    TiebreakerCriterion.HeadToHeadGoalsFor       => tied.OrderByDescending(r => h2h[r.PlayerId].GF),
                    TiebreakerCriterion.GoalDifference           => tied.OrderByDescending(r => r.GoalDifference),
                    TiebreakerCriterion.GoalsFor                 => tied.OrderByDescending(r => r.GoalsFor),
                    TiebreakerCriterion.Wins                     => tied.OrderByDescending(r => r.Won),
                    _                                            => tied.OrderBy(r => r.DisplayName, StringComparer.OrdinalIgnoreCase)
                };
            }
            else
            {
                ordered = criterion switch
                {
                    TiebreakerCriterion.HeadToHeadPoints         => ordered.ThenByDescending(r => h2h[r.PlayerId].Pts),
                    TiebreakerCriterion.HeadToHeadWins           => ordered.ThenByDescending(r => h2h[r.PlayerId].Wins),
                    TiebreakerCriterion.HeadToHeadGoalDifference => ordered.ThenByDescending(r => h2h[r.PlayerId].GD),
                    TiebreakerCriterion.HeadToHeadGoalsFor       => ordered.ThenByDescending(r => h2h[r.PlayerId].GF),
                    TiebreakerCriterion.GoalDifference           => ordered.ThenByDescending(r => r.GoalDifference),
                    TiebreakerCriterion.GoalsFor                 => ordered.ThenByDescending(r => r.GoalsFor),
                    TiebreakerCriterion.Wins                     => ordered.ThenByDescending(r => r.Won),
                    _                                            => ordered.ThenBy(r => r.DisplayName, StringComparer.OrdinalIgnoreCase)
                };
            }
        }

        var baseOrdered = ordered ?? tied.OrderBy(r => r.DisplayName, StringComparer.OrdinalIgnoreCase);

        if (randomFallback)
        {
            var randomKeys = tied.ToDictionary(r => r.PlayerId, _ => Random.Shared.Next());
            return baseOrdered.ThenBy(r => randomKeys[r.PlayerId]).ToList();
        }

        return baseOrdered.ThenBy(r => r.DisplayName, StringComparer.OrdinalIgnoreCase).ToList();
    }
}
