using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Jobs;

/// Validation for PATCH /api/jobs/{id} - metadata-only (planning fields only).
public class UpdateJobRequestDto
{
    public int? MachineId { get; set; }

    public int? StockLotId { get; set; }

    public int? PartAmountPlanned { get; set; }

    public int? BarAmountPlanned { get; set; }

    public TimeSpan? BarCycleTime { get; set; }

    public int? BarsInJob { get; set; }

    public int? EstimatedPartsPerBar { get; set; }

    public DateOnly? DueDate { get; set; }
}

