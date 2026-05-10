using Futelo.Shared.DTOs.Stats;

namespace Futelo.Server.Services.Stats;

public interface IStatsService
{
    Task<PlayerStatsResponse> GetPlayerStatsAsync(string playerId, int vaultId, string requesterId);
    Task<HeadToHeadResponse> GetHeadToHeadAsync(string player1Id, string player2Id, int vaultId, string requesterId);
    Task<List<RankingRow>> GetGeneralRankingAsync(int vaultId, string requesterId);
    Task<List<RankingRow>> GetRankingAsync(int seasonId, int vaultId, string requesterId);
    Task<List<PalmaresSeasonRow>> GetPalmaresAsync(int vaultId, string requesterId);
    Task<List<EloHistoryPoint>> GetEloHistoryAsync(string playerId, int vaultId, string requesterId);
    Task<List<ScorerRow>> GetScorersAsync(int vaultId, string requesterId);
    Task<VaultRecordsResponse> GetVaultRecordsAsync(int vaultId, string requesterId);
}
