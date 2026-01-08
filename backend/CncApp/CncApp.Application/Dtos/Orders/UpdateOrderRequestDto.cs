using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Orders;

/// Validation mirrored from Infrastructure.Persistence.Configurations.OrderConfiguration where applicable.
/// Update is metadata-only (planning fields only).
public class UpdateOrderRequestDto
{
    [Required(ErrorMessage = "PartId is required.")]
    public int PartId { get; set; }

    [Required(ErrorMessage = "CustomerId is required.")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "PartAmountRequested is required.")]
    public int PartAmountRequested { get; set; }

    public int PartsPerBar { get; set; }
}

