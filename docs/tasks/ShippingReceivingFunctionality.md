# Inventory & Workflow Architecture Rules

---

# StockLot vs StockLotAdjustment (Hybrid Rule)

## Relationship Rule

`StockLotAdjustment` is the source of change.  
`StockLot.AmountOfBars` is a cached current total.

The bar amount on a StockLot:

- MUST only change as a result of a `StockLotAdjustment`.
- MUST never be edited directly.
- EXISTS only as a convenience for fast reads.

Every change to inventory must:

1. Create or modify a `StockLotAdjustment`.
2. Update `StockLot.AmountOfBars` in the same transaction.

This guarantees:
- A permanent audit record of all inventory movement.
- A fast-access current quantity.
- No silent inventory changes.

---

# Inventory Integrity Rule

## Core Rule

Any change in effective inventory must be represented by:

- Creating a new active `StockLotAdjustment`, or
- Changing the active state of an existing `StockLotAdjustment`.

No other workflow may modify `StockLot.AmountOfBars`.

---

## Activation / Inactivation Behavior

When an adjustment's active state changes:

- If an adjustment is inactivated, its `DeltaBars` effect must be removed from `StockLot.AmountOfBars`.
- If an adjustment is activated, its `DeltaBars` effect must be applied to `StockLot.AmountOfBars`.

State changes must be idempotent:

- Inactivating an already inactive adjustment does nothing.
- Activating an already active adjustment does nothing.

---

## Enforcement

- `StockLot.AmountOfBars` must never be directly edited.
- All inventory changes must flow through adjustment creation or adjustment state changes.
- The cached total and adjustment state changes must occur within the same transaction.

---

# Controller Organization Rule

## Goal

Keep the API easy to understand and avoid creating controllers that aren't actively used.

## Rule

Controllers are organized by **purpose**, not strictly by database tables and not strictly by roles.

We use two buckets:

### 1) CRUD Controllers

- Location: `CncApp.Api/Controllers/Crud/`
- Purpose: Single-entity endpoints (table/aggregate focused)
- Examples: `MaterialsController`, `StockLotsController`, `StockLotAdjustmentsController`, etc.

### 2) Workflow Controllers

- Location: `CncApp.Api/Controllers/Workflow/`
- Purpose: Endpoints that perform a single business workflow spanning multiple entities/tables in one request/transaction.
- Workflow controllers are created only when that workflow is actively being implemented.

---

## Current Implementation

One workflow controller exists for Shipping & Receiving:

- File: `CncApp.Api/Controllers/Workflow/ShippingReceivingController.cs`
- Route: `api/ShippingReceiving`
- Endpoint: `POST /api/ShippingReceiving/receive`
- Authorization: `Admin` role required

This controller represents a business workflow, not a role.
The controller is thin — it delegates entirely to `ShippingReceivingService`.

---

## Inventory Write Restriction

CRUD endpoints must not directly modify `StockLot.AmountOfBars`.

Inventory quantity must only change through:

- Creating a `StockLotAdjustment`, or
- Activating/Inactivating a `StockLotAdjustment`.

---

# ShippingReceiving Workflow Transaction Rule

## Goal

Ensure the ShippingReceiving workflow executes atomically.

Creating:

- Material (if needed)
- StockLot (with `AmountOfBars = 0`)
- StockLotAdjustment (with `DeltaBars` = received quantity, `Reason = Received`)
- Updating `StockLot.AmountOfBars` from the adjustment's `DeltaBars`

must succeed or fail as a single unit.

---

## Transaction Rule

All multi-entity workflows inside `ShippingReceivingService`
must be wrapped in an explicit database transaction.

All participating services share the same scoped `DbContext` instance
(guaranteed by ASP.NET Core scoped DI).

---

## ITransactionManager

Because the Application layer does not reference `Microsoft.EntityFrameworkCore`,
the workflow service cannot access `AppDbContext.Database` directly.

Transaction control is exposed through a thin interface:

- Interface: `CncApp.Application/Interfaces/ITransactionManager.cs`
- Implementation: `CncApp.Infrastructure/Services/TransactionManager.cs`
- Registration: `AddScoped<ITransactionManager, TransactionManager>()` in `Infrastructure/DependencyInjection.cs`

The interface exposes three methods:

```csharp
public interface ITransactionManager
{
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
```

The implementation wraps `AppDbContext.Database.BeginTransactionAsync()` internally.
This follows the same interface-in-Application / implementation-in-Infrastructure
convention used by repositories and `ICurrentUserService`.

---

## Implemented Transaction Pattern

```csharp
await _transactionManager.BeginTransactionAsync(ct);

try
{
    // Step 1: Resolve or create Material
    materialId = dto.MaterialId ?? await _materialService.CreateAsync(..., ct);

    // Step 2: Create StockLot with AmountOfBars = 0
    stockLotId = await _stockLotService.CreateAsync(..., ct);

    // Step 3: Create StockLotAdjustment with DeltaBars and Reason = Received
    adjustmentId = await _stockLotAdjustmentService.CreateAsync(..., ct);

    // Step 4: Apply DeltaBars to StockLot.AmountOfBars
    var stockLot = await _stockLotRepository.GetByIdAsync(stockLotId, ct);
    stockLot.AmountOfBars += dto.AmountOfBars;
    await _stockLotRepository.SaveChangesAsync(ct);

    await _transactionManager.CommitTransactionAsync(ct);
}
catch
{
    await _transactionManager.RollbackTransactionAsync(ct);
    throw;
}
```

### Why existing service methods work inside the transaction

Each single-entity `CreateAsync` method calls `SaveChangesAsync` internally.
Inside an open EF Core transaction, `SaveChangesAsync` sends SQL to the database
but does **not** commit. Only `CommitTransactionAsync` finalizes the transaction.
If any step throws, `RollbackTransactionAsync` undoes all SQL sent within the transaction.

### Why StockLot is created with AmountOfBars = 0

The inventory integrity rule requires that `AmountOfBars` only changes
as a result of a `StockLotAdjustment`. The workflow enforces this by:

1. Creating the StockLot with `AmountOfBars = 0`.
2. Creating a `StockLotAdjustment` with `DeltaBars` = the received quantity.
3. Applying the adjustment's `DeltaBars` to the StockLot.

All within the same transaction.

---

# Application Service Structure Rule

## Goal

Keep single-entity services and multi-entity workflows clearly separated
while maintaining the existing partial-class pattern.

---

## Single-Entity Services

Location:

CncApp.Application/Services/<Entity>/

Pattern:

- Commands/
- Queries/
- <Entity>Service.cs (partial class root)

Examples:

- Materials/
- StockLots/
- StockLotAdjustments/

These services operate on one aggregate only.

---

## Workflow Services

Multi-entity business workflows live under:

CncApp.Application/Services/Workflows/

Each workflow gets its own folder.

---

## ShippingReceiving (Implemented Structure)

CncApp.Application/
  Services/
    Workflows/
      ShippingReceiving/
        Commands/
          ShippingReceivingService.ReceiveShipment.cs
        ShippingReceivingService.cs

---

## ShippingReceivingService Dependencies

The partial root (`ShippingReceivingService.cs`) injects:

| Dependency | Purpose |
|---|---|
| `MaterialService` | Create material (if new) |
| `StockLotService` | Create stock lot |
| `StockLotAdjustmentService` | Create adjustment |
| `IStockLotRepository` | Fetch and update `StockLot.AmountOfBars` after adjustment |
| `ITransactionManager` | Begin / commit / rollback the database transaction |

`IStockLotRepository` is injected directly because updating `AmountOfBars`
is not exposed by `StockLotService` (its `UpdateAsync` is metadata-only).
This is the correct boundary — the workflow service owns the inventory-update rule.

---

## Rule

- Workflow services orchestrate multiple entity services.
- Controllers remain thin.
- Workflow services own transaction boundaries.
- We do not mix workflow logic into single-entity services.
- We only create workflow folders when actively implementing that workflow.

---

This keeps the architecture:

- Organized
- Scalable
- Consistent with existing partial service pattern
- Free from premature abstraction

---

# DTOs

## ReceiveShipmentRequestDto

Location: `CncApp.Application/Dtos/ShippingReceiving/ReceiveShipmentRequestDto.cs`

| Field | Type | Required | Notes |
|---|---|---|---|
| MaterialId | `int?` | No | If provided, uses existing material. If null, creates new. |
| HeatNumber | `string?` | Conditional | Required when `MaterialId` is null. Max 100. |
| MaterialName | `string?` | Conditional | Required when `MaterialId` is null. Max 100. |
| LotNumber | `string` | Yes | Max 100. |
| AmountOfBars | `int` | Yes | Minimum 1. Becomes `DeltaBars` on the adjustment. |
| Diameter | `decimal` | Yes | |
| BarLength | `decimal` | Yes | |
| Condition | `StockLotConditionEnum` | Yes | |
| CheckedInDateTime | `DateTime` | Yes | |
| Notes | `string?` | No | Passed to the adjustment. Max 2000. |

## ReceiveShipmentResponseDto

Location: `CncApp.Application/Dtos/ShippingReceiving/ReceiveShipmentResponseDto.cs`

| Field | Type | Notes |
|---|---|---|
| MaterialId | `int` | Existing or newly created |
| StockLotId | `int` | Newly created |
| StockLotAdjustmentId | `int` | Newly created |

---

# Partial Class & Test Symmetry Rule

## Goal

Maintain the existing slice-by-slice pattern used throughout the application.

We do not create large monolithic service classes.
We split functionality into partial classes per command/query.

This pattern must also apply to workflow services.

---

## Workflow Service Structure (Partial Pattern)

Each workflow service follows the same structure as single-entity services.

Implemented:

CncApp.Application/
  Services/
    Workflows/
      ShippingReceiving/
        Commands/
          ShippingReceivingService.ReceiveShipment.cs
        ShippingReceivingService.cs

Rules:

- `ShippingReceivingService.cs` contains:
  - Constructor
  - Injected dependencies
  - Shared private fields

- Each workflow action lives in its own partial file under `Commands/`
- We do not place multiple workflow methods inside one large file.
- We only create command files when actively implementing them.

---

## Application Test Structure (Mirror Rule)

The Application test project mirrors the Application service structure.

Implemented:

CncApp.Application.Tests/
  Services/
    Workflows/
      ShippingReceiving/
        Commands/
          ShippingReceivingTests.ReceiveShipment.cs
        ShippingReceivingTests.cs

Rules:

- Test root file (`ShippingReceivingTests.cs`) contains:
  - Shared mocks (repositories, mapper, transaction manager)
  - Real service instances (MaterialService, StockLotService, StockLotAdjustmentService) created with mocked dependencies
  - `ShippingReceivingService` instance wired with the above
- Each workflow command gets its own test file.
- Folder structure matches Application structure for clarity.
- Tests verify workflow behavior (atomicity, invariants, orchestration),
  not database internals.

---

## Test Coverage (ReceiveShipment)

| Test | What it verifies |
|---|---|
| `ReceiveShipmentAsync_WithNewMaterial_CreatesAllEntitiesAndCommits` | Full happy path with new material; all IDs returned; transaction committed; AmountOfBars updated |
| `ReceiveShipmentAsync_WithExistingMaterial_SkipsMaterialCreation` | Existing MaterialId is passed through; material repository Add is never called |
| `ReceiveShipmentAsync_WhenStockLotCreationFails_RollsBackTransaction` | Exception triggers rollback; commit is never called |
| `ReceiveShipmentAsync_StockLotCreatedWithZeroBars_BeforeAdjustment` | StockLot DTO has `AmountOfBars = 0` (not the incoming quantity) |
| `ReceiveShipmentAsync_AdjustmentUsesReceivedReason` | Adjustment DTO has `Reason = Received`, correct `DeltaBars`, and `Notes` passed through |

---

## DI Registration Summary

| Registration | Location |
|---|---|
| `AddScoped<ShippingReceivingService>()` | `CncApp.Application/DependencyInjection.cs` |
| `AddScoped<ITransactionManager, TransactionManager>()` | `CncApp.Infrastructure/DependencyInjection.cs` |

---

## Architectural Principle

Single-entity services handle one aggregate.

Workflow services orchestrate multiple aggregates.

Both follow the same partial-class slice pattern.

This ensures:

- Structural consistency
- Predictable navigation
- Clean scaling as new workflows are added
- No deviation from established project conventions
