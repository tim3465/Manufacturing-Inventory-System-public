using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Part : AuditableEntityBase
{
    public TimeSpan ApproxPartCycleTime { get; set; }

    public int CheckPerPart { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
