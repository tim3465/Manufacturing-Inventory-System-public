namespace CncApp.Application.Dtos.SupervisorDashboard;

public class SupervisorDashboardDto
{
    public int MachinesRunning { get; set; }
    public int OperatorsActive { get; set; }
    public int LateJobs { get; set; }
    public List<SupervisorDashboardOperatorDto> Operators { get; set; } = new();
}
