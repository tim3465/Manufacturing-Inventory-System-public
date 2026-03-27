using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Workflows.SupervisorDashboard;

public partial class SupervisorDashboardTests
{
    [Fact]
    public async Task GetDashboardAsync_WhenNoOpenShifts_ReturnsZeroHeaderStats()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        MockShiftRepository
            .Setup(r => r.ListOpenWithContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift>());
        MockShiftRepository
            .Setup(r => r.ListStartedTodayAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift>());
        MockJobRepository
            .Setup(r => r.ListLateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Job>());

        // Act
        var result = await Service.GetDashboardAsync();

        // Assert
        Assert.Equal(0, result.MachinesRunning);
        Assert.Equal(0, result.OperatorsActive);
        Assert.Equal(0, result.LateJobs);
        Assert.Empty(result.Operators);

        MockShiftRepository.Verify(r => r.ListOpenWithContextAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockShiftRepository.Verify(r => r.ListStartedTodayAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Once);
        MockJobRepository.Verify(r => r.ListLateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDashboardAsync_WithLateJobs_ReturnsCorrectLateJobCount()
    {
        // Arrange
        var lateJob1 = new Job(orderId: 1, stockLotId: null, machineId: 1,
            partAmountPlanned: 100, barAmountPlanned: 10,
            barCycleTime: TimeSpan.FromMinutes(5),
            estimatedPartsPerBar: 10,
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2))) { Id = 1 };
        var lateJob2 = new Job(orderId: 2, stockLotId: null, machineId: 2,
            partAmountPlanned: 50, barAmountPlanned: 5,
            barCycleTime: TimeSpan.FromMinutes(5),
            estimatedPartsPerBar: 10,
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))) { Id = 2 };

        MockShiftRepository
            .Setup(r => r.ListOpenWithContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift>());
        MockShiftRepository
            .Setup(r => r.ListStartedTodayAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift>());
        MockJobRepository
            .Setup(r => r.ListLateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Job> { lateJob1, lateJob2 });

        // Act
        var result = await Service.GetDashboardAsync();

        // Assert
        Assert.Equal(2, result.LateJobs);
    }

    [Fact]
    public async Task GetDashboardAsync_WithOpenShifts_ReturnsCorrectHeaderStats()
    {
        // Arrange
        var machine1 = new Machine("SN-001", "MODEL-A") { Id = 1 };
        var machine2 = new Machine("SN-002", "MODEL-B") { Id = 2 };
        var part = new Part("WidgetA", "PN-001", TimeSpan.FromMinutes(2), 5) { Id = 1 };
        var order = new Order(partId: 1, customerId: 1, partAmountRequested: 100) { Id = 1 };
        order.Part = part;

        var job1 = new Job(orderId: 1, stockLotId: null, machineId: 1,
            partAmountPlanned: 100, barAmountPlanned: 10,
            barCycleTime: TimeSpan.FromMinutes(5),
            estimatedPartsPerBar: 10,
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))) { Id = 1 };
        job1.Machine = machine1;
        job1.Order = order;

        var job2 = new Job(orderId: 1, stockLotId: null, machineId: 2,
            partAmountPlanned: 100, barAmountPlanned: 10,
            barCycleTime: TimeSpan.FromMinutes(5),
            estimatedPartsPerBar: 10,
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))) { Id = 2 };
        job2.Machine = machine2;
        job2.Order = order;

        var operator1 = new User { Id = 10, UserName = "op1", FirstName = "Alice", LastName = "Smith" };
        var operator2 = new User { Id = 11, UserName = "op2", FirstName = "Bob", LastName = "Jones" };

        var shift1 = new Shift(jobId: 1, operatorId: 10, barsConsumed: 1,
            startTime: DateTime.UtcNow.AddHours(-2)) { Id = 1 };
        shift1.Job = job1;
        shift1.Operator = operator1;

        var shift2 = new Shift(jobId: 2, operatorId: 11, barsConsumed: 1,
            startTime: DateTime.UtcNow.AddHours(-1)) { Id = 2 };
        shift2.Job = job2;
        shift2.Operator = operator2;

        MockShiftRepository
            .Setup(r => r.ListOpenWithContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift> { shift1, shift2 });
        MockShiftRepository
            .Setup(r => r.ListStartedTodayAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift>());
        MockJobRepository
            .Setup(r => r.ListLateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Job>());

        // Act
        var result = await Service.GetDashboardAsync();

        // Assert
        Assert.Equal(2, result.MachinesRunning);
        Assert.Equal(2, result.OperatorsActive);
        Assert.Equal(0, result.LateJobs);
        Assert.Equal(2, result.Operators.Count);
    }

    [Fact]
    public async Task GetDashboardAsync_WithTodayShifts_ComputesPartsMadeAndScrapPerOperator()
    {
        // Arrange
        var machine1 = new Machine("SN-001", "MODEL-A") { Id = 1 };
        var part = new Part("WidgetA", "PN-001", TimeSpan.FromMinutes(2), 5) { Id = 1 };
        var order = new Order(partId: 1, customerId: 1, partAmountRequested: 100) { Id = 1 };
        order.Part = part;

        var job1 = new Job(orderId: 1, stockLotId: null, machineId: 1,
            partAmountPlanned: 100, barAmountPlanned: 10,
            barCycleTime: TimeSpan.FromMinutes(5),
            estimatedPartsPerBar: 10,
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))) { Id = 1 };
        job1.Machine = machine1;
        job1.Order = order;

        var operator1 = new User { Id = 10, UserName = "op1", FirstName = "Alice", LastName = "Smith" };

        // Open shift for operator 10
        var openShift = new Shift(jobId: 1, operatorId: 10, barsConsumed: 1,
            startTime: DateTime.UtcNow.AddHours(-2), partsMade: 8, scrap: 2) { Id = 1 };
        openShift.Job = job1;
        openShift.Operator = operator1;

        // Today's shifts for operator 10 (includes open + a closed earlier shift)
        var todayShift1 = new Shift(jobId: 1, operatorId: 10, barsConsumed: 1,
            startTime: DateTime.UtcNow.AddHours(-6), partsMade: 20, scrap: 5) { Id = 10 };
        var todayShift2 = new Shift(jobId: 1, operatorId: 10, barsConsumed: 1,
            startTime: DateTime.UtcNow.AddHours(-2), partsMade: 8, scrap: 2) { Id = 1 };

        MockShiftRepository
            .Setup(r => r.ListOpenWithContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift> { openShift });
        MockShiftRepository
            .Setup(r => r.ListStartedTodayAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift> { todayShift1, todayShift2 });
        MockJobRepository
            .Setup(r => r.ListLateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Job>());

        // Act
        var result = await Service.GetDashboardAsync();

        // Assert
        Assert.Single(result.Operators);
        var op = result.Operators[0];
        Assert.Equal(10, op.OperatorId);
        Assert.Equal("Alice Smith", op.OperatorName);
        Assert.Equal(28, op.PartsMadeToday);   // 20 + 8
        Assert.Equal(7, op.ScrapToday);         // 5 + 2
        // total = 35, scrap% = 7/35*100 = 20.0
        Assert.Equal(20.0m, op.ScrapPercentage);
    }

    [Fact]
    public async Task GetDashboardAsync_WhenNoPartsProduced_ScrapPercentageIsZero()
    {
        // Arrange
        var machine1 = new Machine("SN-001", "MODEL-A") { Id = 1 };
        var part = new Part("WidgetA", "PN-001", TimeSpan.FromMinutes(2), 5) { Id = 1 };
        var order = new Order(partId: 1, customerId: 1, partAmountRequested: 100) { Id = 1 };
        order.Part = part;

        var job1 = new Job(orderId: 1, stockLotId: null, machineId: 1,
            partAmountPlanned: 100, barAmountPlanned: 10,
            barCycleTime: TimeSpan.FromMinutes(5),
            estimatedPartsPerBar: 10,
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))) { Id = 1 };
        job1.Machine = machine1;
        job1.Order = order;

        var operator1 = new User { Id = 10, UserName = "op1", FirstName = "Alice", LastName = "Smith" };

        var openShift = new Shift(jobId: 1, operatorId: 10, barsConsumed: 1,
            startTime: DateTime.UtcNow.AddHours(-1), partsMade: 0, scrap: 0) { Id = 1 };
        openShift.Job = job1;
        openShift.Operator = operator1;

        MockShiftRepository
            .Setup(r => r.ListOpenWithContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift> { openShift });
        MockShiftRepository
            .Setup(r => r.ListStartedTodayAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift>());
        MockJobRepository
            .Setup(r => r.ListLateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Job>());

        // Act
        var result = await Service.GetDashboardAsync();

        // Assert
        Assert.Single(result.Operators);
        Assert.Equal(0m, result.Operators[0].ScrapPercentage);
    }

    [Fact]
    public async Task GetDashboardAsync_WithSingleOperatorOnTwoMachines_CountsMachinesCorrectly()
    {
        // Arrange
        var machine1 = new Machine("SN-001", "MODEL-A") { Id = 1 };
        var machine2 = new Machine("SN-002", "MODEL-B") { Id = 2 };
        var part = new Part("WidgetA", "PN-001", TimeSpan.FromMinutes(2), 5) { Id = 1 };
        var order = new Order(partId: 1, customerId: 1, partAmountRequested: 100) { Id = 1 };
        order.Part = part;

        var job1 = new Job(orderId: 1, stockLotId: null, machineId: 1,
            partAmountPlanned: 100, barAmountPlanned: 10,
            barCycleTime: TimeSpan.FromMinutes(5),
            estimatedPartsPerBar: 10,
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))) { Id = 1 };
        job1.Machine = machine1;
        job1.Order = order;

        var job2 = new Job(orderId: 1, stockLotId: null, machineId: 2,
            partAmountPlanned: 100, barAmountPlanned: 10,
            barCycleTime: TimeSpan.FromMinutes(5),
            estimatedPartsPerBar: 10,
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))) { Id = 2 };
        job2.Machine = machine2;
        job2.Order = order;

        var operator1 = new User { Id = 10, UserName = "op1", FirstName = "Alice", LastName = "Smith" };

        var shift1 = new Shift(jobId: 1, operatorId: 10, barsConsumed: 1,
            startTime: DateTime.UtcNow.AddHours(-3)) { Id = 1 };
        shift1.Job = job1;
        shift1.Operator = operator1;

        var shift2 = new Shift(jobId: 2, operatorId: 10, barsConsumed: 1,
            startTime: DateTime.UtcNow.AddHours(-2)) { Id = 2 };
        shift2.Job = job2;
        shift2.Operator = operator1;

        MockShiftRepository
            .Setup(r => r.ListOpenWithContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift> { shift1, shift2 });
        MockShiftRepository
            .Setup(r => r.ListStartedTodayAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift>());
        MockJobRepository
            .Setup(r => r.ListLateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Job>());

        // Act
        var result = await Service.GetDashboardAsync();

        // Assert: one operator, two machines running (global and per-operator)
        Assert.Equal(2, result.MachinesRunning);
        Assert.Equal(1, result.OperatorsActive);
        Assert.Single(result.Operators);

        var op = result.Operators[0];
        Assert.Equal(10, op.OperatorId);
        Assert.Equal(2, op.MachinesRunning);
        Assert.Equal(2, op.ActiveJobs.Count);
    }

    [Fact]
    public async Task GetDashboardAsync_ActiveJobDto_ContainsCorrectPartNameAndMachineName()
    {
        // Arrange
        var machine = new Machine("SN-XYZ", "MODEL-C") { Id = 5 };
        var part = new Part("GearA", "PN-999", TimeSpan.FromMinutes(3), 2) { Id = 7 };
        var order = new Order(partId: 7, customerId: 1, partAmountRequested: 50) { Id = 3 };
        order.Part = part;

        var job = new Job(orderId: 3, stockLotId: null, machineId: 5,
            partAmountPlanned: 50, barAmountPlanned: 5,
            barCycleTime: TimeSpan.FromMinutes(3),
            estimatedPartsPerBar: 10,
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))) { Id = 9 };
        job.Machine = machine;
        job.Order = order;

        var op = new User { Id = 20, UserName = "op2", FirstName = "John", LastName = "Doe" };

        var shift = new Shift(jobId: 9, operatorId: 20, barsConsumed: 1,
            startTime: DateTime.UtcNow.AddHours(-1)) { Id = 5 };
        shift.Job = job;
        shift.Operator = op;

        MockShiftRepository
            .Setup(r => r.ListOpenWithContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift> { shift });
        MockShiftRepository
            .Setup(r => r.ListStartedTodayAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift>());
        MockJobRepository
            .Setup(r => r.ListLateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Job>());

        // Act
        var result = await Service.GetDashboardAsync();

        // Assert
        Assert.Single(result.Operators);
        var operatorDto = result.Operators[0];
        Assert.Equal("John Doe", operatorDto.OperatorName);
        Assert.Single(operatorDto.ActiveJobs);

        var activeJob = operatorDto.ActiveJobs[0];
        Assert.Equal(9, activeJob.JobId);
        Assert.Equal("GearA", activeJob.PartName);
        Assert.Equal("SN-XYZ", activeJob.MachineName);
    }
}
