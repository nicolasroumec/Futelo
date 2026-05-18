using System.Security.Claims;
using Futelo.Server.Services.Cup;
using Futelo.Shared.DTOs.Cup;
using Futelo.Shared.DTOs.League;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futelo.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/cups")]
public class CupController(ICupService cupService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cup = await cupService.GetByIdAsync(id, UserId);
        return Ok(cup);
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        await cupService.GenerateBracketAsync(id, UserId);
        return NoContent();
    }

    [HttpPut("{id}/matches/{matchId}/result")]
    public async Task<IActionResult> RecordResult(int id, int matchId, RecordCupResultRequest request)
    {
        var result = await cupService.RecordResultAsync(
            id, matchId, request.HomeScore, request.AwayScore,
            request.WonOnPenaltiesId, request.HomePenaltyScore, request.AwayPenaltyScore, UserId);
        return Ok(result);
    }

    [HttpPatch("{id}/matches/{matchId}")]
    public async Task<IActionResult> PatchMatch(int id, int matchId, PatchMatchRequest request)
    {
        await cupService.PatchMatchAsync(id, matchId, request.HomeTeamId, request.AwayTeamId, request.VideoGameId, UserId);
        return NoContent();
    }
}
