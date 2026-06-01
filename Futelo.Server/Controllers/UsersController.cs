using System.Security.Claims;
using System.Security.Cryptography;
using Futelo.Server.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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
        if (bytes is null)
        {
            // Never cache the "no image" response: otherwise the browser keeps
            // serving it after the user uploads an avatar, hiding the new image.
            Response.Headers.CacheControl = "no-store";
            return NotFound();
        }
        var etag = new EntityTagHeaderValue('"' + Convert.ToHexString(MD5.HashData(bytes)) + '"');
        Response.Headers.CacheControl = "no-cache, max-age=0, must-revalidate";
        return File(bytes, "image/webp", lastModified: null, entityTag: etag);
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
