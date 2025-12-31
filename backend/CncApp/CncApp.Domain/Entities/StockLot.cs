using CncApp.Domain.Common;
using CncApp.Domain.Enums;

namespace CncApp.Domain.Entities;

public class StockLot : AuditableEntityBase
{
    public string LotNumber { get; set; } = string.Empty;

    public int MaterialId { get; set; }

    public int AmountOfBars { get; set; }

    public decimal Diameter { get; set; }

    public decimal BarLength { get; set; }

    public StockLotConditionEnum Condition { get; set; }

    public DateTime CheckedInDateTime { get; set; }

    public Material Material { get; set; } = null!;

    public ICollection<StockLotAdjustment> StockLotAdjustments { get; set; } = new List<StockLotAdjustment>();
}
