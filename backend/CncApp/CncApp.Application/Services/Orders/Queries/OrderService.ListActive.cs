using CncApp.Application.Dtos.Orders;

namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    public async Task<List<OrderDto>> ListActiveAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

