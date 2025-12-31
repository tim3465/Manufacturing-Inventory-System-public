# CNC App — Database Target Blueprint (Refactor Spec)

**Purpose:** This document describes the **desired (“end in mind”) database + EF Core model structure** for the CNC app so the current schema can be refactored cleanly and consistently (SOLID / Clean Architecture friendly).

> This is **not** a snapshot of what the database is today. It’s the **target structure** we want the codebase and migrations to move toward.

---

## 1) Guiding principles

### Clean Architecture boundaries
- **Domain** contains entities/value objects and business rules. **No EF Core attributes** required.
- **Application** contains use cases, DTOs, interfaces (repositories/services), validation.
- **Infrastructure** contains EF Core (DbContext, configurations, migrations, repository implementations).
- **API** is the composition root (DI wiring) and controllers.

### Database design philosophy
- Model the business (Domain) first, then map it to the database (Infrastructure EF Core config).
- Use a **consistent identity + auditing + soft-delete model** across entities.

---

## 2) Naming + conventions (target)

### Primary keys
- Every table has a primary key named: `Id`
- Use a single PK type consistently across the system (recommended: `int` identity).
  - If you ever change to `Guid`, change it *everywhere* in one sweep.

### Foreign keys
- Foreign keys should be named: `<RelatedEntityName>Id` (ex: `UserId`, `MachineId`, `OrderId`).
- Navigation properties should be named after the entity (`User`, `Machine`) or plural for collections (`Orders`, `Jobs`).

### Date/time
- Store timestamps in **UTC**.
- Prefer `DateTimeOffset` over `DateTime` for audit fields (recommended).
- All daytime property names ends with DateTime. For example "CreatedDateTime"

---

## 3) Base entity + auditing + soft delete (target)

This is the pattern you liked from the SOLID course (“every ID is just `Id`”) plus the audit + soft delete you want.

### Option A (recommended): Two base classes
Use one base class for identity, and a second for auditing/soft-delete.

#### `EntityBase`
- `Id`

#### `AuditableEntityBase : EntityBase`
- `CreatedAt` (UTC)
- `CreatedByUserId` (nullable if system-created)
- `UpdatedAt` (nullable)
- `UpdatedByUserId` (nullable)
- **Soft delete**
  - `InactivatedAt` (nullable)
  - `InactivatedByUserId` (nullable)

**Rules:**
- Most business tables should inherit `AuditableEntityBase`.
- Lookup/reference tables can inherit `EntityBase` only if you don’t care about auditing them.

### Soft delete behavior
- “Deleted” records are not removed; they are **inactivated**.
- Default queries should **exclude inactive** rows.
- In EF Core, implement a **global query filter** for entities that support soft delete:
  - include rows where `InactivatedAt IS NULL`

### Option B (acceptable): Owned audit value object
If you prefer an `AuditTrail` owned type, you *can* keep it, but the target from this doc is to make auditing **consistent and obvious** across entities. If you keep `AuditTrail`, ensure it includes `InactivatedAt/InactivatedByUserId` and is owned consistently everywhere.

---

## 4) Entity file organization (target)

You currently experimented with `*Base` classes (ex: `UserBase`) and derived classes to separate navigation properties.

### Preferred approach for “clean look”
**Keep each entity as a single type** (no `UserBase` / `User` inheritance), and use either:

- **Single file per entity**: simplest and most readable, OR
- **Partial classes for separation** (if you love the “split nav props” idea):
  - `User.cs` (core properties + rules)
  - `User.Navigations.cs` (navigation properties only)

> This keeps the domain model clean without introducing inheritance that implies “is-a” relationships that aren’t real in the business.

---

## 5) Current CNC entities (targetized)

These are the current entity concepts in the CNC project. The refactor goal is **structural** (base entity + audit + soft-delete + mapping), not changing the business meaning.

### Core entities (expected to be auditable + soft deletable)
- `User`
- `UserRole`
- `Material`
- `StockLot`
- `StockLotAdjustment`
- `Machine`
- `Part`
- `Order`
- `Job`
- `Shift`

**Rule:** Unless there’s a strong reason otherwise, each of the above should inherit `AuditableEntityBase`.

---

## 6) Relationship + delete behavior conventions (target)

### FK relationships
- Use explicit FK scalar properties (`UserId`, `OrderId`, etc.) plus navigation properties.
- Required vs optional relationships should be defined in **Fluent Config**, not data annotations.

### Delete behavior
- Default to `Restrict` (or `NoAction`) for important relationships to avoid cascade surprises.
- If you need cascading deletes, document the reason *per relationship*.

### Indexing (baseline)
- Add indexes for:
  - all foreign keys
  - fields used for common lookups/search (ex: `User.Email` if present)
  - “active” filtering: an index on `InactivatedAt` can help if tables get large

---

## 7) EF Core mapping rules (target)

### Where EF belongs
- `DbContext`, migrations, and all configuration classes live in **Infrastructure**.

### How configuration is organized
Prefer **one config class per entity**:

- `Infrastructure/Persistence/Configurations/UserConfiguration.cs`
- `Infrastructure/Persistence/Configurations/MachineConfiguration.cs`
- etc.

Each implements:
- `IEntityTypeConfiguration<TEntity>`

And `OnModelCreating` does:
- `modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);`

### Global query filters (soft delete)
- Apply a query filter to each soft-deletable entity (or via a helper) so inactive rows are excluded by default.

### Decimal precision
- Decide and standardize (examples):
  - money: `decimal(18,2)`
  - measured quantities: `decimal(18,4)`
- Configure precision in Fluent Config (don’t rely on defaults).

### Owned/value objects
- If you keep value objects (like `AuditTrail`), configure them as owned consistently.

---

## 8) Migration + refactor safety rules (target)

### Early-stage refactor rule
Because this is a portfolio project and still early:
- It’s acceptable to **reset migrations** after refactoring (drop local dev DB, recreate an `InitialCreate`).
- Keep a tag/branch of the pre-refactor version for history.

### Never lose intent
When refactoring entities:
- Preserve existing business fields (don’t rename/retype fields unless there’s a reason).
- Focus changes on:
  - base identity (`Id`)
  - audit + soft delete fields
  - removing per-entity `*Base` inheritance
  - moving constraints to Fluent Config

---

## 9) Acceptance checklist (definition of “done” for the refactor)

The refactor is considered successful when:

- [ ] All entities use `Id` consistently (no `UserId`, `MachineId` PK names, etc.)
- [ ] Entities inherit `EntityBase` / `AuditableEntityBase` (or consistently use owned `AuditTrail`)
- [ ] Soft delete implemented consistently (inactive fields exist and global filter is applied)
- [ ] No EF Core attributes are required on Domain entities (constraints live in Fluent Config)
- [ ] EF mapping uses `IEntityTypeConfiguration<T>` per entity and is assembly-scanned
- [ ] Infrastructure does **not** depend on Application (dependency direction is clean)
- [ ] A fresh migration builds the schema cleanly from scratch

---

## 10) Notes / open decisions (fill in as you refactor)

- **PK type:** `int` vs `Guid` (choose once; stay consistent)
- **Audit user linkage:** do audit fields reference `User.Id` (FK) or store a string/username?
- **System actions:** how to represent “system” user for CreatedBy/UpdatedBy?
- **Time type:** `DateTimeOffset` vs `DateTime` (recommended: `DateTimeOffset`)

---

### Appendix: Example base class shapes (pseudo)

> Keep these in Domain as simple C# classes (no EF attributes).

- `abstract class EntityBase { public int Id { get; set; } }`
- `abstract class AuditableEntityBase : EntityBase { /* Created/Updated/Inactivated fields */ }`

