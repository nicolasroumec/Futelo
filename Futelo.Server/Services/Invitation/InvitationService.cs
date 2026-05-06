using Futelo.Server.Models;
using Futelo.Server.Repositories.Invitation;
using Futelo.Server.Repositories.Vault;
using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.Enums;

namespace Futelo.Server.Services.Invitation;

public class InvitationService(IInvitationRepository invitationRepository, IVaultRepository vaultRepository) : IInvitationService
{
    public async Task<InvitationResponse> InviteAsync(int vaultId, string userId, InviteRequest request)
    {
        var vault = await vaultRepository.GetByIdAsync(vaultId)
            ?? throw new KeyNotFoundException("Vault not found.");

        var caller = vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || caller.Role != VaultRole.Admin)
            throw new UnauthorizedAccessException("Only vault admins can invite players.");

        var invitation = new VaultInvitation
        {
            VaultId = vaultId,
            Email = string.Empty,
            Token = Guid.NewGuid().ToString("N"),
            Status = InvitationStatus.Pending,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await invitationRepository.CreateAsync(invitation);

        return MapToResponse(invitation, vault.Name);
    }

    public async Task AcceptAsync(string token, string userId)
    {
        var invitation = await invitationRepository.GetByTokenAsync(token)
            ?? throw new KeyNotFoundException("Invitation not found.");

        if (invitation.Status != InvitationStatus.Pending)
            throw new InvalidOperationException("This invitation is no longer valid.");

        if (invitation.ExpiresAt < DateTime.UtcNow)
        {
            invitation.Status = InvitationStatus.Expired;
            await invitationRepository.UpdateAsync(invitation);
            throw new InvalidOperationException("This invitation has expired.");
        }

        if (invitation.Vault.Players.Any(p => p.PlayerId == userId))
            throw new InvalidOperationException("You are already a member of this vault.");

        await vaultRepository.AddPlayerAsync(new VaultPlayer
        {
            VaultId = invitation.VaultId,
            PlayerId = userId,
            JoinedAt = DateTime.UtcNow,
            Role = invitation.Role
        });

        invitation.Status = InvitationStatus.Accepted;
        await invitationRepository.UpdateAsync(invitation);
    }

    private static InvitationResponse MapToResponse(VaultInvitation invitation, string vaultName) => new()
    {
        Id = invitation.Id,
        VaultId = invitation.VaultId,
        VaultName = vaultName,
        Token = invitation.Token,
        Status = invitation.Status.ToString(),
        CreatedAt = invitation.CreatedAt,
        ExpiresAt = invitation.ExpiresAt
    };
}
