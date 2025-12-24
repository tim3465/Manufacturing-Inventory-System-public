using System.ComponentModel.DataAnnotations;
namespace CncApp.Domain.Entities;
using CncApp.Domain.Enums;
public class StockLotBase
{
    [Key]
    public int StockLotId { get; set; }

    [Required, MaxLength(100)]
    public string LotNumber { get; set; } = string.Empty;

    [Required]
    public int MaterialId { get; set; }

    [Required]
    public int AmountOfBars { get; set; }

    [Required]
    public decimal Diameter { get; set; }

    [Required]
    public decimal BarLength { get; set; }

    [Required]
    public StockLotConditionEnum Condition { get; set; }

    [Required]
    public DateTime CheckedInDateTime { get; set; }
    }
public class StockLot : StockLotBase
{

    public Material Material { get; set; } = null!;

    public ICollection<StockLotAdjustment> StockLotAdjustments { get; set; } = new List<StockLotAdjustment>();

}
