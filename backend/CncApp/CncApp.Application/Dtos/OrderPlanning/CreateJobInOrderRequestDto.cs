using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.OrderPlanning;

public class CreateJobInOrderRequestDto
{
    public int? StockLotId { get; set; }

    [Required(ErrorMessage = "MachineId is required.")]
    public int MachineId { get; set; }

    [Required(ErrorMessage = "PartAmountPlanned is required.")]
    public int PartAmountPlanned { get; set; }

    [Required(ErrorMessage = "BarAmountPlanned is required.")]
    public int BarAmountPlanned { get; set; }

    [Required(ErrorMessage = "BarCycleTime is required.")]
    public TimeSpan BarCycleTime { get; set; }

    public int? EstimatedPartsPerBar { get; set; }

    [Required(ErrorMessage = "DueDate is required.")]
    public DateOnly DueDate { get; set; }
}
