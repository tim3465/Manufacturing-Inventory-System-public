using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Shifts;

/// Validation mirrored from Infrastructure.Persistence.Configurations.ShiftConfiguration where applicable.
public class ShiftDto
{
    public int Id { get; set; }

    // TODO: Add properties based on Shift entity
    // TODO: Add validation attributes matching Infrastructure configuration
}

