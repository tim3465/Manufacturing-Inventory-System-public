namespace CncApp.Application.Dtos.SupervisorDashboard;

public class SupervisorDashboardOperatorDto
{
    public int OperatorId { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public int MachinesRunning { get; set; }
    public List<SupervisorDashboardActiveJobDto> ActiveJobs { get; set; } = new();
    public int PartsMadeToday { get; set; }
    public int ScrapToday { get; set; }
    public decimal ScrapPercentage { get; set; }
}
