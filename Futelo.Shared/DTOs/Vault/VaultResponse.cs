namespace Futelo.Shared.DTOs.Vault;

public class VaultResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string OwnerDisplayName { get; set; } = string.Empty;
    public bool HasActiveSeason { get; set; }
    public int? ActiveSeasonId { get; set; }
    public List<VaultPlayerResponse> Players { get; set; } = [];
}
