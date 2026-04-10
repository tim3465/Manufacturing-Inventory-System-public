using System.ComponentModel.DataAnnotations;
using CncApp.Domain.Enums;

namespace CncApp.Application.Dtos.ShippingReceiving;

public class ReceiveShipmentRequestDto
{
    public int? MaterialId { get; set; }

    [MaxLength(100, ErrorMessage = "HeatNumber cannot exceed 100 characters.")]
    public string? HeatNumber { get; set; }

    [MaxLength(100, ErrorMessage = "MaterialName cannot exceed 100 characters.")]
    public string? MaterialName { get; set; }

    [Required(ErrorMessage = "LotNumber is required.")]
    [MaxLength(100, ErrorMessage = "LotNumber cannot exceed 100 characters.")]
    public string LotNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "AmountOfBars is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "AmountOfBars must be at least 1.")]
    public int AmountOfBars { get; set; }

    [Required(ErrorMessage = "Diameter is required.")]
    public decimal Diameter { get; set; }

    [Required(ErrorMessage = "BarLength is required.")]
    public decimal BarLength { get; set; }

    [Required(ErrorMessage = "Condition is required.")]
    public StockLotConditionEnum Condition { get; set; }

    [Required(ErrorMessage = "CheckedInDateTime is required.")]
    public DateTime CheckedInDateTime { get; set; }

    [MaxLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters.")]
    public string? Notes { get; set; }
}
