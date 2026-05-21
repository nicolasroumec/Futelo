using System.Security.Claims;
using Futelo.Server.Services.Season;
using Futelo.Shared.DTOs;
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
        var seasons = await seasonService.GetByVaultAsync(vaultId, UserId);
        return Ok(seasons);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var season = await seasonService.GetByIdAsync(id, UserId);
        return Ok(season);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSeasonRequest request)
    {
        var season = await seasonService.CreateAsync(UserId, request);
        return CreatedAtAction(nameof(GetById), new { id = season.Id }, season);
    }

    [HttpPut("{id}/finish")]
    public async Task<IActionResult> Finish(int id)
    {
        await seasonService.FinishAsync(id, UserId);
        return NoContent();
    }

    [HttpPut("{id}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        await seasonService.ActivateAsync(id, UserId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await seasonService.DeleteAsync(id, UserId);
        return NoContent();
    }

    [HttpPatch("{id}/players/{playerId}/team")]
    public async Task<IActionResult> SetPlayerTeam(int id, string playerId, SetSeasonPlayerTeamRequest request)
    {
        await seasonService.SetPlayerTeamAsync(id, playerId, UserId, request.TeamId);
        return NoContent();
    }

    [HttpPatch("{id}/video-game")]
    public async Task<IActionResult> PatchVideoGame(int id, UpdateSeasonVideoGameRequest request)
    {
        await seasonService.PatchVideoGameAsync(id, UserId, request.VideoGameId);
        return NoContent();
    }

    [HttpPatch("{id}/dates")]
    public async Task<IActionResult> PatchDates(int id, PatchDatesRequest request)
    {
        await seasonService.PatchDatesAsync(id, UserId, request.StartDate, request.EndDate);
        return NoContent();
    }

    [HttpPut("{id}/configure")]
    public async Task<IActionResult> Configure(int id, ConfigureSeasonRequest request)
    {
        await seasonService.ConfigureAsync(id, UserId, request);
        return NoContent();
    }
}
