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
        try
        {
            var stats = await statsService.GetPlayerStatsAsync(playerId, vaultId, UserId);
            return Ok(stats);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("vaults/{vaultId}/ranking")]
    public async Task<IActionResult> GetGeneralRanking(int vaultId)
    {
        try
        {
            var ranking = await statsService.GetGeneralRankingAsync(vaultId, UserId);
            return Ok(ranking);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("vaults/{vaultId}/h2h")]
    public async Task<IActionResult> GetHeadToHead(int vaultId, [FromQuery] string player1Id, [FromQuery] string player2Id)
    {
        try
        {
            var h2h = await statsService.GetHeadToHeadAsync(player1Id, player2Id, vaultId, UserId);
            return Ok(h2h);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("vaults/{vaultId}/seasons/{seasonId}/ranking")]
    public async Task<IActionResult> GetRanking(int vaultId, int seasonId)
    {
        try
        {
            var ranking = await statsService.GetRankingAsync(seasonId, vaultId, UserId);
            return Ok(ranking);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("vaults/{vaultId}/palmares")]
    public async Task<IActionResult> GetPalmares(int vaultId)
    {
        try
        {
            var palmares = await statsService.GetPalmaresAsync(vaultId, UserId);
            return Ok(palmares);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("vaults/{vaultId}/players/{playerId}/elo-history")]
    public async Task<IActionResult> GetEloHistory(int vaultId, string playerId)
    {
        try
        {
            var history = await statsService.GetEloHistoryAsync(playerId, vaultId, UserId);
            return Ok(history);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("vaults/{vaultId}/scorers")]
    public async Task<IActionResult> GetScorers(int vaultId)
    {
        try
        {
            var scorers = await statsService.GetScorersAsync(vaultId, UserId);
            return Ok(scorers);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("vaults/{vaultId}/records")]
    public async Task<IActionResult> GetVaultRecords(int vaultId)
    {
        try
        {
            var records = await statsService.GetVaultRecordsAsync(vaultId, UserId);
            return Ok(records);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("vaults/{vaultId}/games")]
    public async Task<IActionResult> GetGamesRanking(int vaultId)
    {
        try
        {
            var games = await statsService.GetGamesRankingAsync(vaultId, UserId);
            return Ok(games);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("vaults/{vaultId}/teams")]
    public async Task<IActionResult> GetTeamPanel(int vaultId)
    {
        try
        {
            var teams = await statsService.GetTeamPanelAsync(vaultId, UserId);
            return Ok(teams);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
