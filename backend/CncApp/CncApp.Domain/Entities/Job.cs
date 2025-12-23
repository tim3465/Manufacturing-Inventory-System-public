using System.ComponentModel.DataAnnotations;
namespace CncApp.Domain.Entities;
public class JobBase
{
    [Key]
    public int JobId { get; set; }

    [Required]
    public int OrderId { get; set; }

    [Required]
    public int StockLotId { get; set; }

    [Required]
    public int MachineId { get; set; }

    [Required]
    public int PartAmountPlanned { get; set; }

    [Required]
    public int BarAmountPlanned { get; set; }

    [Required]
    public TimeSpan BarCycleTime { get; set; }

}
public class Job : JobBase
{
    public Order Order { get; set; } = null!;

    public StockLot StockLot { get; set; } = null!;

    [Required]
    public Machine Machine { get; set; } = null!;

    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
