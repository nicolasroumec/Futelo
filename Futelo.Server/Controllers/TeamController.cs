using Futelo.Server.Services.Teams;
using Futelo.Shared.DTOs.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futelo.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/teams")]
public class TeamController(ITeamService teamService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teams = await teamService.GetAllAsync();
        return Ok(teams);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamRequest request)
    {
        var team = await teamService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = team.Id }, team);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateTeamRequest request)
    {
        await teamService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await teamService.DeleteAsync(id);
        return NoContent();
    }
}
