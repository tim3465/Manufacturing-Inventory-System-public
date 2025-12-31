using CncApp.Domain.Common;
using CncApp.Domain.Enums;

namespace CncApp.Domain.Entities;

public class StockLotAdjustment : AuditableEntityBase
{
    public int StockLotId { get; set; }

    public int? JobId { get; set; }

    public int DeltaBars { get; set; }

    public StockLotAdjustmentReasonEnum Reason { get; set; }

    public string? Notes { get; set; }

    public StockLot StockLot { get; set; } = null!;
}
