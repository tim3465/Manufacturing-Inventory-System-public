using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Material : AuditableEntityBase
{
    public string HeatNumber { get; set; } = string.Empty;

    public string MaterialName { get; set; } = string.Empty;

    public ICollection<StockLot> StockLots { get; set; } = new List<StockLot>();
}
