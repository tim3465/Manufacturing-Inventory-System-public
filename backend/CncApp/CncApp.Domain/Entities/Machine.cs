using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Machine : AuditableEntityBase
{
    public string SerialNumber { get; set; } = string.Empty;

    public string ModelNumber { get; set; } = string.Empty;

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
