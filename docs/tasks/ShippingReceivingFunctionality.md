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

## Current Locked Decision

We will create **one** workflow controller for Shipping & Receiving:

- Folder: `CncApp.Api/Controllers/Workflow/`
- Controller name: `ShippingReceivingController`

This controller represents a business workflow, not a role.

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
- StockLot
- StockLotAdjustment
- Updating `StockLot.AmountOfBars`

must succeed or fail as a single unit.

---

## Transaction Rule

All multi-entity workflows inside `ShippingReceivingService`
must be wrapped in an explicit database transaction.

All participating services must share the same scoped `DbContext` instance.

### Pattern

```csharp
await using var tx = await _context.Database.BeginTransactionAsync(ct);

try
{
    await _materialService.CreateAsync(..., ct);
    await _stockLotService.CreateAsync(..., ct);
    await _stockLotAdjustmentService.CreateAsync(..., ct);

    await tx.CommitAsync(ct);
}
catch
{
    await tx.RollbackAsync(ct);
    throw;
}

```

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

Multi-entity business workflows must live under:

CncApp.Application/Services/Workflows/

Each workflow gets its own folder.

---

## ShippingReceiving (Minimum Viable Structure)

For the current implementation, we will create only what is needed:

CncApp.Application/
  Services/
    Workflows/
      ShippingReceiving/
        Commands/
          ShippingReceivingService.ReceiveShipment.cs
        ShippingReceivingService.cs

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

# Partial Class & Test Symmetry Rule

## Goal

Maintain the existing slice-by-slice pattern used throughout the application.

We do not create large monolithic service classes.
We split functionality into partial classes per command/query.

This pattern must also apply to workflow services.

---

## Workflow Service Structure (Partial Pattern)

Each workflow service must follow the same structure as single-entity services.

Example:

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

The Application test project must mirror the Application service structure.

Example:

CncApp.Application.Tests/
  Services/
    Workflows/
      ShippingReceiving/
        Commands/
          ShippingReceivingTests.ReceiveShipment.cs
        ShippingReceivingTests.cs

Rules:

- Test root file (`ShippingReceivingTests.cs`) contains:
  - Shared setup
  - Shared test helpers
- Each workflow command gets its own test file.
- Folder structure must match Application structure for clarity.
- Tests verify workflow behavior (atomicity, invariants, orchestration),
  not database internals.

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