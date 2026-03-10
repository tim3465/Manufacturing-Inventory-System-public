using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Parts;

public class UpdatePartRequestDto
{
    [MaxLength(100, ErrorMessage = "PartName cannot exceed 100 characters.")]
    public string? PartName { get; set; }

    [MaxLength(50, ErrorMessage = "PartNumber cannot exceed 50 characters.")]
    public string? PartNumber { get; set; }

    public TimeSpan? ApproxPartCycleTime { get; set; }

    public int? CheckPerPart { get; set; }
}

