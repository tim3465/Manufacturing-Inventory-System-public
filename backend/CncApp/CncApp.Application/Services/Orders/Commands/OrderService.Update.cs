using AutoMapper;
using CncApp.Application.Dtos.Orders;

namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    public async Task<OrderDto?> UpdateAsync(int id, UpdateOrderRequestDto dto, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        if (order == null)
            return null;

        // Update metadata only - PartId, CustomerId, PartAmountRequested, PartsPerBar
        order.PartId = dto.PartId;
        order.CustomerId = dto.CustomerId;
        order.PartAmountRequested = dto.PartAmountRequested;
        order.PartsPerBar = dto.PartsPerBar;

        await _orderRepository.SaveChangesAsync(ct);

        return _mapper.Map<OrderDto>(order);
    }
}

