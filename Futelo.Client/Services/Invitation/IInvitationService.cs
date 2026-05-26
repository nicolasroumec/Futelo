using Futelo.Shared.DTOs.Invitation;

namespace Futelo.Client.Services.Invitation;

public interface IInvitationService
{
    Task<InvitationResponse> GetPreviewAsync(string token);
    Task AcceptAsync(string token);
}
