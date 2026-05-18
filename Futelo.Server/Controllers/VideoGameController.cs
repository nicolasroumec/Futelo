using Futelo.Server.Services.VideoGames;
using Futelo.Shared.DTOs.VideoGame;
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

    [HttpPost]
    public async Task<IActionResult> Create(CreateVideoGameRequest request)
    {
        var game = await videoGameService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = game.Id }, game);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateVideoGameRequest request)
    {
        await videoGameService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await videoGameService.DeleteAsync(id);
        return NoContent();
    }
}
