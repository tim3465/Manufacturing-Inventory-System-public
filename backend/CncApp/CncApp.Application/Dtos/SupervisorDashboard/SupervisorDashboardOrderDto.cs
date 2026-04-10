namespace CncApp.Application.Dtos.SupervisorDashboard;

public class SupervisorDashboardOrderDto
{
    public int OrderId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int Target { get; set; }
    public int GoodParts { get; set; }
    public int Scrap { get; set; }
}
