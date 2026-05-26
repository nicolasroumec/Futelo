namespace Futelo.Server.Services.Achievement;

public record MatchAchievementContext(
    int MatchId, int VaultId, int SeasonId,
    string HomePlayerId, string AwayPlayerId,
    int HomeScore, int AwayScore,
    string? WonOnPenaltiesId,
    int HomeOldHistElo, int HomeNewHistElo,
    int AwayOldHistElo, int AwayNewHistElo,
    bool IsFinal
);

public interface IAchievementEngine
{
    Task EvaluateAfterMatchAsync(MatchAchievementContext ctx);
    Task EvaluateAfterSeasonAsync(int seasonId, int vaultId);
}
