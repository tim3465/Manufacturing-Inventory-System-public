using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Jobs;

public class StartJobRequestDto
{
    [Required(ErrorMessage = "BarsToAdd is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "BarsToAdd must be greater than zero.")]
    public int BarsToAdd { get; set; }
}
