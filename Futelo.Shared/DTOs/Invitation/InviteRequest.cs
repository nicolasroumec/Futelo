using System.ComponentModel.DataAnnotations;

namespace Futelo.Shared.DTOs.Invitation;

public class InviteRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}
