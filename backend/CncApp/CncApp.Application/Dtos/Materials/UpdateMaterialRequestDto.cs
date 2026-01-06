using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Materials;

/// Validation mirrored from Infrastructure.Persistence.Configurations.MaterialConfiguration where applicable.
/// Update DTO for metadata-only updates (HeatNumber, MaterialName).
public class UpdateMaterialRequestDto
{
    [Required(ErrorMessage = "HeatNumber is required.")]
    [MaxLength(100, ErrorMessage = "HeatNumber cannot exceed 100 characters.")]
    public string HeatNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "MaterialName is required.")]
    [MaxLength(100, ErrorMessage = "MaterialName cannot exceed 100 characters.")]
    public string MaterialName { get; set; } = string.Empty;
}

