using AutoMapper;
using CncApp.Application.Dtos.Orders;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    public async Task<int> CreateAsync(CreateOrderRequestDto dto, CancellationToken ct = default)
    {
        var order = _mapper.Map<Order>(dto);

        await _orderRepository.AddAsync(order, ct);
        await _orderRepository.SaveChangesAsync(ct);

        return order.Id;
    }
}

