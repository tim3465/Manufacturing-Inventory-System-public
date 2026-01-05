namespace CncApp.Application.Services.StockLots;

public partial class StockLotService
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _stockLotRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (result)
        {
            await _stockLotRepository.SaveChangesAsync(ct);
        }
        return result;
    }
}

