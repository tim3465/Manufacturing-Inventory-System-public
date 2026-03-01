using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    /// <summary>
    /// Creates a stock lot adjustment and applies its DeltaBars to the parent
    /// StockLot.AmountOfBars, wrapped in its own transaction.
    /// Use this entrypoint from CRUD controllers.
    /// Workflows that already own a transaction should call
    /// <see cref="CreateWithinTransactionAsync"/> instead.
    /// </summary>
    public async Task<int> CreateAsync(CreateStockLotAdjustmentRequestDto dto, CancellationToken ct = default)
    {
        await _transactionManager.BeginTransactionAsync(ct);

        try
        {
            var id = await CreateWithinTransactionAsync(dto, ct);
            await _transactionManager.CommitTransactionAsync(ct);
            return id;
        }
        catch
        {
            await _transactionManager.RollbackTransactionAsync(ct);
            throw;
        }
    }

    /// <summary>
    /// Core logic: creates a stock lot adjustment and applies its DeltaBars to
    /// the parent StockLot.AmountOfBars.
    /// <para>
    /// <b>Caller MUST already have an active transaction.</b>
    /// Do not call this from controllers; use <see cref="CreateAsync"/> instead.
    /// </para>
    /// </summary>
    // Not tested directly — exercised through CreateAsync (controller path)
    // and ShippingReceivingService.ReceiveShipmentAsync (workflow path).
    internal async Task<int> CreateWithinTransactionAsync(CreateStockLotAdjustmentRequestDto dto, CancellationToken ct = default)
    {
        var stockLotAdjustment = _mapper.Map<StockLotAdjustment>(dto);

        await _stockLotAdjustmentRepository.AddAsync(stockLotAdjustment, ct);
        await _stockLotAdjustmentRepository.SaveChangesAsync(ct);

        // StockLot.AmountOfBars is a cached total derived from adjustments.
        // Every adjustment must keep it in sync.
        var stockLot = await _stockLotRepository.GetByIdAsync(dto.StockLotId, ct);
        stockLot!.AmountOfBars += dto.DeltaBars;
        await _stockLotRepository.SaveChangesAsync(ct);

        return stockLotAdjustment.Id;
    }
}
