using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Users;

/// <summary>
/// DTO for creating a new user (both Identity and Domain user).
/// </summary>
/// Validation mirrored from Infrastructure.Persistence.Configurations.UserConfiguration where applicable.
/// For Identity-owned fields (Email/Password), validation is based on Identity input requirements.
public class CreateUserRequestDto
{
    /// <summary>
    /// Email address for the Identity user (required, used for authentication).
    /// This will also be used as the UserName in Identity (UserName = Email).
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    [MaxLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name for the Domain user.
    /// </summary>
    [MaxLength(200, ErrorMessage = "FirstName cannot exceed 200 characters.")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name for the Domain user.
    /// </summary>
    [MaxLength(200, ErrorMessage = "LastName cannot exceed 200 characters.")]
    public string? LastName { get; set; }

    /// <summary>
    /// Temporary password for the Identity user.
    /// In production, this should be generated securely or sent via secure channel.
    /// </summary>
    [Required(ErrorMessage = "TemporaryPassword is required.")]
    public string TemporaryPassword { get; set; } = string.Empty;

    /// <summary>
    /// Initial Identity roles to assign to the user.
    /// These roles are the source of truth for authorization.
    /// </summary>
    public List<string> Roles { get; set; } = new();
}

