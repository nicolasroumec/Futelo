using Futelo.Server.Models;
using Futelo.Server.Repositories.Vault;
using Futelo.Shared.DTOs.Vault;

namespace Futelo.Server.Services.Vault;

public class VaultService(IVaultRepository repository) : IVaultService
{
    public async Task<List<VaultResponse>> GetUserVaultsAsync(string userId)
    {
        var vaults = await repository.GetByUserAsync(userId);
        return vaults.Select(MapToResponse).ToList();
    }

    public async Task<VaultResponse> GetByIdAsync(int id, string userId)
    {
        var vault = await repository.GetByIdAsync(id);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Vault not found.");
        return MapToResponse(vault);
    }

    public async Task<VaultResponse> CreateAsync(string ownerId, CreateVaultRequest request)
    {
        var vault = new Models.Vault
        {
            Name = request.Name,
            OwnerId = ownerId,
            Players = [new VaultPlayer { PlayerId = ownerId, JoinedAt = DateTime.UtcNow }]
        };
        await repository.CreateAsync(vault);
        var created = await repository.GetByIdAsync(vault.Id);
        return MapToResponse(created!);
    }

    public async Task UpdateAsync(int id, string userId, UpdateVaultRequest request)
    {
        var vault = await repository.GetByIdAsync(id);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Vault not found.");
        if (vault.OwnerId != userId)
            throw new UnauthorizedAccessException("Only the vault owner can perform this action.");
        vault.Name = request.Name;
        await repository.UpdateAsync(vault);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var vault = await repository.GetByIdAsync(id);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Vault not found.");
        if (vault.OwnerId != userId)
            throw new UnauthorizedAccessException("Only the vault owner can perform this action.");
        await repository.DeleteAsync(vault);
    }

    private static VaultResponse MapToResponse(Models.Vault vault) => new()
    {
        Id = vault.Id,
        Name = vault.Name,
        OwnerId = vault.OwnerId,
        OwnerDisplayName = vault.Owner.DisplayName,
        Players = vault.Players.Select(p => new VaultPlayerResponse
        {
            PlayerId = p.PlayerId,
            DisplayName = p.Player.DisplayName,
            JoinedAt = p.JoinedAt
        }).ToList()
    };
}
