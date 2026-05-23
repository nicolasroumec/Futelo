using System.Security.Claims;
using Futelo.Server.Services.Stats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futelo.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/stats")]
public class StatsController(IStatsService statsService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("vaults/{vaultId}/players/{playerId}")]
    public async Task<IActionResult> GetPlayerStats(int vaultId, string playerId)
    {
        var stats = await statsService.GetPlayerStatsAsync(playerId, vaultId, UserId);
        return Ok(stats);
    }

    [HttpGet("vaults/{vaultId}/ranking")]
    public async Task<IActionResult> GetGeneralRanking(int vaultId)
    {
        var ranking = await statsService.GetGeneralRankingAsync(vaultId, UserId);
        return Ok(ranking);
    }

    [HttpGet("vaults/{vaultId}/h2h")]
    public async Task<IActionResult> GetHeadToHead(int vaultId, [FromQuery] string player1Id, [FromQuery] string player2Id)
    {
        var h2h = await statsService.GetHeadToHeadAsync(player1Id, player2Id, vaultId, UserId);
        return Ok(h2h);
    }

    [HttpGet("vaults/{vaultId}/seasons/{seasonId}/ranking")]
    public async Task<IActionResult> GetRanking(int vaultId, int seasonId)
    {
        var ranking = await statsService.GetRankingAsync(seasonId, vaultId, UserId);
        return Ok(ranking);
    }

    [HttpGet("vaults/{vaultId}/palmares")]
    public async Task<IActionResult> GetPalmares(int vaultId)
    {
        var palmares = await statsService.GetPalmaresAsync(vaultId, UserId);
        return Ok(palmares);
    }

    [HttpGet("vaults/{vaultId}/players/{playerId}/elo-history")]
    public async Task<IActionResult> GetEloHistory(int vaultId, string playerId)
    {
        var history = await statsService.GetEloHistoryAsync(playerId, vaultId, UserId);
        return Ok(history);
    }

    [HttpGet("vaults/{vaultId}/players/{playerId}/global-elo-history")]
    public async Task<IActionResult> GetGlobalEloHistory(int vaultId, string playerId, [FromQuery] string? competition)
    {
        var result = await statsService.GetGlobalEloHistoryAsync(playerId, vaultId, UserId, competition);
        return Ok(result);
    }

    [HttpGet("vaults/{vaultId}/scorers")]
    public async Task<IActionResult> GetScorers(int vaultId)
    {
        var scorers = await statsService.GetScorersAsync(vaultId, UserId);
        return Ok(scorers);
    }

    [HttpGet("vaults/{vaultId}/records")]
    public async Task<IActionResult> GetVaultRecords(int vaultId)
    {
        var records = await statsService.GetVaultRecordsAsync(vaultId, UserId);
        return Ok(records);
    }

    [HttpGet("vaults/{vaultId}/games")]
    public async Task<IActionResult> GetGamesRanking(int vaultId)
    {
        var games = await statsService.GetGamesRankingAsync(vaultId, UserId);
        return Ok(games);
    }

    [HttpGet("vaults/{vaultId}/teams")]
    public async Task<IActionResult> GetTeamPanel(int vaultId)
    {
        var teams = await statsService.GetTeamPanelAsync(vaultId, UserId);
        return Ok(teams);
    }

    [HttpGet("vaults/{vaultId}/players/{playerId}/recent-matches")]
    public async Task<IActionResult> GetPlayerRecentMatches(int vaultId, string playerId, [FromQuery] int limit = 5)
    {
        var matches = await statsService.GetPlayerRecentMatchesAsync(playerId, vaultId, UserId, limit);
        return Ok(matches);
    }

    [HttpGet("vaults/{vaultId}/players/{playerId}/matches")]
    public async Task<IActionResult> GetPlayerMatchHistory(int vaultId, string playerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? competitionType = null)
    {
        var result = await statsService.GetPlayerMatchHistoryAsync(playerId, vaultId, UserId, page, pageSize, competitionType);
        return Ok(result);
    }

    [HttpGet("vaults/{vaultId}/players/{playerId}/recent-form")]
    public async Task<IActionResult> GetRecentForm(int vaultId, string playerId)
    {
        var form = await statsService.GetRecentFormAsync(playerId, vaultId, UserId);
        return Ok(form);
    }

    [HttpGet("vaults/{vaultId}/players/{playerId}/records")]
    public async Task<IActionResult> GetPlayerRecords(int vaultId, string playerId)
    {
        var records = await statsService.GetPlayerRecordsAsync(playerId, vaultId, UserId);
        return Ok(records);
    }

    [HttpGet("vaults/{vaultId}/records/top-scoring-match")]
    public async Task<IActionResult> GetTopScoringMatch(int vaultId)
    {
        var match = await statsService.GetTopScoringMatchAsync(vaultId, UserId);
        if (match == null) return NotFound();
        return Ok(match);
    }

    [HttpGet("vaults/{vaultId}/all-time-standings")]
    public async Task<IActionResult> GetAllTimeStandings(int vaultId)
    {
        var standings = await statsService.GetAllTimeStandingsAsync(vaultId, UserId);
        return Ok(standings);
    }
}
