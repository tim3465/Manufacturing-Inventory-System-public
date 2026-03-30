using CncApp.Application.Dtos.Jobs;
using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    [Fact]
    public async Task ListByOrderAsync_WhenJobsExist_ReturnsMappedDtos()
    {
        // Arrange
        var orderId = 30;
        var ct = CancellationToken.None;

        var machine = new Machine("SN-001", "Model-A") { Id = 10 };
        var part = new Part("Widget", "P-001", TimeSpan.FromMinutes(1), 5) { Id = 20 };
        var order = new Order(part.Id, 1, 100) { Id = orderId, Part = part };
        var stockLot = new StockLot("LOT-001", 1, 10, 1.5m, 12m, CncApp.Domain.Enums.StockLotConditionEnum.AsReceived, DateTime.UtcNow) { Id = 40 };

        var shift1 = new Shift(1, 1, 1, DateTime.UtcNow, partsMade: 5) { Id = 100 };
        var shift2 = new Shift(1, 1, 1, DateTime.UtcNow, partsMade: 3) { Id = 101 };

        var job = new Job(order.Id, stockLot.Id, machine.Id, 100, 10, TimeSpan.FromMinutes(1), 5, new DateOnly(2026, 6, 1))
        {
            Id = 1,
            Machine = machine,
            Order = order,
            StockLot = stockLot,
            Shifts = new List<Shift> { shift1, shift2 }
        };

        var jobs = new List<Job> { job };
        var mappedShifts = new List<ShiftDto> { new ShiftDto { Id = 100 }, new ShiftDto { Id = 101 } };

        MockRepository
            .Setup(r => r.ListByOrderWithShiftsAsync(orderId, ct))
            .ReturnsAsync(jobs);

        MockMapper
            .Setup(m => m.Map<List<ShiftDto>>(job.Shifts))
            .Returns(mappedShifts);

        // Act
        var result = await JobService.ListByOrderAsync(orderId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        var dto = result[0];
        Assert.Equal(1, dto.Id);
        Assert.Equal(orderId, dto.OrderId);
        Assert.Equal(10, dto.MachineId);
        Assert.Equal("SN-001", dto.MachineName);
        Assert.Equal(100, dto.PartAmountPlanned);
        Assert.Equal("Widget", dto.PartName);
        Assert.Equal("P-001", dto.PartNumber);
        Assert.Equal(8, dto.PartsCompleted);
        Assert.Equal(8.0m, dto.PercentComplete);
        Assert.Equal(40, dto.StockLotId);
        Assert.Equal("LOT-001", dto.LotNumber);
        Assert.Equal(2, dto.Shifts.Count);

        MockRepository.Verify(r => r.ListByOrderWithShiftsAsync(orderId, ct), Times.Once);
        MockMapper.Verify(m => m.Map<List<ShiftDto>>(job.Shifts), Times.Once);
    }

    [Fact]
    public async Task ListByOrderAsync_WhenNoJobs_ReturnsEmptyList()
    {
        // Arrange
        var orderId = 99;
        var ct = CancellationToken.None;

        MockRepository
            .Setup(r => r.ListByOrderWithShiftsAsync(orderId, ct))
            .ReturnsAsync(new List<Job>());

        // Act
        var result = await JobService.ListByOrderAsync(orderId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListByOrderWithShiftsAsync(orderId, ct), Times.Once);
        MockMapper.Verify(m => m.Map<List<ShiftDto>>(It.IsAny<object>()), Times.Never);
    }
}
