using System.ComponentModel.DataAnnotations;

namespace Futelo.Shared.DTOs.Team;

public class CreateTeamRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
