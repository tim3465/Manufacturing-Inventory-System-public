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
    public async Task CreateAsync_WhenValidDto_CreatesStockLotAdjustmentAndReturnsId()
    {
        // Arrange
        var dto = new CreateStockLotAdjustmentRequestDto
        {
            StockLotId = 1,
            DeltaBars = 10,
            Reason = StockLotAdjustmentReasonEnum.Received,
            JobId = 5,
            Notes = "Test notes"
        };
        var cancellationToken = CancellationToken.None;

        var stockLotAdjustment = new StockLotAdjustment(dto.StockLotId, dto.DeltaBars, dto.Reason, dto.JobId, dto.Notes)
        {
            Id = 42
        };

        MockMapper
            .Setup(m => m.Map<StockLotAdjustment>(dto))
            .Returns(stockLotAdjustment);

        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLotAdjustment>(), cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await StockLotAdjustmentService.CreateAsync(dto, cancellationToken);

        // Assert
        Assert.Equal(42, result);

        MockMapper.Verify(m => m.Map<StockLotAdjustment>(dto), Times.Once);
        MockRepository.Verify(r => r.AddAsync(It.IsAny<StockLotAdjustment>(), cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}

