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
        var vault = await vaultService.GetByIdAsync(id, UserId);
        return Ok(vault);
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
        await vaultService.UpdateAsync(id, UserId, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await vaultService.DeleteAsync(id, UserId);
        return NoContent();
    }

    [HttpGet("{id}/matches")]
    public async Task<IActionResult> GetMatchHistory(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? competitionType = null)
    {
        var result = await vaultService.GetMatchHistoryAsync(id, UserId, page, pageSize, competitionType);
        return Ok(result);
    }

    [HttpGet("{id}/recent-matches")]
    public async Task<IActionResult> GetRecentMatches(int id, [FromQuery] int limit = 10)
    {
        var matches = await vaultService.GetRecentMatchesAsync(id, UserId, limit);
        return Ok(matches);
    }

    [HttpGet("{id}/feed")]
    public async Task<IActionResult> GetFeed(int id, [FromQuery] int limit = 10)
    {
        var feed = await vaultService.GetFeedAsync(id, UserId, limit);
        return Ok(feed);
    }

    [HttpDelete("{id}/players/{playerId}")]
    public async Task<IActionResult> RemovePlayer(int id, string playerId)
    {
        await vaultService.RemovePlayerAsync(id, UserId, playerId);
        return NoContent();
    }

    [HttpPost("{id}/invite")]
    public async Task<IActionResult> Invite(int id, InviteRequest request)
    {
        var invitation = await invitationService.InviteAsync(id, UserId, request);
        return Ok(invitation);
    }
}
