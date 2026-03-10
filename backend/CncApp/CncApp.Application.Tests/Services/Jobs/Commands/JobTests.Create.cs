using AutoMapper;
using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    [Fact]
    public async Task CreateAsync_WhenValidDto_CreatesJobAndReturnsId()
    {
        // Arrange
        var dto = new CreateJobRequestDto
        {
            OrderId = 1,
            MachineId = 2,
            StockLotId = null,
            PartAmountPlanned = 10,
            BarAmountPlanned = 5,
            BarCycleTime = TimeSpan.FromMinutes(1),
            BarsInJob = 2,
            EstimatedPartsPerBar = 5,
            DueDate = new DateOnly(2026, 6, 1)
        };
        var cancellationToken = CancellationToken.None;

        var job = new Job(
            orderId: dto.OrderId,
            stockLotId: dto.StockLotId,
            machineId: dto.MachineId,
            partAmountPlanned: dto.PartAmountPlanned,
            barAmountPlanned: dto.BarAmountPlanned,
            barCycleTime: dto.BarCycleTime,
            barsInJob: dto.BarsInJob,
            estimatedPartsPerBar: dto.EstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1))
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<Job>(dto))
            .Returns(job);

        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<Job>(), cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await JobService.CreateAsync(dto, cancellationToken);

        // Assert
        Assert.Equal(1, result);

        MockMapper.Verify(m => m.Map<Job>(dto), Times.Once);
        MockRepository.Verify(r => r.AddAsync(It.IsAny<Job>(), cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}

