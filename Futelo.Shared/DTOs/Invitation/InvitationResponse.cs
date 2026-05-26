namespace Futelo.Shared.DTOs.Invitation;

public class InvitationResponse
{
    public int Id { get; set; }
    public int VaultId { get; set; }
    public string VaultName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
