using System.Security.Claims;
using Futelo.Server.Services.League;
using Futelo.Shared.DTOs;
using Futelo.Shared.DTOs.League;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futelo.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/leagues")]
public class LeagueController(ILeagueService leagueService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var league = await leagueService.GetByIdAsync(id, UserId);
        return Ok(league);
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        await leagueService.GenerateFixtureAsync(id, UserId);
        return NoContent();
    }

    [HttpPost("{id}/start-manual")]
    public async Task<IActionResult> StartManual(int id)
    {
        await leagueService.StartManualAsync(id, UserId);
        return NoContent();
    }

    [HttpPost("{id}/matches")]
    public async Task<IActionResult> AddMatch(int id, AddLeagueMatchRequest request)
    {
        await leagueService.AddMatchManuallyAsync(id, request, UserId);
        return NoContent();
    }

    [HttpPut("{id}/reshuffle")]
    public async Task<IActionResult> Reshuffle(int id)
    {
        await leagueService.RegenerateFixtureAsync(id, UserId);
        return NoContent();
    }

    [HttpPatch("{id}/matches/{matchId}")]
    public async Task<IActionResult> PatchMatch(int id, int matchId, PatchMatchRequest request)
    {
        await leagueService.PatchMatchAsync(id, matchId, request.HomeTeamId, request.AwayTeamId, request.VideoGameId, request.ScheduledDate, UserId);
        return NoContent();
    }

    [HttpPatch("{id}/dates")]
    public async Task<IActionResult> PatchDates(int id, PatchDatesRequest request)
    {
        await leagueService.PatchDatesAsync(id, UserId, request.StartDate, request.EndDate);
        return NoContent();
    }

    [HttpPut("{id}/matches/{matchId}/result")]
    public async Task<IActionResult> RecordResult(int id, int matchId, RecordResultRequest request)
    {
        var result = await leagueService.RecordResultAsync(id, matchId, request.HomeScore, request.AwayScore, UserId);
        return Ok(result);
    }
}
