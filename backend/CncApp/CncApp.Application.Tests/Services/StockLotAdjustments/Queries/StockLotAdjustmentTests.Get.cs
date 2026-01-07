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
    public async Task GetAsync_WhenAdjustmentExists_ReturnsStockLotAdjustmentDto()
    {
        // Arrange
        var adjustmentId = 1;
        var cancellationToken = CancellationToken.None;

        var stockLotAdjustment = new StockLotAdjustment(1, 10, StockLotAdjustmentReasonEnum.Received, 5, "Test notes")
        {
            Id = adjustmentId
        };

        var expectedDto = new StockLotAdjustmentDto
        {
            Id = adjustmentId,
            StockLotId = 1,
            DeltaBars = 10,
            Reason = StockLotAdjustmentReasonEnum.Received,
            JobId = 5,
            Notes = "Test notes"
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(adjustmentId, cancellationToken))
            .ReturnsAsync(stockLotAdjustment);

        MockMapper
            .Setup(m => m.Map<StockLotAdjustmentDto>(stockLotAdjustment))
            .Returns(expectedDto);

        // Act
        var result = await StockLotAdjustmentService.GetAsync(adjustmentId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(adjustmentId, result.Id);
        Assert.Equal(1, result.StockLotId);
        Assert.Equal(10, result.DeltaBars);
        Assert.Equal(StockLotAdjustmentReasonEnum.Received, result.Reason);

        MockRepository.Verify(r => r.GetByIdAsync(adjustmentId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<StockLotAdjustmentDto>(stockLotAdjustment), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenAdjustmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var adjustmentId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(adjustmentId, cancellationToken))
            .ReturnsAsync((StockLotAdjustment?)null);

        // Act
        var result = await StockLotAdjustmentService.GetAsync(adjustmentId, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(adjustmentId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<StockLotAdjustmentDto>(It.IsAny<StockLotAdjustment>()), Times.Never);
    }
}

