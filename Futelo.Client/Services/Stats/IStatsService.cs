using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Client.Services.Stats;

public interface IStatsService
{
    Task<PlayerStatsResponse> GetPlayerStatsAsync(string playerId, int vaultId, CancellationToken ct = default);
    Task<HeadToHeadResponse> GetHeadToHeadAsync(string player1Id, string player2Id, int vaultId, CancellationToken ct = default);
    Task<List<RankingRow>> GetGeneralRankingAsync(int vaultId, CancellationToken ct = default);
    Task<List<RankingRow>> GetRankingAsync(int seasonId, int vaultId, CancellationToken ct = default);
    Task<List<PalmaresSeasonRow>> GetPalmaresAsync(int vaultId, CancellationToken ct = default);
    Task<List<EloHistoryPoint>> GetEloHistoryAsync(int vaultId, string playerId, CancellationToken ct = default);
    Task<GlobalEloHistoryResponse> GetGlobalEloHistoryAsync(int vaultId, string playerId, string? competition = null, CancellationToken ct = default);
    Task<List<ScorerRow>> GetScorersAsync(int vaultId, CancellationToken ct = default);
    Task<VaultRecordsResponse> GetVaultRecordsAsync(int vaultId, CancellationToken ct = default);
    Task<List<TeamPanelRow>> GetTeamPanelAsync(int vaultId, CancellationToken ct = default);
    Task<List<GameStatsEntry>> GetGamesRankingAsync(int vaultId, CancellationToken ct = default);
    Task<List<RecentFormEntry>> GetRecentFormAsync(int vaultId, string playerId, CancellationToken ct = default);
    Task<List<RecentMatchResponse>> GetPlayerRecentMatchesAsync(int vaultId, string playerId, int limit = 5, CancellationToken ct = default);
    Task<MatchHistoryPageResponse> GetPlayerMatchHistoryAsync(int vaultId, string playerId, int page = 1, int pageSize = 10, string? competitionType = null, CancellationToken ct = default);
    Task<TopScoringMatchResponse?> GetTopScoringMatchAsync(int vaultId, CancellationToken ct = default);
    Task<PlayerRecordsResponse> GetPlayerRecordsAsync(int vaultId, string playerId, CancellationToken ct = default);
}
