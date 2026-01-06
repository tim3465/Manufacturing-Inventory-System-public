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
    public async Task CreateAsync_WhenValidDto_CreatesStockLotAndReturnsId()
    {
        // Arrange
        var dto = new CreateStockLotRequestDto
        {
            LotNumber = "LOT-001",
            MaterialId = 1,
            AmountOfBars = 10,
            Diameter = 25.5m,
            BarLength = 1000.0m,
            Condition = StockLotConditionEnum.AsReceived,
            CheckedInDateTime = new DateTime(2025, 1, 1, 10, 0, 0)
        };

        var cancellationToken = CancellationToken.None;

        var stockLot = new StockLot(
            dto.LotNumber,
            dto.MaterialId,
            dto.AmountOfBars,
            dto.Diameter,
            dto.BarLength,
            dto.Condition,
            dto.CheckedInDateTime)
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<StockLot>(dto))
            .Returns(stockLot);

        MockRepository
            .Setup(r => r.AddAsync(stockLot, cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await StockLotService.CreateAsync(dto, cancellationToken);

        // Assert
        Assert.Equal(1, result);

        MockMapper.Verify(m => m.Map<StockLot>(dto), Times.Once);
        MockRepository.Verify(r => r.AddAsync(stockLot, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}

