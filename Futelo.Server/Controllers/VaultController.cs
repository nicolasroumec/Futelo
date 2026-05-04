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
