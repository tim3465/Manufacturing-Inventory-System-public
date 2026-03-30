using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Shifts;

public class UpdateShiftRequestDto
{
    [Required]
    public DateTime StartTime { get; set; }

    public DateTime? StopTime { get; set; }

    [Required]
    public int PartsMade { get; set; }

    [Required]
    public int BarsConsumed { get; set; }

    public int? PartsPerBar { get; set; }
}
