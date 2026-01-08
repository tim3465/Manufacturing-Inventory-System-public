using AutoMapper;
using CncApp.Application.Dtos.Orders;

namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    public async Task<List<OrderDto>> ListActiveAsync(CancellationToken ct = default)
    {
        var orders = await _orderRepository.ListActiveAsync(ct);
        return _mapper.Map<List<OrderDto>>(orders);
    }
}

