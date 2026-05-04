using Futelo.Server.Services.VideoGames;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futelo.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/videogames")]
public class VideoGameController(IVideoGameService videoGameService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var games = await videoGameService.GetAllAsync();
        return Ok(games);
    }
}
