namespace CncApp.Application.Dtos.Orders;

public class OrderProductionDto
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string PartName { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public int PartAmountRequested { get; set; }

    public int PartAmountCompleted { get; set; }

    public double PercentComplete { get; set; }
}
