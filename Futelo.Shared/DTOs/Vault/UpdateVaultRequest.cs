using System.ComponentModel.DataAnnotations;

namespace Futelo.Shared.DTOs.Vault;

public class UpdateVaultRequest
{
    [Required, MinLength(3), MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
