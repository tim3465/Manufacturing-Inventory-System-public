using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Dtos.Jobs;

public class JobProductionDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public DateOnly DueDate { get; set; }

    public int MachineId { get; set; }

    public string MachineName { get; set; } = string.Empty;

    public int PartAmountPlanned { get; set; }

    public string PartName { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public int PartsCompleted { get; set; }

    public decimal PercentComplete { get; set; }

    public int? StockLotId { get; set; }

    public string? LotNumber { get; set; }

    public List<ShiftDto> Shifts { get; set; } = new();
}
