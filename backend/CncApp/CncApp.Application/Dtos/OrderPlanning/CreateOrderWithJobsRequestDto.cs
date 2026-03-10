using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.OrderPlanning;

public class CreateOrderWithJobsRequestDto
{
    [Required(ErrorMessage = "CustomerId is required.")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "PartId is required.")]
    public int PartId { get; set; }

    [Required(ErrorMessage = "PartAmountRequested is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "PartAmountRequested must be at least 1.")]
    public int PartAmountRequested { get; set; }

    public int PartsPerBar { get; set; }

    [Required(ErrorMessage = "Jobs is required.")]
    [MinLength(1, ErrorMessage = "At least one job must be provided.")]
    public List<CreateJobInOrderRequestDto> Jobs { get; set; } = new();
}
