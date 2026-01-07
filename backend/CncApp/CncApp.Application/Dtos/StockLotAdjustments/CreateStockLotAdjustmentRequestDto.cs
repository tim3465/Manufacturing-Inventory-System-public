using System.ComponentModel.DataAnnotations;
using CncApp.Domain.Enums;

namespace CncApp.Application.Dtos.StockLotAdjustments;

/// Validation mirrored from Infrastructure.Persistence.Configurations.StockLotAdjustmentConfiguration where applicable.
public class CreateStockLotAdjustmentRequestDto
{
    [Required(ErrorMessage = "StockLotId is required.")]
    public int StockLotId { get; set; }

    public int? JobId { get; set; }

    [Required(ErrorMessage = "DeltaBars is required.")]
    public int DeltaBars { get; set; }

    [Required(ErrorMessage = "Reason is required.")]
    public StockLotAdjustmentReasonEnum Reason { get; set; }

    [MaxLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters.")]
    public string? Notes { get; set; }
}

