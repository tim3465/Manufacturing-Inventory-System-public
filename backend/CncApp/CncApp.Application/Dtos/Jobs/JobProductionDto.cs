using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Dtos.Jobs;

public class JobProductionDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public DateOnly DueDate { get; set; }

    public int MachineId { get; set; }

    public int PartAmountPlanned { get; set; }

    public List<ShiftDto> Shifts { get; set; } = new();
}
