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
    public async Task ListActiveAsync_WhenStockLotsExist_ReturnsListOfStockLotDtos()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var stockLots = new List<StockLot>
        {
            new StockLot("LOT-001", 1, 10, 25.5m, 1000.0m, StockLotConditionEnum.AsReceived, new DateTime(2025, 1, 1, 10, 0, 0))
            {
                Id = 1
            },
            new StockLot("LOT-002", 2, 20, 30.0m, 2000.0m, StockLotConditionEnum.Ground, new DateTime(2025, 1, 2, 10, 0, 0))
            {
                Id = 2
            }
        };

        var expectedDtos = new List<StockLotDto>
        {
            new StockLotDto
            {
                Id = 1,
                LotNumber = "LOT-001",
                MaterialId = 1,
                AmountOfBars = 10,
                Diameter = 25.5m,
                BarLength = 1000.0m,
                Condition = StockLotConditionEnum.AsReceived,
                CheckedInDateTime = new DateTime(2025, 1, 1, 10, 0, 0)
            },
            new StockLotDto
            {
                Id = 2,
                LotNumber = "LOT-002",
                MaterialId = 2,
                AmountOfBars = 20,
                Diameter = 30.0m,
                BarLength = 2000.0m,
                Condition = StockLotConditionEnum.Ground,
                CheckedInDateTime = new DateTime(2025, 1, 2, 10, 0, 0)
            }
        };

        MockRepository
            .Setup(r => r.ListActiveAsync(cancellationToken))
            .ReturnsAsync(stockLots);

        MockMapper
            .Setup(m => m.Map<List<StockLotDto>>(stockLots))
            .Returns(expectedDtos);

        // Act
        var result = await StockLotService.ListActiveAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("LOT-001", result[0].LotNumber);
        Assert.Equal("LOT-002", result[1].LotNumber);

        MockRepository.Verify(r => r.ListActiveAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<StockLotDto>>(stockLots), Times.Once);
    }

    [Fact]
    public async Task ListActiveAsync_WhenNoStockLotsExist_ReturnsEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var stockLots = new List<StockLot>();
        var expectedDtos = new List<StockLotDto>();

        MockRepository
            .Setup(r => r.ListActiveAsync(cancellationToken))
            .ReturnsAsync(stockLots);

        MockMapper
            .Setup(m => m.Map<List<StockLotDto>>(stockLots))
            .Returns(expectedDtos);

        // Act
        var result = await StockLotService.ListActiveAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListActiveAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<StockLotDto>>(stockLots), Times.Once);
    }
}

