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
    public async Task UpdateNotesAsync_WhenAdjustmentExists_UpdatesNotesAndReturnsDto()
    {
        // Arrange
        var adjustmentId = 1;
        var dto = new UpdateStockLotAdjustmentNotesRequestDto
        {
            Notes = "Updated notes"
        };
        var cancellationToken = CancellationToken.None;

        var stockLotAdjustment = new StockLotAdjustment(1, 10, StockLotAdjustmentReasonEnum.Received)
        {
            Id = adjustmentId,
            Notes = "Original notes"
        };

        var expectedDto = new StockLotAdjustmentDto
        {
            Id = adjustmentId,
            StockLotId = 1,
            DeltaBars = 10,
            Reason = StockLotAdjustmentReasonEnum.Received,
            Notes = "Updated notes"
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(adjustmentId, cancellationToken))
            .ReturnsAsync(stockLotAdjustment);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        MockMapper
            .Setup(m => m.Map<StockLotAdjustmentDto>(stockLotAdjustment))
            .Returns(expectedDto);

        // Act
        var result = await StockLotAdjustmentService.UpdateNotesAsync(adjustmentId, dto, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated notes", result.Notes);
        Assert.Equal("Updated notes", stockLotAdjustment.Notes);

        MockRepository.Verify(r => r.GetByIdAsync(adjustmentId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<StockLotAdjustmentDto>(stockLotAdjustment), Times.Once);
    }

    [Fact]
    public async Task UpdateNotesAsync_WhenAdjustmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var adjustmentId = 999;
        var dto = new UpdateStockLotAdjustmentNotesRequestDto
        {
            Notes = "Updated notes"
        };
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(adjustmentId, cancellationToken))
            .ReturnsAsync((StockLotAdjustment?)null);

        // Act
        var result = await StockLotAdjustmentService.UpdateNotesAsync(adjustmentId, dto, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(adjustmentId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
        MockMapper.Verify(m => m.Map<StockLotAdjustmentDto>(It.IsAny<StockLotAdjustment>()), Times.Never);
    }
}

