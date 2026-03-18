using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Shifts;

/// Read/return model for Shifts (used by Get/List/ListAll).
public class ShiftDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "JobId is required.")]
    public int JobId { get; set; }

    [Required(ErrorMessage = "OperatorId is required.")]
    public int OperatorId { get; set; }

    public string OperatorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "BarsConsumed is required.")]
    public int BarsConsumed { get; set; }

    public int PartsMade { get; set; }

    public int Scrap { get; set; }

    public int? PartsPerBar { get; set; }

    [Required(ErrorMessage = "StartTime is required.")]
    public DateTime StartTime { get; set; }

    public DateTime? StopTime { get; set; }

    public TimeSpan? Downtime { get; set; }

    public string PartName { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;
}

