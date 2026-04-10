namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    /// <summary>
    /// Inactivates a stock lot adjustment and reverses its DeltaBars effect
    /// on StockLot.AmountOfBars, wrapped in its own transaction.
    /// Use this entrypoint from CRUD controllers.
    /// Workflows that already own a transaction should call
    /// <see cref="InactivateWithinTransactionAsync"/> instead.
    /// </summary>
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var adjustment = await _stockLotAdjustmentRepository.GetByIdAsync(id, ct);
        if (adjustment == null || adjustment.InactivatedDateTime.HasValue)
            return false;

        await _transactionManager.BeginTransactionAsync(ct);

        try
        {
            var result = await InactivateWithinTransactionAsync(id, adjustment.StockLotId, adjustment.DeltaBars, inactivatedByUserId, ct);
            if (!result)
            {
                await _transactionManager.RollbackTransactionAsync(ct);
                return false;
            }

            await _transactionManager.CommitTransactionAsync(ct);
            return true;
        }
        catch
        {
            await _transactionManager.RollbackTransactionAsync(ct);
            throw;
        }
    }

    /// <summary>
    /// Core logic: inactivates a stock lot adjustment and reverses its DeltaBars
    /// effect on StockLot.AmountOfBars.
    /// <para>
    /// <b>Caller MUST already have an active transaction.</b>
    /// Do not call this from controllers; use <see cref="InactivateAsync"/> instead.
    /// </para>
    /// </summary>
    // Not tested directly — exercised through InactivateAsync (controller path).
    internal async Task<bool> InactivateWithinTransactionAsync(
        int id, int stockLotId, int deltaBars, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _stockLotAdjustmentRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (!result)
            return false;

        await _stockLotAdjustmentRepository.SaveChangesAsync(ct);

        // Reverse the adjustment's effect on the cached bar count.
        var stockLot = await _stockLotRepository.GetByIdAsync(stockLotId, ct);
        stockLot!.AmountOfBars -= deltaBars;
        await _stockLotRepository.SaveChangesAsync(ct);

        return true;
    }
}
