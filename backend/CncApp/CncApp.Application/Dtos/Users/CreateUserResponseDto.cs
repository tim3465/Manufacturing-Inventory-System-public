using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Users;

/// <summary>
/// DTO returned after successfully creating a user.
/// </summary>
/// Validation mirrored from Infrastructure.Persistence.Configurations.UserConfiguration where applicable.
/// For Identity-owned fields (Email/Password), validation is based on Identity input requirements.
public class CreateUserResponseDto
{
    /// <summary>
    /// The Identity UserId (from ASP.NET Core Identity).
    /// </summary>
    [Required(ErrorMessage = "IdentityUserId is required.")]
    public int IdentityUserId { get; set; }

    /// <summary>
    /// The Domain UserId (from Domain User entity).
    /// </summary>
    [Required(ErrorMessage = "DomainUserId is required.")]
    public int DomainUserId { get; set; }

    /// <summary>
    /// Username of the created user.
    /// </summary>
    [MaxLength(200, ErrorMessage = "UserName cannot exceed 200 characters.")]
    public string UserName { get; set; } = string.Empty;
}

