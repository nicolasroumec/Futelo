using Futelo.Server.Models;

namespace Futelo.Server.Repositories.Invitation;

public interface IInvitationRepository
{
    Task<VaultInvitation?> GetByTokenAsync(string token);
    Task<bool> HasPendingAsync(int vaultId, string email);
    Task CreateAsync(VaultInvitation invitation);
    Task UpdateAsync(VaultInvitation invitation);
}
