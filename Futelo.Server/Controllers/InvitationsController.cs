using System.Security.Claims;
using Futelo.Server.Services.Invitation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futelo.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InvitationsController(IInvitationService invitationService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [AllowAnonymous]
    [HttpGet("{token}")]
    public async Task<IActionResult> GetPreview(string token)
    {
        var preview = await invitationService.GetPreviewAsync(token);
        return Ok(preview);
    }

    [HttpPost("{token}/accept")]
    public async Task<IActionResult> Accept(string token)
    {
        await invitationService.AcceptAsync(token, UserId);
        return NoContent();
    }
}
