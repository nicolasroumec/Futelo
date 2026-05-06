namespace Futelo.Client.Services.Invitation;

public class InvitationService(HttpClient http) : IInvitationService
{
    public async Task AcceptAsync(string token)
    {
        var response = await http.PostAsync($"api/invitations/{token}/accept", null);
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(message) ? "Failed to accept invitation." : message);
        }
    }
}
