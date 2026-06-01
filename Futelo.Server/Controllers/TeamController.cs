using System.Security.Cryptography;
using Futelo.Server.Services.Teams;
using Futelo.Shared.DTOs.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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

    [AllowAnonymous]
    [HttpGet("{id}/shield")]
    public async Task<IActionResult> GetShield(int id)
    {
        var bytes = await teamService.GetShieldAsync(id);
        if (bytes is null)
        {
            // Never cache the "no image" response: otherwise the browser keeps
            // serving it after a shield is uploaded, hiding the new image.
            Response.Headers.CacheControl = "no-store";
            return NotFound();
        }
        var etag = new EntityTagHeaderValue('"' + Convert.ToHexString(MD5.HashData(bytes)) + '"');
        Response.Headers.CacheControl = "no-cache, max-age=0, must-revalidate";
        return File(bytes, "image/webp", lastModified: null, entityTag: etag);
    }

    [HttpPut("{id}/shield")]
    public async Task<IActionResult> UploadShield(int id, IFormFile file)
    {
        if (file.Length > 200_000) return BadRequest("Image must be under 200 KB.");
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        await teamService.SetShieldAsync(id, ms.ToArray());
        return NoContent();
    }

    [HttpDelete("{id}/shield")]
    public async Task<IActionResult> DeleteShield(int id)
    {
        await teamService.DeleteShieldAsync(id);
        return NoContent();
    }
}
