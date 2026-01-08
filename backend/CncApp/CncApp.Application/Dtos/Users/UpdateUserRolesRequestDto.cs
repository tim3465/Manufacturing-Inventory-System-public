using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Users;

/// <summary>
/// Request to replace the roles assigned to an Identity user.
/// </summary>
public class UpdateUserRolesRequestDto
{
    [Required]
    public List<string> Roles { get; set; } = new();
}

