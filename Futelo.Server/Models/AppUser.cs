using Microsoft.AspNetCore.Identity;

namespace Futelo.Server.Models;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public int EloRating { get; set; } = 1500;

    public ICollection<VaultPlayer> VaultPlayers { get; set; } = [];
    public ICollection<EloHistory> EloHistories { get; set; } = [];
}
