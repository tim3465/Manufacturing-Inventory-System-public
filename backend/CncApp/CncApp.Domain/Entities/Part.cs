using System.ComponentModel.DataAnnotations;
namespace CncApp.Domain.Entities;
public class PartBase
{
    [Key]
    public int PartId { get; set; }

    [Required]
    public TimeSpan ApproxPartCycleTime { get; set; }

    [Required]
    public int CheckPerPart { get; set; }
}
public class Part : PartBase
{
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
