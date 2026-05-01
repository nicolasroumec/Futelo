namespace Futelo.Server.Models;

public class CupPlayer
{
    public int CupId { get; set; }
    public string PlayerId { get; set; } = string.Empty;

    public Cup Cup { get; set; } = null!;
    public AppUser Player { get; set; } = null!;
}
