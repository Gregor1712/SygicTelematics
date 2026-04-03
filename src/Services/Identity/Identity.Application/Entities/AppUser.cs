using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Entities;

public class AppUser : IdentityUser
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }
}