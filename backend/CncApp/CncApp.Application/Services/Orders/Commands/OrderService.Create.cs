using CncApp.Application.Dtos.Orders;

namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    public async Task<int> CreateAsync(CreateOrderRequestDto dto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

