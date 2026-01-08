namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _orderRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (result)
        {
            await _orderRepository.SaveChangesAsync(ct);
        }
        return result;
    }
}

