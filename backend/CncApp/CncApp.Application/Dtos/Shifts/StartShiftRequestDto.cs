using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Shifts;

public class StartShiftRequestDto
{
    [Required]
    public int JobId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }
}
