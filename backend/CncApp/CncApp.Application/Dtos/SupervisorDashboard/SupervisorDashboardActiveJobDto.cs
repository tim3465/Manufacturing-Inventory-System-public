namespace CncApp.Application.Dtos.SupervisorDashboard;

public class SupervisorDashboardActiveJobDto
{
    public int JobId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
}
