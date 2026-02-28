using CncApp.Application.Dtos.Materials;
using CncApp.Application.Dtos.ShippingReceiving;
using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Application.Dtos.StockLots;
using CncApp.Domain.Enums;

namespace CncApp.Application.Services.Workflows.ShippingReceiving;

public partial class ShippingReceivingService
{
    public async Task<ReceiveShipmentResponseDto> ReceiveShipmentAsync(
        ReceiveShipmentRequestDto dto,
        CancellationToken ct = default)
    {
        await _transactionManager.BeginTransactionAsync(ct);

        try
        {
            int materialId;

            if (dto.MaterialId.HasValue)
            {
                materialId = dto.MaterialId.Value;
            }
            else
            {
                materialId = await _materialService.CreateAsync(
                    new CreateMaterialRequestDto
                    {
                        HeatNumber = dto.HeatNumber!,
                        MaterialName = dto.MaterialName!
                    }, ct);
            }

            var stockLotId = await _stockLotService.CreateAsync(
                new CreateStockLotRequestDto
                {
                    LotNumber = dto.LotNumber,
                    MaterialId = materialId,
                    AmountOfBars = 0,
                    Diameter = dto.Diameter,
                    BarLength = dto.BarLength,
                    Condition = dto.Condition,
                    CheckedInDateTime = dto.CheckedInDateTime
                }, ct);

            var adjustmentId = await _stockLotAdjustmentService.CreateAsync(
                new CreateStockLotAdjustmentRequestDto
                {
                    StockLotId = stockLotId,
                    DeltaBars = dto.AmountOfBars,
                    Reason = StockLotAdjustmentReasonEnum.Received,
                    Notes = dto.Notes
                }, ct);

            var stockLot = await _stockLotRepository.GetByIdAsync(stockLotId, ct);
            stockLot!.AmountOfBars += dto.AmountOfBars;
            await _stockLotRepository.SaveChangesAsync(ct);

            await _transactionManager.CommitTransactionAsync(ct);

            return new ReceiveShipmentResponseDto
            {
                MaterialId = materialId,
                StockLotId = stockLotId,
                StockLotAdjustmentId = adjustmentId
            };
        }
        catch
        {
            await _transactionManager.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
