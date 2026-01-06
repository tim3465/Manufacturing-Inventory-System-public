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
    public async Task GetAsync_WhenStockLotExists_ReturnsStockLotDto()
    {
        // Arrange
        var stockLotId = 1;
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

        var expectedDto = new StockLotDto
        {
            Id = stockLotId,
            LotNumber = "LOT-001",
            MaterialId = 1,
            AmountOfBars = 10,
            Diameter = 25.5m,
            BarLength = 1000.0m,
            Condition = StockLotConditionEnum.AsReceived,
            CheckedInDateTime = new DateTime(2025, 1, 1, 10, 0, 0)
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(stockLotId, cancellationToken))
            .ReturnsAsync(stockLot);

        MockMapper
            .Setup(m => m.Map<StockLotDto>(stockLot))
            .Returns(expectedDto);

        // Act
        var result = await StockLotService.GetAsync(stockLotId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(stockLotId, result.Id);
        Assert.Equal("LOT-001", result.LotNumber);
        Assert.Equal(1, result.MaterialId);
        Assert.Equal(10, result.AmountOfBars);

        MockRepository.Verify(r => r.GetByIdAsync(stockLotId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<StockLotDto>(stockLot), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenStockLotDoesNotExist_ReturnsNull()
    {
        // Arrange
        var stockLotId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(stockLotId, cancellationToken))
            .ReturnsAsync((StockLot?)null);

        // Act
        var result = await StockLotService.GetAsync(stockLotId, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(stockLotId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<StockLotDto>(It.IsAny<StockLot>()), Times.Never);
    }
}

