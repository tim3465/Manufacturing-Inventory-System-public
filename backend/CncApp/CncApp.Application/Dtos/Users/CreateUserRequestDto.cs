namespace CncApp.Application.Dtos.Users;

/// <summary>
/// DTO for creating a new user (both Identity and Domain user).
/// </summary>
public class CreateUserRequestDto
{
    /// <summary>
    /// Email address for the Identity user (required, used for authentication).
    /// This will also be used as the UserName in Identity (UserName = Email).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name for the Domain user.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name for the Domain user.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Temporary password for the Identity user.
    /// In production, this should be generated securely or sent via secure channel.
    /// </summary>
    public string TemporaryPassword { get; set; } = string.Empty;

    /// <summary>
    /// Initial Identity roles to assign to the user.
    /// These roles are the source of truth for authorization.
    /// </summary>
    public List<string> Roles { get; set; } = new();
}

