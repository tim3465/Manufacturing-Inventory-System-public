# Backend File & Folder Map

Golden reference: **Machines** slice. When in doubt, mirror Machines.

---

## Two Service Types

| Type | Location | Purpose |
|------|----------|---------|
| Single-Entity | `Application/Services/{EntityPlural}/` | CRUD on one aggregate |
| Workflow | `Application/Services/Workflows/{WorkflowName}/` | Multi-entity business operation in one transaction |

Both use the same partial-class pattern. Workflow services own transaction boundaries via `ITransactionManager`.

---

## Layer-by-Layer Structure

### API — `CncApp.Api/`

```
Controllers/
├── {EntityPlural}Controller.cs          (single-entity CRUD)
├── AuthController.cs                     (login + ping)
└── Workflow/
    └── {WorkflowName}Controller.cs       (multi-entity workflows)
```

- Route: `[Route("api/[controller]")]`
- Injects **concrete** service class (not interface)
- Workflow controllers are thin — delegate entirely to a workflow service
- Only create workflow controllers when actively implementing that workflow

### Application — `CncApp.Application/`

#### DTOs

```
Dtos/
├── {EntityPlural}/
│   ├── {Entity}Dto.cs                         (response)
│   ├── Create{Entity}RequestDto.cs            (create request)
│   └── Update{Entity}RequestDto.cs            (update request, if supported)
└── {WorkflowName}/
    ├── {Action}RequestDto.cs                  (workflow request)
    └── {Action}ResponseDto.cs                 (workflow response)
```

Ledger tables use `{Entity}ResultDto.cs` instead of `{Entity}Dto.cs`.

#### Services — Single-Entity

```
Services/{EntityPlural}/
├── {Entity}Service.cs                         (partial root: constructor + deps)
├── Commands/
│   ├── {Entity}Service.Create.cs
│   ├── {Entity}Service.Update.cs              (if supported)
│   └── {Entity}Service.Inactivate.cs
└── Queries/
    ├── {Entity}Service.Get.cs
    ├── {Entity}Service.ListActive.cs
    └── {Entity}Service.ListAll.cs
```

#### Services — Workflow

```
Services/Workflows/{WorkflowName}/
├── {WorkflowName}Service.cs                   (partial root: constructor + deps)
└── Commands/
    └── {WorkflowName}Service.{Action}.cs      (one file per workflow action)
```

Workflow services inject other entity services + `ITransactionManager`. They may also inject a repository directly when the entity service doesn't expose the needed operation (e.g., `IStockLotRepository` for updating `AmountOfBars`).

#### Mapping

```
Mapping/
└── {Entity}Profile.cs                         (AutoMapper profile, one per entity)
```

#### Interfaces

```
Interfaces/
├── Repositories/
│   └── I{Entity}Repository.cs                (one per entity)
├── ICurrentUserService.cs
├── IIdentityProvisioningService.cs
└── ITransactionManager.cs
```

### Domain — `CncApp.Domain/`

```
Entities/
└── {Entity}.cs                                (all entities in one folder, no subfolders)
Common/
└── AuditableEntityBase.cs
Enums/
└── {EnumName}.cs
```

### Infrastructure — `CncApp.Infrastructure/`

#### Repositories

```
Repositories/{EntityPlural}/
├── {Entity}Repository.cs                      (partial root: constructor + _context)
├── Commands/
│   ├── {Entity}Repository.Add.cs
│   ├── {Entity}Repository.Update.cs           (if supported)
│   ├── {Entity}Repository.Inactivate.cs
│   └── {Entity}Repository.SaveChanges.cs
└── Queries/
    ├── {Entity}Repository.GetById.cs
    ├── {Entity}Repository.ListActive.cs
    └── {Entity}Repository.ListAll.cs
```

#### Persistence

```
Persistence/
├── AppDbContext.cs
├── Configurations/
│   └── {Entity}Configuration.cs               (one per entity, all in same folder)
└── Migrations/
```

#### Services (Infrastructure implementations of Application interfaces)

```
Services/
├── CurrentUserService.cs
├── IdentityProvisioningService.cs
└── TransactionManager.cs
```

### Tests

#### Application Tests

```
Application.Tests/Services/
├── {EntityPlural}/
│   ├── {Entity}Tests.cs                       (partial root: shared mocks + setup)
│   ├── Commands/
│   │   └── {Entity}Tests.{Method}.cs
│   └── Queries/
│       └── {Entity}Tests.{Method}.cs
└── Workflows/{WorkflowName}/
    ├── {WorkflowName}Tests.cs                 (partial root: shared mocks + real services wired with mocked deps)
    └── Commands/
        └── {WorkflowName}Tests.{Action}.cs
```

#### Domain Tests

```
Domain.Tests/Entities/
└── {Entity}Tests.cs                           (single file, uses #region, not partial)
```

---

## Namespace Rules

All partial class files for a type share the **same namespace**. Never include `.Commands` or `.Queries` in the namespace.

| Layer | Namespace |
|-------|-----------|
| API Controller | `CncApp.Api.Controllers` |
| Workflow Controller | `CncApp.Api.Controllers.Workflow` |
| Application DTO | `CncApp.Application.Dtos.{EntityPlural}` |
| Workflow DTO | `CncApp.Application.Dtos.{WorkflowName}` |
| Application Service (all partials) | `CncApp.Application.Services.{EntityPlural}` |
| Workflow Service (all partials) | `CncApp.Application.Services.Workflows.{WorkflowName}` |
| Mapping Profile | `CncApp.Application.Mapping` |
| Repository Interface | `CncApp.Application.Interfaces.Repositories` |
| Domain Entity | `CncApp.Domain.Entities` |
| Infrastructure Repository (all partials) | `CncApp.Infrastructure.Repositories` |
| EF Configuration | `CncApp.Infrastructure.Persistence.Configurations` |
| Application Tests (all partials) | `CncApp.Application.Tests.Services.{EntityPlural}` |
| Workflow Tests (all partials) | `CncApp.Application.Tests.Services.Workflows.{WorkflowName}` |
| Domain Tests | `CncApp.Domain.Tests.Entities` |

---

## Partial Class Pattern

**What splits** (partial classes with one method per file):
- Services (single-entity and workflow)
- Repositories
- Application Tests

**What stays together** (single file):
- Domain Entities
- Domain Tests (uses `#region`)
- DTOs (one file per DTO)
- Mapping Profiles
- EF Configurations
- Controllers

---

## DI Registration

| What | Where | Pattern |
|------|-------|---------|
| Entity Service | `Application/DependencyInjection.cs` | `AddScoped<{Entity}Service>()` (concrete) |
| Workflow Service | `Application/DependencyInjection.cs` | `AddScoped<{WorkflowName}Service>()` (concrete) |
| Repository | `Infrastructure/DependencyInjection.cs` | `AddScoped<I{Entity}Repository, {Entity}Repository>()` |
| Infrastructure Service | `Infrastructure/DependencyInjection.cs` | `AddScoped<IInterface, Implementation>()` |

Services are always registered and injected as **concrete types**. Only repositories and infrastructure services use interfaces.

---

## Current Entities

| Entity | Table Type | Has Update | Special Queries |
|--------|-----------|------------|-----------------|
| Machine | Snapshot | No | — |
| Material | Lookup | Yes | — |
| Part | Lookup | Yes | — |
| User | Snapshot | Yes (roles only) | GetCurrentUser, GetRoles |
| StockLot | Snapshot | Yes (metadata) | — |
| Order | Snapshot | Yes | — |
| Job | Snapshot | Yes | — |
| Shift | Ledger | No | — |
| StockLotAdjustment | Ledger | Yes (notes only) | ListByStockLot |

## Current Workflows

| Workflow | Controller Route | Actions |
|----------|-----------------|---------|
| ShippingReceiving | `api/ShippingReceiving` | `ReceiveShipment` — atomically creates Material (if new) + StockLot + StockLotAdjustment + updates AmountOfBars |

---

## Hard Rules

1. **One method per file** in Commands/Queries folders
2. **No `.Commands` / `.Queries` in namespaces** — all partials share the slice root namespace
3. **Repositories never call `SaveChangesAsync`** inside Add/Inactivate/Update — the service calls it
4. **Repository mutations must call domain methods** (e.g., `entity.Inactivate(userId)`), not set properties directly
5. **Services don't catch domain exceptions** — they bubble to `GlobalExceptionHandler`
6. **Controllers inject concrete services**, not interfaces
7. **No per-entity subfolders** in: `Domain/Entities/`, `Mapping/`, `Persistence/Configurations/`, `Domain.Tests/Entities/`
8. **Workflow services own transaction boundaries** — single-entity services never manage transactions
9. **`StockLot.AmountOfBars` must only change through a `StockLotAdjustment`** in the same transaction
10. **Don't create workflow controllers/services until actively implementing** that workflow
