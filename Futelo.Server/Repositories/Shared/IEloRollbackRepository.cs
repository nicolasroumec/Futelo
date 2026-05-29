namespace Futelo.Server.Repositories.Shared;

public interface IEloRollbackRepository
{
    Task<bool> IsLastMatchForBothPlayersAsync(int matchId, string player1Id, string player2Id);
    Task RollbackMatchEloAsync(int matchId, string player1Id, string player2Id, int seasonId);
}
