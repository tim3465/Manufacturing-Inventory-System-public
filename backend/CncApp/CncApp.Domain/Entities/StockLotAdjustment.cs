
using CncApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using CncApp.Domain.Enums;
namespace CncApp.Domain.Entities;
public class StockLotAdjustmentBase
{
    [Key]
    public int StockLotAdjustmentId { get; set; }

    [Required]
    public int StockLotId { get; set; }

    // Optional link to a job consumption event (nullable by design)
    public int? JobId { get; set; }

    [Required]
    public int DeltaBars { get; set; } // positive = received, negative = consumed

    [Required]
    public StockLotAdjustmentReasonEnum Reason { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Required]
    public AuditTrail AuditTrail { get; set; } = new();

}
public class StockLotAdjustment : StockLotAdjustmentBase
{
    public StockLot StockLot { get; set; } = null!;
    // Add later when Job exists if you want:
    // public Job? Job { get; set; }
}
