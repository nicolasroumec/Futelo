namespace Futelo.Shared.DTOs.Season;

public class CreateSeasonRequest
{
    public int VaultId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public int? VideoGameId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
