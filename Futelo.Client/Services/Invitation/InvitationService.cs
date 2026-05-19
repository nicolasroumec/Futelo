namespace Futelo.Client.Services.Invitation;

public class InvitationService(HttpClient http) : ApiService(http), IInvitationService
{
    public Task AcceptAsync(string token)
        => PostAsync($"api/invitations/{token}/accept");
}
