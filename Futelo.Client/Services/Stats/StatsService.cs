using System.Net.Http.Json;
using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Client.Services.Stats;

public class StatsService(HttpClient http) : ApiService(http), IStatsService
{
    public Task<PlayerStatsResponse> GetPlayerStatsAsync(string playerId, int vaultId, CancellationToken ct = default)
        => GetAsync<PlayerStatsResponse>($"api/stats/vaults/{vaultId}/players/{playerId}", ct);

    public Task<HeadToHeadResponse> GetHeadToHeadAsync(string player1Id, string player2Id, int vaultId, CancellationToken ct = default)
        => GetAsync<HeadToHeadResponse>($"api/stats/vaults/{vaultId}/h2h?player1Id={player1Id}&player2Id={player2Id}", ct);

    public Task<List<RankingRow>> GetGeneralRankingAsync(int vaultId, CancellationToken ct = default)
        => GetListAsync<RankingRow>($"api/stats/vaults/{vaultId}/ranking", ct);

    public Task<List<RankingRow>> GetRankingAsync(int seasonId, int vaultId, CancellationToken ct = default)
        => GetListAsync<RankingRow>($"api/stats/vaults/{vaultId}/seasons/{seasonId}/ranking", ct);

    public Task<List<PalmaresSeasonRow>> GetPalmaresAsync(int vaultId, CancellationToken ct = default)
        => GetListAsync<PalmaresSeasonRow>($"api/stats/vaults/{vaultId}/palmares", ct);

    public Task<List<EloHistoryPoint>> GetEloHistoryAsync(int vaultId, string playerId, CancellationToken ct = default)
        => GetListAsync<EloHistoryPoint>($"api/stats/vaults/{vaultId}/players/{playerId}/elo-history", ct);

    public Task<GlobalEloHistoryResponse> GetGlobalEloHistoryAsync(int vaultId, string playerId, string? competition = null, CancellationToken ct = default)
    {
        var url = $"api/stats/vaults/{vaultId}/players/{playerId}/global-elo-history";
        if (!string.IsNullOrEmpty(competition)) url += $"?competition={competition}";
        return GetAsync<GlobalEloHistoryResponse>(url, ct);
    }

    public Task<List<ScorerRow>> GetScorersAsync(int vaultId, CancellationToken ct = default)
        => GetListAsync<ScorerRow>($"api/stats/vaults/{vaultId}/scorers", ct);

    public Task<VaultRecordsResponse> GetVaultRecordsAsync(int vaultId, CancellationToken ct = default)
        => GetAsync<VaultRecordsResponse>($"api/stats/vaults/{vaultId}/records", ct);

    public Task<List<TeamPanelRow>> GetTeamPanelAsync(int vaultId, CancellationToken ct = default)
        => GetListAsync<TeamPanelRow>($"api/stats/vaults/{vaultId}/teams", ct);

    public Task<List<GameStatsEntry>> GetGamesRankingAsync(int vaultId, CancellationToken ct = default)
        => GetListAsync<GameStatsEntry>($"api/stats/vaults/{vaultId}/games", ct);

    public Task<List<RecentFormEntry>> GetRecentFormAsync(int vaultId, string playerId, CancellationToken ct = default)
        => GetListAsync<RecentFormEntry>($"api/stats/vaults/{vaultId}/players/{playerId}/recent-form", ct);

    public Task<List<RecentMatchResponse>> GetPlayerRecentMatchesAsync(int vaultId, string playerId, int limit = 5, CancellationToken ct = default)
        => GetListAsync<RecentMatchResponse>($"api/stats/vaults/{vaultId}/players/{playerId}/recent-matches?limit={limit}", ct);

    public Task<MatchHistoryPageResponse> GetPlayerMatchHistoryAsync(int vaultId, string playerId, int page = 1, int pageSize = 10, string? competitionType = null, CancellationToken ct = default)
    {
        var url = $"api/stats/vaults/{vaultId}/players/{playerId}/matches?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(competitionType)) url += $"&competitionType={competitionType}";
        return GetAsync<MatchHistoryPageResponse>(url, ct);
    }

    public Task<PlayerRecordsResponse> GetPlayerRecordsAsync(int vaultId, string playerId, CancellationToken ct = default)
        => GetAsync<PlayerRecordsResponse>($"api/stats/vaults/{vaultId}/players/{playerId}/records", ct);

    public async Task<TopScoringMatchResponse?> GetTopScoringMatchAsync(int vaultId, CancellationToken ct = default)
    {
        var response = await Http.GetAsync($"api/stats/vaults/{vaultId}/records/top-scoring-match", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        await response.EnsureSuccessAsync();
        return await response.Content.ReadFromJsonAsync<TopScoringMatchResponse>(ct);
    }
}
