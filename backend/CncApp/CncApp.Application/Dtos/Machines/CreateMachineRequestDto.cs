using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Machines;

/// Validation mirrored from Infrastructure.Persistence.Configurations.MachineConfiguration where applicable.
public class CreateMachineRequestDto
{
    [Required]
    [MaxLength(100)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ModelNumber { get; set; } = string.Empty;
}



