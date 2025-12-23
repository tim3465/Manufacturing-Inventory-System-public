using System.ComponentModel.DataAnnotations;
namespace CncApp.Domain.Entities;
public class MachineBase
{
    [Key]
    public int MachineId { get; set; }

    [Required, MaxLength(100)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string ModelNumber { get; set; } = string.Empty;
}
public class Machine : MachineBase
{
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}