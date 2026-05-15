namespace Futelo.Server.Helpers;

public static class EloCalculator
{
    public static (int change, int newElo) Compute(int myElo, int opponentElo, double result, int goalDiff, int k)
    {
        double expected = 1.0 / (1.0 + Math.Pow(10, (opponentElo - myElo) / 400.0));
        double multiplier = goalDiff >= 3 ? 1.5 : goalDiff == 2 ? 1.2 : 1.0;
        int change = (int)Math.Round(k * multiplier * (result - expected), MidpointRounding.AwayFromZero);
        return (change, myElo + change);
    }
}
