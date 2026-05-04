using Futelo.Shared.DTOs.Invitation;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Client.Services.Vault;

public interface IVaultService
{
    Task<List<VaultResponse>> GetAllAsync();
    Task<VaultResponse> GetByIdAsync(int id);
    Task<VaultResponse> CreateAsync(CreateVaultRequest request);
    Task UpdateAsync(int id, UpdateVaultRequest request);
    Task DeleteAsync(int id);
    Task<InvitationResponse> InviteAsync(int vaultId, InviteRequest request);
}
