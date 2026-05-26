using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Server.Services.Stats;

public interface IStatsService
{
    Task<PlayerStatsResponse> GetPlayerStatsAsync(string playerId, int vaultId, string requesterId);
    Task<HeadToHeadResponse> GetHeadToHeadAsync(string player1Id, string player2Id, int vaultId, string requesterId);
    Task<List<RankingRow>> GetGeneralRankingAsync(int vaultId, string requesterId);
    Task<List<RankingRow>> GetRankingAsync(int seasonId, int vaultId, string requesterId);
    Task<List<PalmaresSeasonRow>> GetPalmaresAsync(int vaultId, string requesterId);
    Task<List<EloHistoryPoint>> GetEloHistoryAsync(string playerId, int vaultId, string requesterId);
    Task<GlobalEloHistoryResponse> GetGlobalEloHistoryAsync(string playerId, int vaultId, string requesterId, string? competitionType);
    Task<List<ScorerRow>> GetScorersAsync(int vaultId, string requesterId);
    Task<VaultRecordsResponse> GetVaultRecordsAsync(int vaultId, string requesterId);
    Task<List<TeamPanelRow>> GetTeamPanelAsync(int vaultId, string requesterId);
    Task<List<GameStatsEntry>> GetGamesRankingAsync(int vaultId, string requesterId);
    Task<List<RecentFormEntry>> GetRecentFormAsync(string playerId, int vaultId, string requesterId);
    Task<List<RecentMatchResponse>> GetPlayerRecentMatchesAsync(string playerId, int vaultId, string requesterId, int limit);
    Task<MatchHistoryPageResponse> GetPlayerMatchHistoryAsync(string playerId, int vaultId, string requesterId, int page, int pageSize, string? competitionType = null);
    Task<TopScoringMatchResponse?> GetTopScoringMatchAsync(int vaultId, string requesterId);
    Task<PlayerRecordsResponse> GetPlayerRecordsAsync(string playerId, int vaultId, string requesterId);
    Task<List<AllTimeStandingRow>> GetAllTimeStandingsAsync(int vaultId, string requesterId);
    Task<List<PlayerAchievementResponse>> GetPlayerAchievementsAsync(string playerId, int vaultId, string requesterId);
}
