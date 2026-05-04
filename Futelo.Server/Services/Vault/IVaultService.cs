using Futelo.Shared.DTOs.Vault;

namespace Futelo.Server.Services.Vault;

public interface IVaultService
{
    Task<List<VaultResponse>> GetUserVaultsAsync(string userId);
    Task<VaultResponse> GetByIdAsync(int id, string userId);
    Task<VaultResponse> CreateAsync(string ownerId, CreateVaultRequest request);
    Task UpdateAsync(int id, string userId, UpdateVaultRequest request);
    Task DeleteAsync(int id, string userId);
}
