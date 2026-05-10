using Futelo.Server.Models;

namespace Futelo.Server.Repositories.Stats;

public interface IStatsRepository
{
    Task<AppUser?> GetPlayerAsync(string playerId);
    Task<List<Match>> GetPlayerMatchesInVaultAsync(string playerId, int vaultId);
    Task<List<Match>> GetH2HMatchesInVaultAsync(string player1Id, string player2Id, int vaultId);
    Task<bool> IsVaultMemberAsync(string playerId, int vaultId);
    Task<List<VaultPlayer>> GetGeneralRankingAsync(int vaultId);
    Task<List<SeasonPlayer>> GetSeasonRankingAsync(int seasonId, int vaultId);
    Task<List<Models.Season>> GetVaultPalmaresAsync(int vaultId);
    Task<List<EloHistory>> GetPlayerEloHistoryInVaultAsync(string playerId, int vaultId);
    Task<List<Match>> GetAllPlayedMatchesInVaultAsync(int vaultId);
}
