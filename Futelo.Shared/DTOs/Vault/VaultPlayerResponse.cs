using Futelo.Shared.Enums;

namespace Futelo.Shared.DTOs.Vault;

public class VaultPlayerResponse
{
    public string PlayerId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public VaultRole Role { get; set; }
    public int EloRating { get; set; }
}
