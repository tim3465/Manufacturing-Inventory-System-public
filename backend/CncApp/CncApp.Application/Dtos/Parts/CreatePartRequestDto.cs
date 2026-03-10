using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Parts;

public class CreatePartRequestDto
{
    [Required(ErrorMessage = "PartName is required.")]
    [MaxLength(100, ErrorMessage = "PartName cannot exceed 100 characters.")]
    public string PartName { get; set; } = string.Empty;

    [Required(ErrorMessage = "PartNumber is required.")]
    [MaxLength(50, ErrorMessage = "PartNumber cannot exceed 50 characters.")]
    public string PartNumber { get; set; } = string.Empty;

    public TimeSpan ApproxPartCycleTime { get; set; }

    public int CheckPerPart { get; set; }
}

