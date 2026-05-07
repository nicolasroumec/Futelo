using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Base;
using Futelo.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Futelo.Server.Repositories.Invitation;

public class InvitationRepository(FuteloContext context) : BaseRepository<VaultInvitation>(context), IInvitationRepository
{
    public async Task<VaultInvitation?> GetByTokenAsync(string token)
        => await Context.Set<VaultInvitation>()
            .Include(i => i.Vault).ThenInclude(v => v.Players)
            .FirstOrDefaultAsync(i => i.Token == token);

    public async Task<bool> HasPendingAsync(int vaultId, string email)
        => await Context.Set<VaultInvitation>()
            .AnyAsync(i => i.VaultId == vaultId && i.Email == email && i.Status == InvitationStatus.Pending);

    public async Task CreateAsync(VaultInvitation invitation)
    {
        Create(invitation);
        await SaveChangesAsync();
    }

    public async Task UpdateAsync(VaultInvitation invitation)
    {
        Update(invitation);
        await SaveChangesAsync();
    }
}
