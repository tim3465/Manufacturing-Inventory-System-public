using System.ComponentModel.DataAnnotations;
using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Dtos.CloseJob;

public class CloseJobRequestDto
{
    [Required(ErrorMessage = "ShiftId is required.")]
    public int ShiftId { get; set; }

    [Required(ErrorMessage = "JobId is required.")]
    public int JobId { get; set; }

    [Required(ErrorMessage = "ShiftData is required.")]
    public UpdateShiftRequestDto ShiftData { get; set; } = null!;
}
