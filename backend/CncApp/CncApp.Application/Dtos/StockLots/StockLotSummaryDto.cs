using CncApp.Domain.Enums;

namespace CncApp.Application.Dtos.StockLots;

public class StockLotSummaryDto
{
    public int Id { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public int AmountOfBars { get; set; }
    public decimal Diameter { get; set; }
    public decimal BarLength { get; set; }
    public StockLotConditionEnum Condition { get; set; }
    public DateTimeOffset CheckedInDateTime { get; set; }
}
