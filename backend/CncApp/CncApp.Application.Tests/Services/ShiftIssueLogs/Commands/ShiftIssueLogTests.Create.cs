using CncApp.Application.Dtos.ShiftIssueLogs;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.ShiftIssueLogs;

public partial class ShiftIssueLogTests
{
    // ── public CreateAsync (controller path) ─────────────────────────────

    [Fact]
    public async Task CreateAsync_WhenValid_BeginsAndCommitsTransaction()
    {
        // Arrange
        var dto = new CreateShiftIssueLogRequestDto
        {
            ShiftId = 1,
            IssueType = IssueTypeEnum.Production,
            ScrapQuantity = 3,
            Description = "Defective parts",
            Downtime = TimeSpan.FromMinutes(10)
        };

        var shiftIssueLog = new ShiftIssueLog(dto.ShiftId, dto.IssueType, dto.ScrapQuantity, dto.Description, dto.Downtime)
        {
            Id = 42
        };

        var shift = new Shift(1, 1, 0, DateTime.UtcNow, scrap: 5, downtime: TimeSpan.FromMinutes(20))
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<ShiftIssueLog>(dto))
            .Returns(shiftIssueLog);
        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<ShiftIssueLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockShiftRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shift);
        MockShiftRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await ShiftIssueLogService.CreateAsync(dto);

        // Assert
        Assert.Equal(42, result);
        Assert.Equal(8, shift.Scrap); // 5 + 3
        Assert.Equal(TimeSpan.FromMinutes(30), shift.Downtime); // 20 + 10

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenFailure_RollsBackTransaction()
    {
        // Arrange
        var dto = new CreateShiftIssueLogRequestDto
        {
            ShiftId = 1,
            IssueType = IssueTypeEnum.Setup,
            ScrapQuantity = 2,
            Description = "Test failure"
        };

        MockMapper
            .Setup(m => m.Map<ShiftIssueLog>(It.IsAny<CreateShiftIssueLogRequestDto>()))
            .Throws(new InvalidOperationException("Simulated failure"));
        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ShiftIssueLogService.CreateAsync(dto));

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenScrapProvided_AddsToShiftScrap()
    {
        // Arrange
        var dto = new CreateShiftIssueLogRequestDto
        {
            ShiftId = 1,
            IssueType = IssueTypeEnum.Production,
            ScrapQuantity = 7,
            Description = "Scrap event",
            Downtime = null
        };

        var shiftIssueLog = new ShiftIssueLog(dto.ShiftId, dto.IssueType, dto.ScrapQuantity, dto.Description)
        {
            Id = 10
        };

        var shift = new Shift(1, 1, 0, DateTime.UtcNow, scrap: 3)
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<ShiftIssueLog>(dto))
            .Returns(shiftIssueLog);
        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<ShiftIssueLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockShiftRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shift);
        MockShiftRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await ShiftIssueLogService.CreateAsync(dto);

        // Assert
        Assert.Equal(10, result);
        Assert.Equal(10, shift.Scrap); // 3 + 7
        Assert.Null(shift.Downtime); // unchanged
    }

    [Fact]
    public async Task CreateAsync_WhenDowntimeProvided_AddsToExistingShiftDowntime()
    {
        // Arrange
        var dto = new CreateShiftIssueLogRequestDto
        {
            ShiftId = 1,
            IssueType = IssueTypeEnum.Setup,
            ScrapQuantity = 0,
            Description = "Downtime event",
            Downtime = TimeSpan.FromMinutes(15)
        };

        var shiftIssueLog = new ShiftIssueLog(dto.ShiftId, dto.IssueType, dto.ScrapQuantity, dto.Description, dto.Downtime)
        {
            Id = 20
        };

        var shift = new Shift(1, 1, 0, DateTime.UtcNow, downtime: TimeSpan.FromMinutes(45))
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<ShiftIssueLog>(dto))
            .Returns(shiftIssueLog);
        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<ShiftIssueLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockShiftRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shift);
        MockShiftRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await ShiftIssueLogService.CreateAsync(dto);

        // Assert
        Assert.Equal(20, result);
        Assert.Equal(TimeSpan.FromMinutes(60), shift.Downtime); // 45 + 15
    }

    [Fact]
    public async Task CreateAsync_WhenDowntimeProvided_AndShiftDowntimeIsNull_SetsDowntime()
    {
        // Arrange
        var dto = new CreateShiftIssueLogRequestDto
        {
            ShiftId = 1,
            IssueType = IssueTypeEnum.Setup,
            ScrapQuantity = 0,
            Description = "First downtime",
            Downtime = TimeSpan.FromMinutes(10)
        };

        var shiftIssueLog = new ShiftIssueLog(dto.ShiftId, dto.IssueType, dto.ScrapQuantity, dto.Description, dto.Downtime)
        {
            Id = 30
        };

        var shift = new Shift(1, 1, 0, DateTime.UtcNow) // downtime is null
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<ShiftIssueLog>(dto))
            .Returns(shiftIssueLog);
        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<ShiftIssueLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockShiftRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shift);
        MockShiftRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await ShiftIssueLogService.CreateAsync(dto);

        // Assert
        Assert.Equal(30, result);
        Assert.Equal(TimeSpan.FromMinutes(10), shift.Downtime); // null + 10 = 10
    }
}
