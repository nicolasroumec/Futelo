using Futelo.Server.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Futelo.Server.Models;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public int EloRating { get; set; } = EloCalculator.InitialElo;

    public byte[]? Avatar { get; set; }

    public ICollection<VaultPlayer> VaultPlayers { get; set; } = [];
    public ICollection<EloHistory> EloHistories { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
