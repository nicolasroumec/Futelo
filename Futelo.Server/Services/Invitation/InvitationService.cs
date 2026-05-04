using Futelo.Server.Models;
using Futelo.Server.Repositories.Invitation;
using Futelo.Server.Repositories.Vault;
using Futelo.Shared.DTOs.Invitation;

namespace Futelo.Server.Services.Invitation;

public class InvitationService(IInvitationRepository invitationRepository, IVaultRepository vaultRepository) : IInvitationService
{
    public async Task<InvitationResponse> InviteAsync(int vaultId, string userId, InviteRequest request)
    {
        var vault = await vaultRepository.GetByIdAsync(vaultId)
            ?? throw new KeyNotFoundException("Vault not found.");

        if (vault.OwnerId != userId)
            throw new UnauthorizedAccessException("Only the vault owner can invite players.");

        if (vault.Players.Any(p => p.Player.Email == request.Email))
            throw new InvalidOperationException("This user is already a member of the vault.");

        if (await invitationRepository.HasPendingAsync(vaultId, request.Email))
            throw new InvalidOperationException("A pending invitation already exists for this email.");

        var invitation = new VaultInvitation
        {
            VaultId = vaultId,
            Email = request.Email,
            Token = Guid.NewGuid().ToString("N"),
            Status = InvitationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await invitationRepository.CreateAsync(invitation);

        return MapToResponse(invitation, vault.Name);
    }

    public async Task AcceptAsync(string token, string userId, string userEmail)
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

        if (!invitation.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("This invitation was not sent to your account.");

        if (invitation.Vault.Players.Any(p => p.PlayerId == userId))
            throw new InvalidOperationException("You are already a member of this vault.");

        await vaultRepository.AddPlayerAsync(new VaultPlayer
        {
            VaultId = invitation.VaultId,
            PlayerId = userId,
            JoinedAt = DateTime.UtcNow
        });

        invitation.Status = InvitationStatus.Accepted;
        await invitationRepository.UpdateAsync(invitation);
    }

    private static InvitationResponse MapToResponse(VaultInvitation invitation, string vaultName) => new()
    {
        Id = invitation.Id,
        VaultId = invitation.VaultId,
        VaultName = vaultName,
        Email = invitation.Email,
        Status = invitation.Status.ToString(),
        CreatedAt = invitation.CreatedAt,
        ExpiresAt = invitation.ExpiresAt
    };
}
