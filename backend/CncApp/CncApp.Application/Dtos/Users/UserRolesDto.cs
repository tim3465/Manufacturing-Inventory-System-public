namespace CncApp.Application.Dtos.Users;

/// <summary>
/// DTO for user role retrieval.
/// </summary>
public class UserRolesDto
{
    public int UserId { get; set; }
    public List<string> Roles { get; set; } = new();
}


