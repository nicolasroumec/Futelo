using Futelo.Server.Services.Teams;
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
}
