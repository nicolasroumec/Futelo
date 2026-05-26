using Futelo.Shared.DTOs.Invitation;

namespace Futelo.Client.Services.Invitation;

public class InvitationService(HttpClient http) : ApiService(http), IInvitationService
{
    public Task<InvitationResponse> GetPreviewAsync(string token)
        => GetAsync<InvitationResponse>($"api/invitations/{token}");

    public Task AcceptAsync(string token)
        => PostAsync($"api/invitations/{token}/accept");
}
