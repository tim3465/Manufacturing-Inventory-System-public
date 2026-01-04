using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Users;

/// Validation mirrored from Infrastructure.Persistence.Configurations.UserConfiguration where applicable.
public class UserDto
{
    public int Id { get; set; }

    // TODO: Add properties based on User entity
    // TODO: Add validation attributes matching Infrastructure configuration
}

