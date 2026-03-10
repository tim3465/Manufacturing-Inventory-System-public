using CncApp.Domain.Common;
using CncApp.Domain.Entities;
using Xunit;

namespace CncApp.Domain.Tests.Entities;

/// <summary>
/// Domain tests for Job entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
public class JobTests
{
    private const int ValidOrderId = 1;
    private const int ValidStockLotId = 1;
    private const int ValidMachineId = 1;
    private const int ValidPartAmountPlanned = 10;
    private const int ValidBarAmountPlanned = 5;
    private static readonly TimeSpan ValidBarCycleTime = TimeSpan.FromMinutes(1);
    private const int ValidBarsInJob = 2;
    private const int ValidEstimatedPartsPerBar = 5;

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenOrderIdIsZero_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Job(
            orderId: 0,
            stockLotId: ValidStockLotId,
            machineId: ValidMachineId,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: ValidBarCycleTime,
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1)));

        Assert.Contains("OrderId", ex.Message);
    }

    [Fact]
    public void Constructor_WhenStockLotIdIsZero_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Job(
            orderId: ValidOrderId,
            stockLotId: 0,
            machineId: ValidMachineId,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: ValidBarCycleTime,
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1)));

        Assert.Contains("StockLotId", ex.Message);
    }

    [Fact]
    public void Constructor_WhenStockLotIdIsNull_CreatesJob()
    {
        var job = new Job(
            orderId: ValidOrderId,
            stockLotId: null,
            machineId: ValidMachineId,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: ValidBarCycleTime,
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1));

        Assert.NotNull(job);
        Assert.Null(job.StockLotId);
    }

    [Fact]
    public void Constructor_WhenMachineIdIsZero_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Job(
            orderId: ValidOrderId,
            stockLotId: ValidStockLotId,
            machineId: 0,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: ValidBarCycleTime,
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1)));

        Assert.Contains("MachineId", ex.Message);
    }

    [Fact]
    public void Constructor_WhenPartAmountPlannedIsNegative_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Job(
            orderId: ValidOrderId,
            stockLotId: ValidStockLotId,
            machineId: ValidMachineId,
            partAmountPlanned: -1,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: ValidBarCycleTime,
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1)));

        Assert.Contains("PartAmountPlanned", ex.Message);
    }

    [Fact]
    public void Constructor_WhenBarAmountPlannedIsNegative_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Job(
            orderId: ValidOrderId,
            stockLotId: ValidStockLotId,
            machineId: ValidMachineId,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: -1,
            barCycleTime: ValidBarCycleTime,
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1)));

        Assert.Contains("BarAmountPlanned", ex.Message);
    }

    [Fact]
    public void Constructor_WhenBarCycleTimeIsNegative_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Job(
            orderId: ValidOrderId,
            stockLotId: ValidStockLotId,
            machineId: ValidMachineId,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: TimeSpan.FromSeconds(-1),
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1)));

        Assert.Contains("BarCycleTime", ex.Message);
    }

    [Fact]
    public void Constructor_WhenBarsInJobIsNegative_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Job(
            orderId: ValidOrderId,
            stockLotId: ValidStockLotId,
            machineId: ValidMachineId,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: ValidBarCycleTime,
            barsInJob: -1,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1)));

        Assert.Contains("BarsInJob", ex.Message);
    }

    [Fact]
    public void Constructor_WhenEstimatedPartsPerBarIsNegative_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Job(
            orderId: ValidOrderId,
            stockLotId: ValidStockLotId,
            machineId: ValidMachineId,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: ValidBarCycleTime,
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: -1,
            dueDate: new DateOnly(2026, 6, 1)));

        Assert.Contains("EstimatedPartsPerBar", ex.Message);
    }

    [Fact]
    public void Constructor_WhenValidParameters_CreatesJob()
    {
        var job = new Job(
            orderId: ValidOrderId,
            stockLotId: ValidStockLotId,
            machineId: ValidMachineId,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: ValidBarCycleTime,
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1));

        Assert.NotNull(job);
        Assert.Equal(ValidOrderId, job.OrderId);
        Assert.Equal(ValidStockLotId, job.StockLotId);
        Assert.Equal(ValidMachineId, job.MachineId);
        Assert.Equal(ValidPartAmountPlanned, job.PartAmountPlanned);
        Assert.Equal(ValidBarAmountPlanned, job.BarAmountPlanned);
        Assert.Equal(ValidBarCycleTime, job.BarCycleTime);
        Assert.Equal(ValidBarsInJob, job.BarsInJob);
        Assert.Equal(ValidEstimatedPartsPerBar, job.EstimatedPartsPerBar);
        Assert.NotNull(job.Shifts);
        Assert.Empty(job.Shifts);
        Assert.Null(job.InactivatedDateTime);
    }

    #endregion

    #region Property Setter Tests

    [Fact]
    public void OrderIdSetter_WhenValueIsZero_ThrowsDomainException()
    {
        var job = CreateValidJob();
        var ex = Assert.Throws<DomainException>(() => job.OrderId = 0);
        Assert.Contains("OrderId", ex.Message);
    }

    [Fact]
    public void StockLotIdSetter_WhenValueIsZero_ThrowsDomainException()
    {
        var job = CreateValidJob();
        var ex = Assert.Throws<DomainException>(() => job.StockLotId = 0);
        Assert.Contains("StockLotId", ex.Message);
    }

    [Fact]
    public void StockLotIdSetter_WhenSetToNull_AllowsNull()
    {
        var job = CreateValidJob();
        job.StockLotId = null;
        Assert.Null(job.StockLotId);
    }

    [Fact]
    public void MachineIdSetter_WhenValueIsZero_ThrowsDomainException()
    {
        var job = CreateValidJob();
        var ex = Assert.Throws<DomainException>(() => job.MachineId = 0);
        Assert.Contains("MachineId", ex.Message);
    }

    [Fact]
    public void PartAmountPlannedSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        var job = CreateValidJob();
        var ex = Assert.Throws<DomainException>(() => job.PartAmountPlanned = -1);
        Assert.Contains("PartAmountPlanned", ex.Message);
    }

    [Fact]
    public void BarAmountPlannedSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        var job = CreateValidJob();
        var ex = Assert.Throws<DomainException>(() => job.BarAmountPlanned = -1);
        Assert.Contains("BarAmountPlanned", ex.Message);
    }

    [Fact]
    public void BarCycleTimeSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        var job = CreateValidJob();
        var ex = Assert.Throws<DomainException>(() => job.BarCycleTime = TimeSpan.FromSeconds(-1));
        Assert.Contains("BarCycleTime", ex.Message);
    }

    [Fact]
    public void BarsInJobSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        var job = CreateValidJob();
        var ex = Assert.Throws<DomainException>(() => job.BarsInJob = -1);
        Assert.Contains("BarsInJob", ex.Message);
    }

    [Fact]
    public void EstimatedPartsPerBarSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        var job = CreateValidJob();
        var ex = Assert.Throws<DomainException>(() => job.EstimatedPartsPerBar = -1);
        Assert.Contains("EstimatedPartsPerBar", ex.Message);
    }

    [Fact]
    public void EstimatedPartsPerBarSetter_WhenSetToNull_AllowsNull()
    {
        var job = CreateValidJob();
        job.EstimatedPartsPerBar = null;
        Assert.Null(job.EstimatedPartsPerBar);
    }

    #endregion

    #region DueDate Tests

    [Fact]
    public void Constructor_WhenDueDateIsDefault_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Job(
            orderId: ValidOrderId,
            stockLotId: ValidStockLotId,
            machineId: ValidMachineId,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: ValidBarCycleTime,
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: default));

        Assert.Contains("DueDate", ex.Message);
    }

    [Fact]
    public void DueDateSetter_WhenValueIsDefault_ThrowsDomainException()
    {
        var job = CreateValidJob();
        var ex = Assert.Throws<DomainException>(() => job.DueDate = default);
        Assert.Contains("DueDate", ex.Message);
    }

    [Fact]
    public void DueDateSetter_WhenValueIsValid_UpdatesProperty()
    {
        var job = CreateValidJob();
        var newDate = new DateOnly(2027, 1, 15);
        job.DueDate = newDate;
        Assert.Equal(newDate, job.DueDate);
    }

    #endregion

    #region Method Tests

    [Fact]
    public void Inactivate_WhenJobIsActive_SetsInactivatedDateTime()
    {
        var job = CreateValidJob();
        Assert.Null(job.InactivatedDateTime);

        job.Inactivate();

        Assert.NotNull(job.InactivatedDateTime);
        Assert.True(job.InactivatedDateTime.Value <= DateTimeOffset.UtcNow);
        Assert.True(job.InactivatedDateTime.Value >= DateTimeOffset.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Inactivate_WhenJobIsActive_SetsInactivatedByUserId()
    {
        var job = CreateValidJob();
        var userId = 42;

        job.Inactivate(userId);

        Assert.NotNull(job.InactivatedDateTime);
        Assert.Equal(userId, job.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenJobIsAlreadyInactivated_ThrowsDomainException()
    {
        var job = CreateValidJob();
        job.Inactivate();

        var ex = Assert.Throws<DomainException>(() => job.Inactivate());
        Assert.Contains("already inactivated", ex.Message);
    }

    #endregion

    private static Job CreateValidJob() =>
        new(
            orderId: ValidOrderId,
            stockLotId: ValidStockLotId,
            machineId: ValidMachineId,
            partAmountPlanned: ValidPartAmountPlanned,
            barAmountPlanned: ValidBarAmountPlanned,
            barCycleTime: ValidBarCycleTime,
            barsInJob: ValidBarsInJob,
            estimatedPartsPerBar: ValidEstimatedPartsPerBar,
            dueDate: new DateOnly(2026, 6, 1));
}

