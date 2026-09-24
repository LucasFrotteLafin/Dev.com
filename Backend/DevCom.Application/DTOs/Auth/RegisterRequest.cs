using System.ComponentModel.DataAnnotations;
using DevCom.Domain.Enums;

namespace DevCom.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required, MinLength(2)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }
}
