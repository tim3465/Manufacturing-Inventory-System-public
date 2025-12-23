using System.ComponentModel.DataAnnotations;
namespace CncApp.Domain.Entities;
public class OrderBase
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    public int PartId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int PartAmountRequested { get; set; }

    public int PartsPerBar { get; set; }
}
public class Order
{
    public Part Part { get; set; } = null!;

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
