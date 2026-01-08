using CncApp.Application.Dtos.Orders;

namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    public async Task<OrderDto?> GetAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

