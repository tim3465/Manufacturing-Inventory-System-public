using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Machines;

/// Validation mirrored from Infrastructure.Persistence.Configurations.MachineConfiguration where applicable.
public class CreateMachineRequestDto
{
    [Required(ErrorMessage = "SerialNumber is required.")]
    [MaxLength(100, ErrorMessage = "SerialNumber cannot exceed 100 characters.")]
    public string SerialNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "ModelNumber is required.")]
    [MaxLength(100, ErrorMessage = "ModelNumber cannot exceed 100 characters.")]
    public string ModelNumber { get; set; } = string.Empty;
}



