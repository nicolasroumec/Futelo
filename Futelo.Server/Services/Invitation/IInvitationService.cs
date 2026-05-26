using Futelo.Shared.DTOs.Invitation;

namespace Futelo.Server.Services.Invitation;

public interface IInvitationService
{
    Task<InvitationResponse> InviteAsync(int vaultId, string userId, InviteRequest request);
    Task AcceptAsync(string token, string userId);
    Task<InvitationResponse> GetPreviewAsync(string token);
}
