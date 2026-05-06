namespace Futelo.Client.Services.Invitation;

public interface IInvitationService
{
    Task AcceptAsync(string token);
}
