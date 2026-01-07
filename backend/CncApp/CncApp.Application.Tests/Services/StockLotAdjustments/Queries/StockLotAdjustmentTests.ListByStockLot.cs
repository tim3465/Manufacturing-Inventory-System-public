using AutoMapper;
using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.StockLotAdjustments;

public partial class StockLotAdjustmentTests
{
    [Fact]
    public async Task ListByStockLotAsync_WhenAdjustmentsExist_ReturnsListOfDtos()
    {
        // Arrange
        var stockLotId = 1;
        var cancellationToken = CancellationToken.None;

        var adjustments = new List<StockLotAdjustment>
        {
            new StockLotAdjustment(stockLotId, 10, StockLotAdjustmentReasonEnum.Received) { Id = 1 },
            new StockLotAdjustment(stockLotId, -5, StockLotAdjustmentReasonEnum.Consumed) { Id = 2 }
        };

        var expectedDtos = new List<StockLotAdjustmentDto>
        {
            new StockLotAdjustmentDto { Id = 1, StockLotId = stockLotId, DeltaBars = 10, Reason = StockLotAdjustmentReasonEnum.Received },
            new StockLotAdjustmentDto { Id = 2, StockLotId = stockLotId, DeltaBars = -5, Reason = StockLotAdjustmentReasonEnum.Consumed }
        };

        MockRepository
            .Setup(r => r.ListByStockLotAsync(stockLotId, cancellationToken))
            .ReturnsAsync(adjustments);

        MockMapper
            .Setup(m => m.Map<List<StockLotAdjustmentDto>>(adjustments))
            .Returns(expectedDtos);

        // Act
        var result = await StockLotAdjustmentService.ListByStockLotAsync(stockLotId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);

        MockRepository.Verify(r => r.ListByStockLotAsync(stockLotId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<StockLotAdjustmentDto>>(adjustments), Times.Once);
    }

    [Fact]
    public async Task ListByStockLotAsync_WhenNoAdjustmentsExist_ReturnsEmptyList()
    {
        // Arrange
        var stockLotId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.ListByStockLotAsync(stockLotId, cancellationToken))
            .ReturnsAsync(new List<StockLotAdjustment>());

        MockMapper
            .Setup(m => m.Map<List<StockLotAdjustmentDto>>(It.IsAny<List<StockLotAdjustment>>()))
            .Returns(new List<StockLotAdjustmentDto>());

        // Act
        var result = await StockLotAdjustmentService.ListByStockLotAsync(stockLotId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListByStockLotAsync(stockLotId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<StockLotAdjustmentDto>>(It.IsAny<List<StockLotAdjustment>>()), Times.Once);
    }
}

