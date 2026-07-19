using Futelo.Server.Models;

namespace Futelo.Server.Repositories.Stats;

public interface IStatsRepository
{
    Task<AppUser?> GetPlayerAsync(string playerId);
    Task<List<Match>> GetPlayerMatchesInVaultAsync(string playerId, int vaultId);
    Task<List<Match>> GetH2HMatchesInVaultAsync(string player1Id, string player2Id, int vaultId);
    Task<bool> IsVaultMemberAsync(string playerId, int vaultId);
    Task<List<VaultPlayer>> GetGeneralRankingAsync(int vaultId);
    Task<Dictionary<string, int>> GetVaultEloMapAsync(int vaultId);
    Task<List<SeasonPlayer>> GetSeasonRankingAsync(int seasonId, int vaultId);
    Task<List<Models.Season>> GetVaultPalmaresAsync(int vaultId);
    Task<List<EloHistory>> GetPlayerEloHistoryInVaultAsync(string playerId, int vaultId);
    Task<List<EloHistory>> GetPlayerGlobalEloHistoryAsync(string playerId, int vaultId, string? competitionType);
    Task<List<Match>> GetAllPlayedMatchesInVaultAsync(int vaultId);
    Task<List<Match>> GetAllPlayedMatchesWithTeamsInVaultAsync(int vaultId);
    Task<List<Match>> GetAllPlayedMatchesWithVideoGameInVaultAsync(int vaultId);
    Task<List<Match>> GetPlayerLastNMatchesAsync(string playerId, int vaultId, int n);
    Task<List<Match>> GetPlayerRecentMatchesAsync(string playerId, int vaultId, int limit);
    Task<List<Match>> GetPlayerMatchesPageAsync(string playerId, int vaultId, int skip, int take, string? competitionType = null);
    Task<int> CountPlayerMatchesAsync(string playerId, int vaultId, string? competitionType = null);
    Task<Match?> GetTopScoringMatchInVaultAsync(int vaultId);
    Task<List<Models.Season>> GetPlayerTitleSeasonsInVaultAsync(string playerId, int vaultId);
    Task<List<Match>> GetPlayerMatchesWithOpponentsInVaultAsync(string playerId, int vaultId);
    Task<List<EloHistory>> GetAllHistoricalEloInVaultAsync(int vaultId);
}
