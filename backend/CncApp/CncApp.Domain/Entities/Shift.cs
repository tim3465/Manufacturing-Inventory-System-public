using System.ComponentModel.DataAnnotations;
namespace CncApp.Domain.Entities;
public class ShiftBase
{
    [Key]
    public int ShiftId { get; set; }

    [Required]
    public int JobId { get; set; }

    [Required]
    public int OperatorId { get; set; } // UserId

    public int PartsMade { get; set; }

    public int Scrap { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? StopTime { get; set; }

    public TimeSpan? Downtime { get; set; }
    }

public class Shift : ShiftBase
{
    public Job Job { get; set; } = null!;

    public User Operator { get; set; } = null!;
}