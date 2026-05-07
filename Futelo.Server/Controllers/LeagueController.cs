using System.Security.Claims;
using Futelo.Server.Services.League;
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
        try
        {
            var league = await leagueService.GetByIdAsync(id, UserId);
            return Ok(league);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        try
        {
            await leagueService.GenerateFixtureAsync(id, UserId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/reshuffle")]
    public async Task<IActionResult> Reshuffle(int id)
    {
        try
        {
            await leagueService.RegenerateFixtureAsync(id, UserId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/matches/{matchId}/result")]
    public async Task<IActionResult> RecordResult(int id, int matchId, RecordResultRequest request)
    {
        try
        {
            var result = await leagueService.RecordResultAsync(id, matchId, request.HomeScore, request.AwayScore, UserId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
