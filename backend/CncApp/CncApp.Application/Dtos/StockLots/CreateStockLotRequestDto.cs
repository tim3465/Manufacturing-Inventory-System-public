using System.ComponentModel.DataAnnotations;
using CncApp.Domain.Enums;

namespace CncApp.Application.Dtos.StockLots;

/// Validation mirrored from Infrastructure.Persistence.Configurations.StockLotConfiguration where applicable.
public class CreateStockLotRequestDto
{
    [Required(ErrorMessage = "LotNumber is required.")]
    [MaxLength(100, ErrorMessage = "LotNumber cannot exceed 100 characters.")]
    public string LotNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "MaterialId is required.")]
    public int MaterialId { get; set; }

    [Required(ErrorMessage = "AmountOfBars is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "AmountOfBars must be non-negative.")]
    public int AmountOfBars { get; set; }

    [Required(ErrorMessage = "Diameter is required.")]
    public decimal Diameter { get; set; }

    [Required(ErrorMessage = "BarLength is required.")]
    public decimal BarLength { get; set; }

    [Required(ErrorMessage = "Condition is required.")]
    public StockLotConditionEnum Condition { get; set; }

    [Required(ErrorMessage = "CheckedInDateTime is required.")]
    public DateTime CheckedInDateTime { get; set; }
}

