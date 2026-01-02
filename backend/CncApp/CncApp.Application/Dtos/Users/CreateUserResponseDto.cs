namespace CncApp.Application.Dtos.Users;

/// <summary>
/// DTO returned after successfully creating a user.
/// </summary>
public class CreateUserResponseDto
{
    /// <summary>
    /// The Identity UserId (from ASP.NET Core Identity).
    /// </summary>
    public int IdentityUserId { get; set; }

    /// <summary>
    /// The Domain UserId (from Domain User entity).
    /// </summary>
    public int DomainUserId { get; set; }

    /// <summary>
    /// Email address of the created user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Username of the created user.
    /// </summary>
    public string UserName { get; set; } = string.Empty;
}

