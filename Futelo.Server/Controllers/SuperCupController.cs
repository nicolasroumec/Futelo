using System.Security.Claims;
using Futelo.Server.Services.SuperCup;
using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.DTOs.SuperCup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futelo.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/supercups")]
public class SuperCupController(ISuperCupService superCupService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var superCup = await superCupService.GetByIdAsync(id, UserId);
        return Ok(superCup);
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        await superCupService.StartAsync(id, UserId);
        return NoContent();
    }

    [HttpPut("{id}/matches/{matchId}/result")]
    public async Task<IActionResult> RecordResult(int id, int matchId, RecordSuperCupResultRequest request)
    {
        var result = await superCupService.RecordResultAsync(
            id, matchId, request.HomeScore, request.AwayScore,
            request.WonOnPenaltiesId, request.HomePenaltyScore, request.AwayPenaltyScore, UserId);
        return Ok(result);
    }

    [HttpPatch("{id}/matches/{matchId}")]
    public async Task<IActionResult> PatchMatch(int id, int matchId, PatchMatchRequest request)
    {
        await superCupService.PatchMatchAsync(id, matchId, request.HomeTeamId, request.AwayTeamId, request.VideoGameId, request.ScheduledDate, UserId);
        return NoContent();
    }

    [HttpPatch("{id}/dates")]
    public async Task<IActionResult> PatchDates(int id, PatchDatesRequest request)
    {
        await superCupService.PatchDatesAsync(id, UserId, request.StartDate, request.EndDate);
        return NoContent();
    }
}
