using AutoMapper;
using CncApp.Application.Dtos.StockLots;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.StockLots;

public partial class StockLotTests
{
    [Fact]
    public async Task UpdateAsync_WhenStockLotExists_UpdatesAndReturnsTrue()
    {
        // Arrange
        var stockLotId = 1;
        var dto = new UpdateStockLotRequestDto
        {
            LotNumber = "LOT-002",
            MaterialId = 2,
            Diameter = 30.0m,
            BarLength = 2000.0m,
            Condition = StockLotConditionEnum.Ground,
            CheckedInDateTime = new DateTime(2025, 1, 2, 10, 0, 0)
        };

        var cancellationToken = CancellationToken.None;

        var stockLot = new StockLot(
            "LOT-001",
            1,
            10,
            25.5m,
            1000.0m,
            StockLotConditionEnum.AsReceived,
            new DateTime(2025, 1, 1, 10, 0, 0))
        {
            Id = stockLotId
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(stockLotId, cancellationToken))
            .ReturnsAsync(stockLot);

        MockMapper
            .Setup(m => m.Map(dto, stockLot))
            .Returns(stockLot);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await StockLotService.UpdateAsync(stockLotId, dto, cancellationToken);

        // Assert
        Assert.True(result);

        MockRepository.Verify(r => r.GetByIdAsync(stockLotId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map(dto, stockLot), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenStockLotDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var stockLotId = 999;
        var dto = new UpdateStockLotRequestDto
        {
            LotNumber = "LOT-002",
            MaterialId = 2,
            Diameter = 30.0m,
            BarLength = 2000.0m,
            Condition = StockLotConditionEnum.Ground,
            CheckedInDateTime = new DateTime(2025, 1, 2, 10, 0, 0)
        };

        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(stockLotId, cancellationToken))
            .ReturnsAsync((StockLot?)null);

        // Act
        var result = await StockLotService.UpdateAsync(stockLotId, dto, cancellationToken);

        // Assert
        Assert.False(result);

        MockRepository.Verify(r => r.GetByIdAsync(stockLotId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map(It.IsAny<UpdateStockLotRequestDto>(), It.IsAny<StockLot>()), Times.Never);
        MockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

