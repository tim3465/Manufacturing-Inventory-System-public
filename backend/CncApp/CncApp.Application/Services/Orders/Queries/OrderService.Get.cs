using AutoMapper;
using CncApp.Application.Dtos.Orders;

namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    public async Task<OrderDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        if (order == null)
            return null;

        return _mapper.Map<OrderDto>(order);
    }
}

