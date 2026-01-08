namespace CncApp.Application.Dtos.Users;

/// <summary>
/// Domain-facing representation of a user/operator (no Identity secrets).
/// Active when InactivatedDateTime is null.
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset? InactivatedDateTime { get; set; }
}

