using System.ComponentModel.DataAnnotations;

namespace Futelo.Shared.DTOs.VideoGame;

public class CreateVideoGameRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
