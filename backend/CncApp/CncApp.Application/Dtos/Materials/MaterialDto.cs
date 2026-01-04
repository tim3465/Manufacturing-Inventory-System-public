using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Materials;

/// Validation mirrored from Infrastructure.Persistence.Configurations.MaterialConfiguration where applicable.
public class MaterialDto
{
    public int Id { get; set; }

    // TODO: Add properties based on Material entity
    // TODO: Add validation attributes matching Infrastructure configuration
}

