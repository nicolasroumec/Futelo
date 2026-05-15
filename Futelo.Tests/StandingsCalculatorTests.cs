using Futelo.Server.Helpers;
using Futelo.Server.Models;
using Futelo.Shared.Enums;

namespace Futelo.Tests;

public class StandingsCalculatorTests
{
    private static LeaguePlayer Player(string id, string name) => new()
    {
        PlayerId = id,
        Player = new AppUser { Id = id, DisplayName = name }
    };

    private static Match Played(string home, string away, int homeScore, int awayScore) => new()
    {
        HomePlayerId = home,
        AwayPlayerId = away,
        HomeScore = homeScore,
        AwayScore = awayScore,
        Status = MatchStatus.Played
    };

    [Fact]
    public void Compute_NoMatchesPlayed_AllZeros()
    {
        var players = new[] { Player("A", "Alice"), Player("B", "Bob") };
        var rows = StandingsCalculator.Compute([], players);

        Assert.All(rows, r => Assert.Equal(0, r.Points));
        Assert.All(rows, r => Assert.Equal(0, r.Played));
    }

    [Fact]
    public void Compute_Win_Awards3Points()
    {
        var players = new[] { Player("A", "Alice"), Player("B", "Bob") };
        var matches = new List<Match> { Played("A", "B", 2, 0) };
        var rows = StandingsCalculator.Compute(matches, players);

        Assert.Equal(3, rows.First(r => r.PlayerId == "A").Points);
        Assert.Equal(0, rows.First(r => r.PlayerId == "B").Points);
    }

    [Fact]
    public void Compute_Draw_Awards1PointEach()
    {
        var players = new[] { Player("A", "Alice"), Player("B", "Bob") };
        var matches = new List<Match> { Played("A", "B", 1, 1) };
        var rows = StandingsCalculator.Compute(matches, players);

        Assert.All(rows, r => Assert.Equal(1, r.Points));
    }

    [Fact]
    public void Compute_GoalsAreCountedCorrectly()
    {
        var players = new[] { Player("A", "Alice"), Player("B", "Bob") };
        var matches = new List<Match> { Played("A", "B", 3, 1) };
        var rows = StandingsCalculator.Compute(matches, players);

        var a = rows.First(r => r.PlayerId == "A");
        Assert.Equal(3, a.GoalsFor);
        Assert.Equal(1, a.GoalsAgainst);
        Assert.Equal(2, a.GoalDifference);
    }

    [Fact]
    public void Compute_SortsByPointsDescending()
    {
        var players = new[] { Player("A", "Alice"), Player("B", "Bob"), Player("C", "Carlos") };
        var matches = new List<Match>
        {
            Played("A", "B", 1, 0),
            Played("A", "C", 1, 0),
            Played("B", "C", 1, 0)
        };
        var rows = StandingsCalculator.Compute(matches, players);

        Assert.Equal("A", rows[0].PlayerId); // 6pts
        Assert.Equal("B", rows[1].PlayerId); // 3pts
        Assert.Equal("C", rows[2].PlayerId); // 0pts
    }

    [Fact]
    public void Compute_TieBreaksByGoalDifference()
    {
        var players = new[] { Player("A", "Alice"), Player("B", "Bob"), Player("C", "Carlos") };
        var matches = new List<Match>
        {
            Played("A", "C", 3, 0), // A beats C big
            Played("B", "C", 1, 0), // B beats C small
            Played("A", "B", 0, 0)  // draw — A and B both end at 4pts
        };
        var rows = StandingsCalculator.Compute(matches, players);

        Assert.Equal("A", rows[0].PlayerId); // 4pts, GD=+3
        Assert.Equal("B", rows[1].PlayerId); // 4pts, GD=+1
        Assert.Equal("C", rows[2].PlayerId); // 0pts
    }

    [Fact]
    public void Compute_TieBreaksByH2H()
    {
        // A and B equal on all stats except head-to-head
        var players = new[] { Player("A", "Alice"), Player("B", "Bob"), Player("C", "Carlos") };
        var matches = new List<Match>
        {
            Played("A", "C", 1, 0),
            Played("B", "C", 1, 0),
            Played("B", "A", 1, 0) // B beat A directly
        };
        var rows = StandingsCalculator.Compute(matches, players);

        Assert.Equal("B", rows[0].PlayerId);
        Assert.Equal("A", rows[1].PlayerId);
    }

    [Fact]
    public void Compute_TieBreaksByNameAlphabetically()
    {
        var players = new[] { Player("A", "Zara"), Player("B", "Alice") };
        var rows = StandingsCalculator.Compute([], players);

        Assert.Equal("B", rows[0].PlayerId); // "Alice" < "Zara"
    }

    [Fact]
    public void Compute_WonDrawnLostAreCorrect()
    {
        var players = new[] { Player("A", "Alice"), Player("B", "Bob"), Player("C", "Carlos") };
        var matches = new List<Match>
        {
            Played("A", "B", 2, 1),
            Played("A", "C", 1, 1),
            Played("B", "C", 0, 2)
        };
        var rows = StandingsCalculator.Compute(matches, players);
        var a = rows.First(r => r.PlayerId == "A");

        Assert.Equal(1, a.Won);
        Assert.Equal(1, a.Drawn);
        Assert.Equal(0, a.Lost);
        Assert.Equal(2, a.Played);
    }
}
