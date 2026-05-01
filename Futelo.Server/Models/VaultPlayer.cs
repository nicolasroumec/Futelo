namespace Futelo.Server.Models;

public class VaultPlayer
{
    public int VaultId { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }

    public Vault Vault { get; set; } = null!;
    public AppUser Player { get; set; } = null!;
}
