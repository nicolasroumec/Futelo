using Futelo.Server.Helpers;
using Futelo.Shared.Enums;

namespace Futelo.Tests;

public class FixtureGeneratorTests
{
    [Fact]
    public void Build_2Players_Generates1Match()
    {
        var matches = FixtureGenerator.Build(["A", "B"], leagueId: 1, isHomeAndAway: false);
        Assert.Single(matches);
    }

    [Fact]
    public void Build_4Players_Generates6Matches()
    {
        var matches = FixtureGenerator.Build(["A", "B", "C", "D"], leagueId: 1, isHomeAndAway: false);
        Assert.Equal(6, matches.Count);
    }

    [Fact]
    public void Build_HomeAndAway_DoublesSingleLegCount()
    {
        var single = FixtureGenerator.Build(["A", "B", "C", "D"], leagueId: 1, isHomeAndAway: false);
        var both = FixtureGenerator.Build(["A", "B", "C", "D"], leagueId: 1, isHomeAndAway: true);
        Assert.Equal(single.Count * 2, both.Count);
    }

    [Fact]
    public void Build_OddPlayers_HandlesByeCorrectly()
    {
        // 3 players → 3 matchdays, 1 match each (bye rotates)
        var matches = FixtureGenerator.Build(["A", "B", "C"], leagueId: 1, isHomeAndAway: false);
        Assert.Equal(3, matches.Count);
    }

    [Fact]
    public void Build_NoPlayerFacesThemself()
    {
        var matches = FixtureGenerator.Build(["A", "B", "C", "D", "E"], leagueId: 1, isHomeAndAway: false);
        Assert.All(matches, m => Assert.NotEqual(m.HomePlayerId, m.AwayPlayerId));
    }

    [Fact]
    public void Build_AllMatchesArePending()
    {
        var matches = FixtureGenerator.Build(["A", "B", "C", "D"], leagueId: 1, isHomeAndAway: false);
        Assert.All(matches, m => Assert.Equal(MatchStatus.Pending, m.Status));
    }

    [Fact]
    public void Build_AllMatchesBelongToLeague()
    {
        var matches = FixtureGenerator.Build(["A", "B", "C", "D"], leagueId: 42, isHomeAndAway: false);
        Assert.All(matches, m => Assert.Equal(42, m.LeagueId));
    }

    [Fact]
    public void Build_EachPairMeetsExactlyOnce_SingleLeg()
    {
        var players = new List<string> { "A", "B", "C", "D" };
        var matches = FixtureGenerator.Build(players, leagueId: 1, isHomeAndAway: false);

        foreach (var p1 in players)
            foreach (var p2 in players.Where(p => p != p1))
            {
                int count = matches.Count(m =>
                    (m.HomePlayerId == p1 && m.AwayPlayerId == p2) ||
                    (m.HomePlayerId == p2 && m.AwayPlayerId == p1));
                Assert.Equal(1, count);
            }
    }

    [Fact]
    public void Build_HomeAndAway_EachPairMeetsTwice()
    {
        var players = new List<string> { "A", "B", "C", "D" };
        var matches = FixtureGenerator.Build(players, leagueId: 1, isHomeAndAway: true);

        foreach (var p1 in players)
            foreach (var p2 in players.Where(p => p != p1))
            {
                int count = matches.Count(m => m.HomePlayerId == p1 && m.AwayPlayerId == p2);
                Assert.Equal(1, count);
            }
    }

    [Fact]
    public void Build_ReturnLegs_HaveHigherLegNumbers()
    {
        var matches = FixtureGenerator.Build(["A", "B", "C", "D"], leagueId: 1, isHomeAndAway: true);
        int totalRounds = 3; // 4 players → 3 rounds
        Assert.All(matches.Where(m => m.Leg > totalRounds), m => Assert.True(m.Leg > totalRounds));
    }
}
