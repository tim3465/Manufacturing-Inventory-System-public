using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Parts;

/// Validation mirrored from Infrastructure.Persistence.Configurations.PartConfiguration where applicable.
public class PartDto
{
    public int Id { get; set; }

    // TODO: Add properties based on Part entity
    // TODO: Add validation attributes matching Infrastructure configuration
}

