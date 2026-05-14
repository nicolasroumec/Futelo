using System.Security.Claims;
using Futelo.Server.Services.Invitation;
using Futelo.Server.Services.Vault;
using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.DTOs.Vault;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futelo.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/vaults")]
public class VaultController(IVaultService vaultService, IInvitationService invitationService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vaults = await vaultService.GetUserVaultsAsync(UserId);
        return Ok(vaults);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var vault = await vaultService.GetByIdAsync(id, UserId);
            return Ok(vault);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVaultRequest request)
    {
        var vault = await vaultService.CreateAsync(UserId, request);
        return CreatedAtAction(nameof(GetById), new { id = vault.Id }, vault);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateVaultRequest request)
    {
        try
        {
            await vaultService.UpdateAsync(id, UserId, request);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await vaultService.DeleteAsync(id, UserId);
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

    [HttpGet("{id}/matches")]
    public async Task<IActionResult> GetMatchHistory(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await vaultService.GetMatchHistoryAsync(id, UserId, page, pageSize);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/recent-matches")]
    public async Task<IActionResult> GetRecentMatches(int id, [FromQuery] int limit = 10)
    {
        try
        {
            var matches = await vaultService.GetRecentMatchesAsync(id, UserId, limit);
            return Ok(matches);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/invite")]
    public async Task<IActionResult> Invite(int id, InviteRequest request)
    {
        try
        {
            var invitation = await invitationService.InviteAsync(id, UserId, request);
            return Ok(invitation);
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
