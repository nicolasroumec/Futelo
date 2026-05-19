using Futelo.Server.Helpers;

namespace Futelo.Tests;

public class EloCalculatorTests
{
    [Fact]
    public void Compute_NewEloIsAlwaysMyEloPlusChange()
    {
        var (change, newElo) = EloCalculator.Compute(1500, 1400, 1.0, 2, k: 32);
        Assert.Equal(1500 + change, newElo);
    }

    [Fact]
    public void Compute_EqualElo_Win_By1Goal_ReturnsHalfK()
    {
        var (change, _) = EloCalculator.Compute(1500, 1500, 1.0, 1, k: 32);
        Assert.Equal(16, change);
    }

    [Fact]
    public void Compute_EqualElo_Draw_ReturnsZero()
    {
        var (change, _) = EloCalculator.Compute(1500, 1500, 0.5, 0, k: 32);
        Assert.Equal(0, change);
    }

    [Fact]
    public void Compute_EqualElo_Win_By3Goals_AppliesMultiplier()
    {
        // multiplier = 1.5 → round(32 * 1.5 * 0.5) = 24
        var (change, _) = EloCalculator.Compute(1500, 1500, 1.0, 3, k: 32);
        Assert.Equal(24, change);
    }

    [Fact]
    public void Compute_EqualElo_Win_By2Goals_AppliesMultiplier()
    {
        // multiplier = 1.2 → round(32 * 1.2 * 0.5) = round(19.2) = 19
        var (change, _) = EloCalculator.Compute(1500, 1500, 1.0, 2, k: 32);
        Assert.Equal(19, change);
    }

    [Fact]
    public void Compute_Favorite_Wins_SmallGain()
    {
        // 1800 beats 1200 — expected ≈ 0.969, change = round(32 * 1.0 * 0.031) = 1
        var (change, _) = EloCalculator.Compute(1800, 1200, 1.0, 1, k: 32);
        Assert.Equal(1, change);
    }

    [Fact]
    public void Compute_Underdog_Wins_LargeGain()
    {
        // 1200 beats 1800 by 3+ goals — expected ≈ 0.031, multiplier 1.5
        // change = round(32 * 1.5 * 0.969) = round(46.5) = 47
        var (change, _) = EloCalculator.Compute(1200, 1800, 1.0, 3, k: 32);
        Assert.Equal(47, change);
    }

    [Fact]
    public void Compute_KScalesChange()
    {
        // Same match, different K
        var (changeK32, _) = EloCalculator.Compute(1500, 1500, 1.0, 1, k: 32);
        var (changeK16, _) = EloCalculator.Compute(1500, 1500, 1.0, 1, k: 16);
        Assert.Equal(changeK32, changeK16 * 2);
    }

    [Fact]
    public void Compute_WinAndLoss_ChangesAreSymmetric()
    {
        var (winChange, _) = EloCalculator.Compute(1500, 1500, 1.0, 1, k: 32);
        var (lossChange, _) = EloCalculator.Compute(1500, 1500, 0.0, 1, k: 32);
        Assert.Equal(winChange, -lossChange);
    }
}
