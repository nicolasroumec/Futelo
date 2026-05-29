using System.Security.Claims;
using Futelo.Server.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futelo.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{id}/avatar")]
    public async Task<IActionResult> GetAvatar(string id)
    {
        var bytes = await userService.GetAvatarAsync(id);
        if (bytes is null) return NotFound();
        return File(bytes, "image/webp");
    }

    [HttpPut("me/avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file.Length > 500_000) return BadRequest("Image must be under 500 KB.");
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await userService.SetAvatarAsync(userId, ms.ToArray());
        return NoContent();
    }

    [HttpDelete("me/avatar")]
    public async Task<IActionResult> DeleteAvatar()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await userService.DeleteAvatarAsync(userId);
        return NoContent();
    }
}
