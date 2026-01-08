using CncApp.Application.Dtos.Orders;

namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    public async Task<OrderDto?> UpdateAsync(int id, UpdateOrderRequestDto dto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

