using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Shifts;

/// Validation mirrored from Infrastructure.Persistence.Configurations.ShiftConfiguration.
public class ShiftResultDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "JobId is required.")]
    public int JobId { get; set; }

    [Required(ErrorMessage = "OperatorId is required.")]
    public int OperatorId { get; set; }

    [Required(ErrorMessage = "BarsConsumed is required.")]
    public int BarsConsumed { get; set; }

    public int PartsMade { get; set; }

    public int Scrap { get; set; }

    public int? PartsPerBar { get; set; }

    [Required(ErrorMessage = "StartTime is required.")]
    public DateTime StartTime { get; set; }

    public DateTime? StopTime { get; set; }

    public TimeSpan? Downtime { get; set; }
}

