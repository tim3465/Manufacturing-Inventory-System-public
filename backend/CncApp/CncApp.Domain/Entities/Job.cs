using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Job : AuditableEntityBase
{
    public int OrderId { get; set; }

    public int StockLotId { get; set; }

    public int MachineId { get; set; }

    public int PartAmountPlanned { get; set; }

    public int BarAmountPlanned { get; set; }

    public TimeSpan BarCycleTime { get; set; }

    public Order Order { get; set; } = null!;

    public StockLot StockLot { get; set; } = null!;

    public Machine Machine { get; set; } = null!;

    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
