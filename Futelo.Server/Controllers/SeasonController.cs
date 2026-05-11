using System.Security.Claims;
using Futelo.Server.Services.Season;
using Futelo.Shared.DTOs.Season;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futelo.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/seasons")]
public class SeasonController(ISeasonService seasonService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetByVault([FromQuery] int vaultId)
    {
        try
        {
            var seasons = await seasonService.GetByVaultAsync(vaultId, UserId);
            return Ok(seasons);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var season = await seasonService.GetByIdAsync(id, UserId);
            return Ok(season);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSeasonRequest request)
    {
        try
        {
            var season = await seasonService.CreateAsync(UserId, request);
            return CreatedAtAction(nameof(GetById), new { id = season.Id }, season);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("{id}/finish")]
    public async Task<IActionResult> Finish(int id)
    {
        try
        {
            await seasonService.FinishAsync(id, UserId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        try
        {
            await seasonService.ActivateAsync(id, UserId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id}/video-game")]
    public async Task<IActionResult> PatchVideoGame(int id, UpdateSeasonVideoGameRequest request)
    {
        try
        {
            await seasonService.PatchVideoGameAsync(id, UserId, request.VideoGameId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("{id}/configure")]
    public async Task<IActionResult> Configure(int id, ConfigureSeasonRequest request)
    {
        try
        {
            await seasonService.ConfigureAsync(id, UserId, request);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
