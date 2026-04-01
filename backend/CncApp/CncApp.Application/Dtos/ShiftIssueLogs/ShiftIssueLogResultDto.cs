using System.ComponentModel.DataAnnotations;
using CncApp.Domain.Enums;

namespace CncApp.Application.Dtos.ShiftIssueLogs;

/// Validation mirrored from Infrastructure.Persistence.Configurations.ShiftIssueLogConfiguration where applicable.
public class ShiftIssueLogResultDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ShiftId is required.")]
    public int ShiftId { get; set; }

    [Required(ErrorMessage = "IssueType is required.")]
    public IssueTypeEnum IssueType { get; set; }

    public int ScrapQuantity { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    public TimeSpan? Downtime { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }
}
