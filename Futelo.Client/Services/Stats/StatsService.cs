using System.Net.Http.Json;
using Futelo.Shared.DTOs.Stats;

namespace Futelo.Client.Services.Stats;

public class StatsService(HttpClient http) : IStatsService
{
    public async Task<PlayerStatsResponse> GetPlayerStatsAsync(string playerId, int vaultId)
        => await http.GetFromJsonAsync<PlayerStatsResponse>($"api/stats/vaults/{vaultId}/players/{playerId}")
            ?? throw new KeyNotFoundException("Player stats not found.");

    public async Task<HeadToHeadResponse> GetHeadToHeadAsync(string player1Id, string player2Id, int vaultId)
        => await http.GetFromJsonAsync<HeadToHeadResponse>(
                $"api/stats/vaults/{vaultId}/h2h?player1Id={player1Id}&player2Id={player2Id}")
            ?? throw new KeyNotFoundException("Head to head data not found.");

    public async Task<List<RankingRow>> GetGeneralRankingAsync(int vaultId)
        => await http.GetFromJsonAsync<List<RankingRow>>($"api/stats/vaults/{vaultId}/ranking")
            ?? [];

    public async Task<List<RankingRow>> GetRankingAsync(int seasonId, int vaultId)
        => await http.GetFromJsonAsync<List<RankingRow>>($"api/stats/vaults/{vaultId}/seasons/{seasonId}/ranking")
            ?? [];

    public async Task<List<PalmaresSeasonRow>> GetPalmaresAsync(int vaultId)
        => await http.GetFromJsonAsync<List<PalmaresSeasonRow>>($"api/stats/vaults/{vaultId}/palmares")
            ?? [];

    public async Task<List<EloHistoryPoint>> GetEloHistoryAsync(int vaultId, string playerId)
        => await http.GetFromJsonAsync<List<EloHistoryPoint>>($"api/stats/vaults/{vaultId}/players/{playerId}/elo-history")
            ?? [];

    public async Task<List<ScorerRow>> GetScorersAsync(int vaultId)
        => await http.GetFromJsonAsync<List<ScorerRow>>($"api/stats/vaults/{vaultId}/scorers")
            ?? [];

    public async Task<VaultRecordsResponse> GetVaultRecordsAsync(int vaultId)
        => await http.GetFromJsonAsync<VaultRecordsResponse>($"api/stats/vaults/{vaultId}/records")
            ?? new VaultRecordsResponse();

    public async Task<List<TeamPanelRow>> GetTeamPanelAsync(int vaultId)
        => await http.GetFromJsonAsync<List<TeamPanelRow>>($"api/stats/vaults/{vaultId}/teams")
            ?? [];
}
