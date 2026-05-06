using System.ComponentModel.DataAnnotations;
using Futelo.Shared.Enums;

namespace Futelo.Shared.DTOs.Invitation;

public class InviteRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public VaultRole Role { get; set; } = VaultRole.Viewer;
}
