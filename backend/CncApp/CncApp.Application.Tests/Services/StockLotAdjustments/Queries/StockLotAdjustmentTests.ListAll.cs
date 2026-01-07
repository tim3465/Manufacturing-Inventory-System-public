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
    public async Task ListAllAsync_WhenAdjustmentsExist_ReturnsListOfDtos()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var adjustments = new List<StockLotAdjustment>
        {
            new StockLotAdjustment(1, 10, StockLotAdjustmentReasonEnum.Received) { Id = 1 },
            new StockLotAdjustment(2, -5, StockLotAdjustmentReasonEnum.Consumed) { Id = 2 },
            new StockLotAdjustment(1, 3, StockLotAdjustmentReasonEnum.Adjusted) { Id = 3 }
        };

        var expectedDtos = new List<StockLotAdjustmentDto>
        {
            new StockLotAdjustmentDto { Id = 1, StockLotId = 1, DeltaBars = 10, Reason = StockLotAdjustmentReasonEnum.Received },
            new StockLotAdjustmentDto { Id = 2, StockLotId = 2, DeltaBars = -5, Reason = StockLotAdjustmentReasonEnum.Consumed },
            new StockLotAdjustmentDto { Id = 3, StockLotId = 1, DeltaBars = 3, Reason = StockLotAdjustmentReasonEnum.Adjusted }
        };

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(adjustments);

        MockMapper
            .Setup(m => m.Map<List<StockLotAdjustmentDto>>(adjustments))
            .Returns(expectedDtos);

        // Act
        var result = await StockLotAdjustmentService.ListAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
        Assert.Equal(3, result[2].Id);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<StockLotAdjustmentDto>>(adjustments), Times.Once);
    }

    [Fact]
    public async Task ListAllAsync_WhenNoAdjustmentsExist_ReturnsEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(new List<StockLotAdjustment>());

        MockMapper
            .Setup(m => m.Map<List<StockLotAdjustmentDto>>(It.IsAny<List<StockLotAdjustment>>()))
            .Returns(new List<StockLotAdjustmentDto>());

        // Act
        var result = await StockLotAdjustmentService.ListAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<StockLotAdjustmentDto>>(It.IsAny<List<StockLotAdjustment>>()), Times.Once);
    }
}

