using CncApp.Application.Dtos.Orders;

namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    public async Task<List<OrderProductionDto>> ListProductionAsync(CancellationToken ct = default)
    {
        var orders = await _orderRepository.ListActiveWithDetailsAsync(ct);

        return orders.Select(o =>
        {
            var partAmountCompleted = o.Jobs
                .SelectMany(j => j.Shifts)
                .Sum(s => s.PartsMade);

            var percentComplete = o.PartAmountRequested > 0
                ? (double)partAmountCompleted / o.PartAmountRequested * 100.0
                : 0.0;

            return new OrderProductionDto
            {
                Id = o.Id,
                CustomerName = o.Customer?.CompanyName ?? string.Empty,
                PartName = o.Part?.PartName ?? string.Empty,
                PartNumber = o.Part?.PartNumber ?? string.Empty,
                PartAmountRequested = o.PartAmountRequested,
                PartAmountCompleted = partAmountCompleted,
                PercentComplete = percentComplete
            };
        }).ToList();
    }
}
