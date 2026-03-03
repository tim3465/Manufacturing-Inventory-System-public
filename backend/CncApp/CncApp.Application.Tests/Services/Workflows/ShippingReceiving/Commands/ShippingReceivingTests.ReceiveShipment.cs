using CncApp.Application.Dtos.Materials;
using CncApp.Application.Dtos.ShippingReceiving;
using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Application.Dtos.StockLots;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Workflows.ShippingReceiving;

public partial class ShippingReceivingTests
{
    [Fact]
    public async Task ReceiveShipmentAsync_WithNewMaterial_CreatesAllEntitiesAndCommits()
    {
        // Arrange
        var dto = new ReceiveShipmentRequestDto
        {
            HeatNumber = "HN-001",
            MaterialName = "Steel-A1",
            LotNumber = "LOT-001",
            AmountOfBars = 50,
            Diameter = 25.4m,
            BarLength = 3000m,
            Condition = StockLotConditionEnum.AsReceived,
            CheckedInDateTime = new DateTime(2026, 1, 15)
        };

        var material = new Material("HN-001", "Steel-A1") { Id = 1 };
        var stockLot = new StockLot("LOT-001", 1, 0, 25.4m, 3000m,
            StockLotConditionEnum.AsReceived, new DateTime(2026, 1, 15)) { Id = 10 };
        var adjustment = new StockLotAdjustment(10, 50, StockLotAdjustmentReasonEnum.Received) { Id = 100 };

        MockMapper
            .Setup(m => m.Map<Material>(It.IsAny<CreateMaterialRequestDto>()))
            .Returns(material);
        MockMapper
            .Setup(m => m.Map<StockLot>(It.IsAny<CreateStockLotRequestDto>()))
            .Returns(stockLot);
        MockMapper
            .Setup(m => m.Map<StockLotAdjustment>(It.IsAny<CreateStockLotAdjustmentRequestDto>()))
            .Returns(adjustment);

        MockMaterialRepository
            .Setup(r => r.AddAsync(It.IsAny<Material>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockMaterialRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockStockLotRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLot>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotRepository
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stockLot);

        MockStockLotAdjustmentRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLotAdjustment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotAdjustmentRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await Service.ReceiveShipmentAsync(dto);

        // Assert
        Assert.Equal(1, result.MaterialId);
        Assert.Equal(10, result.StockLotId);
        Assert.Equal(100, result.StockLotAdjustmentId);
        Assert.Equal(50, stockLot.AmountOfBars);

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReceiveShipmentAsync_WithExistingMaterial_SkipsMaterialCreation()
    {
        // Arrange
        var dto = new ReceiveShipmentRequestDto
        {
            MaterialId = 5,
            LotNumber = "LOT-002",
            AmountOfBars = 25,
            Diameter = 12.7m,
            BarLength = 6000m,
            Condition = StockLotConditionEnum.Ground,
            CheckedInDateTime = new DateTime(2026, 2, 20)
        };

        var stockLot = new StockLot("LOT-002", 5, 0, 12.7m, 6000m,
            StockLotConditionEnum.Ground, new DateTime(2026, 2, 20)) { Id = 20 };
        var adjustment = new StockLotAdjustment(20, 25, StockLotAdjustmentReasonEnum.Received) { Id = 200 };

        MockMapper
            .Setup(m => m.Map<StockLot>(It.IsAny<CreateStockLotRequestDto>()))
            .Returns(stockLot);
        MockMapper
            .Setup(m => m.Map<StockLotAdjustment>(It.IsAny<CreateStockLotAdjustmentRequestDto>()))
            .Returns(adjustment);

        MockStockLotRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLot>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotRepository
            .Setup(r => r.GetByIdAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stockLot);

        MockStockLotAdjustmentRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLotAdjustment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotAdjustmentRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await Service.ReceiveShipmentAsync(dto);

        // Assert
        Assert.Equal(5, result.MaterialId);
        Assert.Equal(20, result.StockLotId);
        Assert.Equal(200, result.StockLotAdjustmentId);
        Assert.Equal(25, stockLot.AmountOfBars);

        MockMaterialRepository.Verify(
            r => r.AddAsync(It.IsAny<Material>(), It.IsAny<CancellationToken>()), Times.Never);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReceiveShipmentAsync_WhenStockLotCreationFails_RollsBackTransaction()
    {
        // Arrange
        var dto = new ReceiveShipmentRequestDto
        {
            MaterialId = 1,
            LotNumber = "LOT-FAIL",
            AmountOfBars = 10,
            Diameter = 10m,
            BarLength = 1000m,
            Condition = StockLotConditionEnum.AsReceived,
            CheckedInDateTime = new DateTime(2026, 3, 1)
        };

        MockMapper
            .Setup(m => m.Map<StockLot>(It.IsAny<CreateStockLotRequestDto>()))
            .Throws(new InvalidOperationException("Simulated failure"));

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Service.ReceiveShipmentAsync(dto));

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReceiveShipmentAsync_StockLotCreatedWithZeroBars_BeforeAdjustment()
    {
        // Arrange
        var dto = new ReceiveShipmentRequestDto
        {
            MaterialId = 1,
            LotNumber = "LOT-ZERO",
            AmountOfBars = 30,
            Diameter = 20m,
            BarLength = 2000m,
            Condition = StockLotConditionEnum.Turned,
            CheckedInDateTime = new DateTime(2026, 1, 10)
        };

        CreateStockLotRequestDto? capturedStockLotDto = null;
        var stockLot = new StockLot("LOT-ZERO", 1, 0, 20m, 2000m,
            StockLotConditionEnum.Turned, new DateTime(2026, 1, 10)) { Id = 30 };
        var adjustment = new StockLotAdjustment(30, 30, StockLotAdjustmentReasonEnum.Received) { Id = 300 };

        MockMapper
            .Setup(m => m.Map<StockLot>(It.IsAny<CreateStockLotRequestDto>()))
            .Callback<object>(obj => capturedStockLotDto = (CreateStockLotRequestDto)obj)
            .Returns(stockLot);
        MockMapper
            .Setup(m => m.Map<StockLotAdjustment>(It.IsAny<CreateStockLotAdjustmentRequestDto>()))
            .Returns(adjustment);

        MockStockLotRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLot>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotRepository
            .Setup(r => r.GetByIdAsync(30, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stockLot);

        MockStockLotAdjustmentRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLotAdjustment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotAdjustmentRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await Service.ReceiveShipmentAsync(dto);

        // Assert: StockLot was created with AmountOfBars = 0 (not the incoming quantity)
        Assert.NotNull(capturedStockLotDto);
        Assert.Equal(0, capturedStockLotDto!.AmountOfBars);
    }

    [Fact]
    public async Task ReceiveShipmentAsync_AdjustmentUsesReceivedReason()
    {
        // Arrange
        var dto = new ReceiveShipmentRequestDto
        {
            MaterialId = 1,
            LotNumber = "LOT-REASON",
            AmountOfBars = 15,
            Diameter = 18m,
            BarLength = 2500m,
            Condition = StockLotConditionEnum.AsReceived,
            CheckedInDateTime = new DateTime(2026, 2, 1),
            Notes = "Test shipment"
        };

        CreateStockLotAdjustmentRequestDto? capturedAdjustmentDto = null;
        var stockLot = new StockLot("LOT-REASON", 1, 0, 18m, 2500m,
            StockLotConditionEnum.AsReceived, new DateTime(2026, 2, 1)) { Id = 40 };
        var adjustment = new StockLotAdjustment(40, 15, StockLotAdjustmentReasonEnum.Received,
            notes: "Test shipment") { Id = 400 };

        MockMapper
            .Setup(m => m.Map<StockLot>(It.IsAny<CreateStockLotRequestDto>()))
            .Returns(stockLot);
        MockMapper
            .Setup(m => m.Map<StockLotAdjustment>(It.IsAny<CreateStockLotAdjustmentRequestDto>()))
            .Callback<object>(obj => capturedAdjustmentDto = (CreateStockLotAdjustmentRequestDto)obj)
            .Returns(adjustment);

        MockStockLotRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLot>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotRepository
            .Setup(r => r.GetByIdAsync(40, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stockLot);

        MockStockLotAdjustmentRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLotAdjustment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotAdjustmentRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await Service.ReceiveShipmentAsync(dto);

        // Assert: Adjustment was created with Received reason and correct DeltaBars/Notes
        Assert.NotNull(capturedAdjustmentDto);
        Assert.Equal(StockLotAdjustmentReasonEnum.Received, capturedAdjustmentDto!.Reason);
        Assert.Equal(15, capturedAdjustmentDto.DeltaBars);
        Assert.Equal("Test shipment", capturedAdjustmentDto.Notes);
    }
}
