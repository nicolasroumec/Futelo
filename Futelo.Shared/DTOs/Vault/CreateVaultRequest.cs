using System.ComponentModel.DataAnnotations;

namespace Futelo.Shared.DTOs.Vault;

public class CreateVaultRequest
{
    [Required, MinLength(3), MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
