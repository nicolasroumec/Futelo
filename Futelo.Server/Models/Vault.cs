namespace Futelo.Server.Models;

public class Vault
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;

    public AppUser Owner { get; set; } = null!;
    public ICollection<VaultPlayer> Players { get; set; } = [];
    public ICollection<VaultInvitation> Invitations { get; set; } = [];
    public ICollection<Season> Seasons { get; set; } = [];
}
