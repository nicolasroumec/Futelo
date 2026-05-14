using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Client.Services.Stats;

public interface IStatsService
{
    Task<PlayerStatsResponse> GetPlayerStatsAsync(string playerId, int vaultId);
    Task<HeadToHeadResponse> GetHeadToHeadAsync(string player1Id, string player2Id, int vaultId);
    Task<List<RankingRow>> GetGeneralRankingAsync(int vaultId);
    Task<List<RankingRow>> GetRankingAsync(int seasonId, int vaultId);
    Task<List<PalmaresSeasonRow>> GetPalmaresAsync(int vaultId);
    Task<List<EloHistoryPoint>> GetEloHistoryAsync(int vaultId, string playerId);
    Task<List<ScorerRow>> GetScorersAsync(int vaultId);
    Task<VaultRecordsResponse> GetVaultRecordsAsync(int vaultId);
    Task<List<TeamPanelRow>> GetTeamPanelAsync(int vaultId);
    Task<List<GameStatsEntry>> GetGamesRankingAsync(int vaultId);
    Task<List<RecentFormEntry>> GetRecentFormAsync(int vaultId, string playerId);
    Task<List<RecentMatchResponse>> GetPlayerRecentMatchesAsync(int vaultId, string playerId, int limit = 5);
    Task<MatchHistoryPageResponse> GetPlayerMatchHistoryAsync(int vaultId, string playerId, int page = 1, int pageSize = 10);
    Task<TopScoringMatchResponse?> GetTopScoringMatchAsync(int vaultId);
}
