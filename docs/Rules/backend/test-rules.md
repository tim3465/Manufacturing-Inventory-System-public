---
category: backend-rules
area: test-rules
layer: backend
activation: passive
summary: Defines testing boundaries, structure, naming conventions, and responsibilities for domain and application tests.
keywords:
  - testing
  - unit tests
  - domain tests
  - application tests
  - workflow tests
  - test structure
  - test naming
use-when:
  - writing tests
  - structuring test files
  - deciding test boundaries
  - validating workflows
---

# Backend Test Rules

Two separate test projects. Each has a distinct responsibility and must not cross the boundary.

---

## The Two Projects

| Project | Path | What It Tests |
|---------|------|---------------|
| `CncApp.Domain.Tests` | `Entities/` | Entity invariants only |
| `CncApp.Application.Tests` | `Services/` | Service workflows only |

---

## Domain Tests (`CncApp.Domain.Tests`)

### What They Test

Entity invariants — can the entity be constructed with invalid data, can a property be set to an invalid value, can a domain method be called illegally.

### What They Never Do

- No database access
- No mocks
- No application services
- No repositories
- Do not re-test what other layers are responsible for

### File Structure

One non-partial file per entity, located under `Entities/`:

```
CncApp.Domain.Tests/
  Entities/
    MachineTests.cs
    MaterialTests.cs
    StockLotTests.cs
    ...
```

### Internal Organization

Use `#region` blocks to separate the three test groups inside the file:

```csharp
#region Constructor Tests
// test every required field: null, empty, whitespace, exceeds max length, at max length (boundary), valid
#endregion

#region Property Setter Tests
// same boundary cases as constructor for each settable property
#endregion

#region Method Tests (e.g. Inactivate)
// happy path, guard cases (double-inactivate throws), optional params (null userId)
#endregion
```

### Class Comment

Every domain test class has this XML summary:

```csharp
/// <summary>
/// Domain tests for {Entity} entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
```

### Test Naming

```
{Subject}_When{Condition}_{Outcome}
```

Examples:
- `Constructor_WhenSerialNumberIsNull_ThrowsDomainException`
- `Constructor_WhenSerialNumberIsMaxLength_CreatesMachine`
- `SerialNumberSetter_WhenValueExceedsMaxLength_ThrowsDomainException`
- `Inactivate_WhenMachineIsAlreadyInactivated_ThrowsDomainException`

### Boundary Requirement

Every string field must have tests for: `null`, empty (`""`), whitespace (`"   "`), exceeds max length (`new string('A', MaxLength + 1)`), and exactly at max length (`new string('A', MaxLength)`). Both the constructor and the setter get their own copies of these tests.

---

## Application Tests (`CncApp.Application.Tests`)

Application tests split into two sub-types depending on whether the service under test is a single-entity service or a workflow service.

---

### Single-Entity Service Tests

#### What They Test

That the service calls the correct repository and mapper methods in the right order and returns the correct result.

#### File Structure

Partial class split by operation, under `Services/{Entity}/`:

```
Services/Machines/
  MachineTests.cs          ← base: mocks + service construction
  Commands/
    MachineTests.Create.cs
    MachineTests.Inactivate.cs
  Queries/
    MachineTests.Get.cs
    MachineTests.ListAll.cs
    MachineTests.ListActive.cs
```

#### Base File Pattern

```csharp
public partial class MachineTests
{
    protected readonly Mock<IMachineRepository> MockRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly MachineService MachineService;

    public MachineTests()
    {
        MockRepository = new Mock<IMachineRepository>();
        MockMapper = new Mock<IMapper>();
        MachineService = new MachineService(MockRepository.Object, MockMapper.Object);
    }
}
```

Mocks are `protected` so partial files can access them directly.

#### What Each Test Verifies

- `.Setup(...)` the mock repository method that should be called
- `.Setup(...)` the mock mapper if a mapped object is needed
- Call the service method (Act)
- `Assert` the return value
- `.Verify(..., Times.Once)` for every mock interaction that must happen
- `.Verify(..., Times.Never)` for mock interactions that must NOT happen given the scenario

#### Test Naming

```
{MethodName}_When{Condition}_Returns{Result}
{MethodName}_When{Condition}_Calls{Dependency}
```

Examples:
- `CreateAsync_WhenValidDto_ReturnsCreatedId`
- `InactivateAsync_WhenEntityNotFound_ReturnsFalse`
- `GetAsync_WhenEntityExists_ReturnsMappedDto`

---

### Workflow Service Tests

Workflow services orchestrate multiple entity services and transactions. Their tests are structured differently.

#### File Structure

```
Services/Workflows/ShippingReceiving/
  ShippingReceivingTests.cs          ← base: all mocks + real sub-services + workflow service
  Commands/
    ShippingReceivingTests.ReceiveShipment.cs
```

#### Base File Pattern

Workflow tests mock all repositories and `ITransactionManager`, then instantiate **real** entity services (not mocked), and wire those into the workflow service:

```csharp
public partial class ShippingReceivingTests
{
    protected readonly Mock<IMaterialRepository> MockMaterialRepository;
    protected readonly Mock<IStockLotRepository> MockStockLotRepository;
    protected readonly Mock<IStockLotAdjustmentRepository> MockStockLotAdjustmentRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly Mock<ITransactionManager> MockTransactionManager;
    protected readonly ShippingReceivingService Service;

    public ShippingReceivingTests()
    {
        // real sub-services, not mocked — workflows test their orchestration
        var materialService = new MaterialService(MockMaterialRepository.Object, MockMapper.Object);
        var stockLotService = new StockLotService(MockStockLotRepository.Object, MockMapper.Object);
        var stockLotAdjustmentService = new StockLotAdjustmentService(...);

        Service = new ShippingReceivingService(
            materialService, stockLotService, stockLotAdjustmentService, MockTransactionManager.Object);
    }
}
```

The sub-services are real so that the test exercises the full call chain through the workflow, not just a stub.

#### What Each Workflow Test Verifies

- Transaction begins and commits on success
- Transaction begins and rolls back on failure
- Entity services are called in the correct sequence
- Repository methods that must NOT be called are verified with `Times.Never` (e.g., material creation is skipped when `MaterialId` is already provided)
- Business invariants upheld by the workflow (e.g., `StockLot.AmountOfBars` is 0 at creation, only updated via adjustment)

#### Transaction Verification Pattern

Every workflow test that involves a transaction must verify all three outcomes explicitly:

```csharp
MockTransactionManager.Verify(t => t.BeginTransactionAsync(...), Times.Once);
MockTransactionManager.Verify(t => t.CommitTransactionAsync(...), Times.Once);   // on success
MockTransactionManager.Verify(t => t.RollbackTransactionAsync(...), Times.Never); // on success

// or for failure path:
MockTransactionManager.Verify(t => t.BeginTransactionAsync(...), Times.Once);
MockTransactionManager.Verify(t => t.RollbackTransactionAsync(...), Times.Once);
MockTransactionManager.Verify(t => t.CommitTransactionAsync(...), Times.Never);
```

---

## Hard Rules

1. Domain tests never reference repositories, services, or mocks.
2. Application tests never test domain invariants (e.g., do not assert `DomainException` for bad field values).
3. The base file in application tests contains only mock declarations and the service constructor — no test methods.
4. `protected` visibility on mocks in the base file so partial files can access them.
5. Workflow tests use real sub-services, not mocked ones, so the orchestration is actually exercised.
6. Every workflow test verifies `BeginTransactionAsync`, `CommitTransactionAsync`, and `RollbackTransactionAsync` by name with explicit `Times` counts.
7. `Times.Never` is required wherever a mock should not be called given the scenario — do not omit it.
8. Application tests target the public surface area of services/workflows (public methods) and verify observable outcomes + key interactions.
   - Do NOT write tests solely for internal/private helper methods.
   - Internal logic is covered indirectly through tests of the public method(s) that use it.
   - Exception: if an internal method contains complex, reusable logic, extract it into a testable component (pure function / domain method / dedicated service) and test that component directly.