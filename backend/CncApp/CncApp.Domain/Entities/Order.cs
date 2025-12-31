using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Order : AuditableEntityBase
{
    public int PartId { get; set; }

    public int CustomerId { get; set; }

    public int PartAmountRequested { get; set; }

    public int PartsPerBar { get; set; }

    public Part Part { get; set; } = null!;

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
