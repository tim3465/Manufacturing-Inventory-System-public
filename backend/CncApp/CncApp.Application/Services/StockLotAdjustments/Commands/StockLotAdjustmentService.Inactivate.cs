namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _stockLotAdjustmentRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (result)
        {
            await _stockLotAdjustmentRepository.SaveChangesAsync(ct);
        }
        return result;
    }
}

