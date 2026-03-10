namespace CncApp.Application.Dtos.OrderPlanning;

public class CreateOrderWithJobsResponseDto
{
    public int OrderId { get; set; }

    public List<int> JobIds { get; set; } = new();
}
