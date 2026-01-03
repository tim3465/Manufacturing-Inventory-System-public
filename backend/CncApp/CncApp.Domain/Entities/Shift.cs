using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Shift : AuditableEntityBase
{
    public int JobId { get; set; }

    public int OperatorId { get; set; }

    public int PartsMade { get; set; }

    public int Scrap { get; set; }

    public int BarsConsumed { get; set; }

    public int? PartsPerBar { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? StopTime { get; set; }

    public TimeSpan? Downtime { get; set; }

    public Job Job { get; set; } = null!;

    public User Operator { get; set; } = null!;
}
