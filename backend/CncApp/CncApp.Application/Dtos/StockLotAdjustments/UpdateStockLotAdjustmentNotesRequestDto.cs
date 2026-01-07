using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.StockLotAdjustments;

/// Validation mirrored from Infrastructure.Persistence.Configurations.StockLotAdjustmentConfiguration where applicable.
/// This DTO is for metadata-only updates (Notes field only).
public class UpdateStockLotAdjustmentNotesRequestDto
{
    [MaxLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters.")]
    public string? Notes { get; set; }
}

