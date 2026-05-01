namespace Futelo.Server.Models;

public class VaultInvitation
{
    public int Id { get; set; }
    public int VaultId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public Vault Vault { get; set; } = null!;
}
