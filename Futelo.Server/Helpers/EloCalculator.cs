namespace Futelo.Server.Helpers;

public static class EloCalculator
{
    public const int    InitialElo = 1500;
    public const int    LeagueK    = 32;
    public const int    CupBaseK   = 24;
    public const int    SuperCupK  = 16;
    public const double GoalDiff3x = 1.5;
    public const double GoalDiff2x = 1.2;

    public static (int change, int newElo) Compute(int myElo, int opponentElo, double result, int goalDiff, int k)
    {
        double expected = 1.0 / (1.0 + Math.Pow(10, (opponentElo - myElo) / 400.0));
        double multiplier = goalDiff >= 3 ? GoalDiff3x : goalDiff == 2 ? GoalDiff2x : 1.0;
        int change = (int)Math.Round(k * multiplier * (result - expected), MidpointRounding.AwayFromZero);
        return (change, myElo + change);
    }
}
