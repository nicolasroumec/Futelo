using Futelo.Server.Models;

namespace Futelo.Server.Repositories.Vault;

public interface IVaultRepository
{
    Task<IEnumerable<Models.Vault>> GetByUserAsync(string userId);
    Task<Models.Vault?> GetByIdAsync(int id);
    Task<List<Match>> GetRecentMatchesAsync(int vaultId, int limit);
    Task<List<Match>> GetMatchesPageAsync(int vaultId, int skip, int take);
    Task<int> CountMatchesAsync(int vaultId);
    Task CreateAsync(Models.Vault vault);
    Task UpdateAsync(Models.Vault vault);
    Task DeleteAsync(Models.Vault vault);
    Task AddPlayerAsync(VaultPlayer player);
}
