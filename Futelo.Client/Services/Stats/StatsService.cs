using System.Net.Http.Json;
using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Client.Services.Stats;

public class StatsService(HttpClient http) : ApiService(http), IStatsService
{
    public Task<PlayerStatsResponse> GetPlayerStatsAsync(string playerId, int vaultId)
        => GetAsync<PlayerStatsResponse>($"api/stats/vaults/{vaultId}/players/{playerId}");

    public Task<HeadToHeadResponse> GetHeadToHeadAsync(string player1Id, string player2Id, int vaultId)
        => GetAsync<HeadToHeadResponse>($"api/stats/vaults/{vaultId}/h2h?player1Id={player1Id}&player2Id={player2Id}");

    public Task<List<RankingRow>> GetGeneralRankingAsync(int vaultId)
        => GetListAsync<RankingRow>($"api/stats/vaults/{vaultId}/ranking");

    public Task<List<RankingRow>> GetRankingAsync(int seasonId, int vaultId)
        => GetListAsync<RankingRow>($"api/stats/vaults/{vaultId}/seasons/{seasonId}/ranking");

    public Task<List<PalmaresSeasonRow>> GetPalmaresAsync(int vaultId)
        => GetListAsync<PalmaresSeasonRow>($"api/stats/vaults/{vaultId}/palmares");

    public Task<List<EloHistoryPoint>> GetEloHistoryAsync(int vaultId, string playerId)
        => GetListAsync<EloHistoryPoint>($"api/stats/vaults/{vaultId}/players/{playerId}/elo-history");

    public Task<List<ScorerRow>> GetScorersAsync(int vaultId)
        => GetListAsync<ScorerRow>($"api/stats/vaults/{vaultId}/scorers");

    public Task<VaultRecordsResponse> GetVaultRecordsAsync(int vaultId)
        => GetAsync<VaultRecordsResponse>($"api/stats/vaults/{vaultId}/records");

    public Task<List<TeamPanelRow>> GetTeamPanelAsync(int vaultId)
        => GetListAsync<TeamPanelRow>($"api/stats/vaults/{vaultId}/teams");

    public Task<List<GameStatsEntry>> GetGamesRankingAsync(int vaultId)
        => GetListAsync<GameStatsEntry>($"api/stats/vaults/{vaultId}/games");

    public Task<List<RecentFormEntry>> GetRecentFormAsync(int vaultId, string playerId)
        => GetListAsync<RecentFormEntry>($"api/stats/vaults/{vaultId}/players/{playerId}/recent-form");

    public Task<List<RecentMatchResponse>> GetPlayerRecentMatchesAsync(int vaultId, string playerId, int limit = 5)
        => GetListAsync<RecentMatchResponse>($"api/stats/vaults/{vaultId}/players/{playerId}/recent-matches?limit={limit}");

    public Task<MatchHistoryPageResponse> GetPlayerMatchHistoryAsync(int vaultId, string playerId, int page = 1, int pageSize = 10)
        => GetAsync<MatchHistoryPageResponse>($"api/stats/vaults/{vaultId}/players/{playerId}/matches?page={page}&pageSize={pageSize}");

    public async Task<TopScoringMatchResponse?> GetTopScoringMatchAsync(int vaultId)
    {
        var response = await Http.GetAsync($"api/stats/vaults/{vaultId}/records/top-scoring-match");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        await response.EnsureSuccessAsync();
        return await response.Content.ReadFromJsonAsync<TopScoringMatchResponse>();
    }
}
