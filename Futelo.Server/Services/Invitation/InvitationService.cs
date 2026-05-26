using Futelo.Server.Models;
using Futelo.Server.Repositories.Invitation;
using Futelo.Server.Repositories.Vault;
using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.Enums;

namespace Futelo.Server.Services.Invitation;

using static ErrorMessages;

public class InvitationService(IInvitationRepository invitationRepository, IVaultRepository vaultRepository) : IInvitationService
{
    public async Task<InvitationResponse> InviteAsync(int vaultId, string userId, InviteRequest request)
    {
        var vault = await vaultRepository.GetByIdAsync(vaultId)
            ?? throw new KeyNotFoundException(VaultNotFound);

        var caller = vault.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (caller == null || caller.Role != VaultRole.Admin)
            throw new UnauthorizedAccessException(OnlyAdminsCanInvite);

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
            ?? throw new KeyNotFoundException(InvitationNotFound);

        if (invitation.Status != InvitationStatus.Pending)
            throw new InvalidOperationException(InvitationNoLongerValid);

        if (invitation.ExpiresAt < DateTime.UtcNow)
        {
            invitation.Status = InvitationStatus.Expired;
            await invitationRepository.UpdateAsync(invitation);
            throw new InvalidOperationException(InvitationExpired);
        }

        if (invitation.Vault.Players.Any(p => p.PlayerId == userId))
            throw new InvalidOperationException(AlreadyVaultMember);

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

    public async Task<InvitationResponse> GetPreviewAsync(string token)
    {
        var invitation = await invitationRepository.GetByTokenAsync(token)
            ?? throw new KeyNotFoundException(InvitationNotFound);
        return MapToResponse(invitation, invitation.Vault.Name);
    }

    private static InvitationResponse MapToResponse(VaultInvitation invitation, string vaultName) => new()
    {
        Id = invitation.Id,
        VaultId = invitation.VaultId,
        VaultName = vaultName,
        Token = invitation.Token,
        Role = invitation.Role.ToString(),
        Status = invitation.Status.ToString(),
        CreatedAt = invitation.CreatedAt,
        ExpiresAt = invitation.ExpiresAt
    };
}
