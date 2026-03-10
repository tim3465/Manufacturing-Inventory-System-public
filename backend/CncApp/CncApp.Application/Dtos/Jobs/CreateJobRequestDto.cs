using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Jobs;

/// Validation mirrored from Infrastructure.Persistence.Configurations.JobConfiguration where applicable.
public class CreateJobRequestDto
{
    [Required(ErrorMessage = "OrderId is required.")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "StockLotId is required.")]
    public int StockLotId { get; set; }

    [Required(ErrorMessage = "MachineId is required.")]
    public int MachineId { get; set; }

    [Required(ErrorMessage = "PartAmountPlanned is required.")]
    public int PartAmountPlanned { get; set; }

    [Required(ErrorMessage = "BarAmountPlanned is required.")]
    public int BarAmountPlanned { get; set; }

    [Required(ErrorMessage = "BarCycleTime is required.")]
    public TimeSpan BarCycleTime { get; set; }

    [Required(ErrorMessage = "BarsInJob is required.")]
    public int BarsInJob { get; set; }

    public int? EstimatedPartsPerBar { get; set; }

    [Required(ErrorMessage = "DueDate is required.")]
    public DateOnly DueDate { get; set; }
}

