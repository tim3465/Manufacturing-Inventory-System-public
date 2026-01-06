# SliceMap: Canonical Folder/File Structure for One Table

## 1. Purpose

This document defines the **canonical folder and file structure** for implementing a single database table (a "slice") across all layers of the application: API, Application, Domain, Infrastructure, and Tests.

**Use this document when:**
- Creating a new slice/table implementation
- Verifying existing slice structure matches conventions
- Understanding where files belong for each layer

**Reference implementation:** The **Machines** slice is the ground-truth example. All new slices must mirror its structure exactly.

---

## 2. Golden Slice: Machines

The **Machines** slice (`backend/CncApp/CncApp.*/`) is the canonical reference because:
- It is fully implemented across all layers
- It follows all naming and structural conventions
- It demonstrates the partial class pattern correctly
- It has complete test coverage
- It uses correct namespace patterns (prefactored namespace correction applied)

**When in doubt, check the Machines slice files and mirror them exactly.**

---

## 3. Layer-by-Layer Folder Map

### API Layer (`CncApp.Api`)

```
CncApp.Api/
└── Controllers/
    └── {EntityPlural}Controller.cs
```

**Example:** `Controllers/MachinesController.cs`

**Rules:**
- Controller name: `{EntityPlural}Controller` (e.g., `MachinesController`)
- Route: `[Route("api/[controller]")]` → `/api/machines`
- Injects concrete service class (not interface)
- Methods: `CreateAsync`, `GetAsync`, `ListAsync`, `ListAllAsync`, `DeleteAsync`
- Authorization: `[Authorize(Roles = "Admin")]` for write operations, `[AllowAnonymous]` for reads

---

### Application Layer (`CncApp.Application`)

```
CncApp.Application/
├── Dtos/
│   └── {EntityPlural}/
│       ├── {Entity}Dto.cs                    (response DTO)
│       └── Create{Entity}RequestDto.cs      (request DTO)
├── Services/
│   └── {EntityPlural}/
│       ├── {Entity}Service.cs                (base partial: constructor + dependencies)
│       ├── Commands/
│       │   ├── {Entity}Service.Create.cs
│       │   └── {Entity}Service.Inactivate.cs
│       └── Queries/
│           ├── {Entity}Service.Get.cs
│           ├── {Entity}Service.ListActive.cs
│           └── {Entity}Service.ListAll.cs
├── Mapping/
│   └── {Entity}Profile.cs                     (AutoMapper profile)
└── Interfaces/
    └── Repositories/
        └── I{Entity}Repository.cs
```

**Example:** `Services/Machines/MachineService.cs` + `Commands/MachineService.Create.cs`

**Rules:**
- Service name: `{Entity}Service` (singular)
- DTO folder: `Dtos/{EntityPlural}/` (plural)
- Service folder: `Services/{EntityPlural}/` (plural)
- Mapping folder: `Mapping/` (shared, not per-entity)
- Interface folder: `Interfaces/Repositories/` (shared)

---

### Domain Layer (`CncApp.Domain`)

```
CncApp.Domain/
└── Entities/
    └── {Entity}.cs
```

**Example:** `Entities/Machine.cs`

**Rules:**
- Entity name: `{Entity}` (singular, e.g., `Machine`)
- All entities in shared `Entities/` folder (no per-entity subfolders)
- Inherits from `AuditableEntityBase` (for audit fields)
- Uses private constructor for EF Core materialization
- Public constructor with validation for domain creation
- Domain methods (e.g., `Inactivate()`) enforce invariants

---

### Infrastructure Layer (`CncApp.Infrastructure`)

```
CncApp.Infrastructure/
├── Repositories/
│   └── {EntityPlural}/
│       ├── {Entity}Repository.cs             (base partial: constructor + _context)
│       ├── Commands/
│       │   ├── {Entity}Repository.Add.cs
│       │   ├── {Entity}Repository.Inactivate.cs
│       │   └── {Entity}Repository.SaveChanges.cs
│       └── Queries/
│           ├── {Entity}Repository.GetById.cs
│           ├── {Entity}Repository.ListActive.cs
│           └── {Entity}Repository.ListAll.cs
└── Persistence/
    ├── Configurations/
    │   └── {Entity}Configuration.cs          (EF Core configuration)
    └── AppDbContext.cs                        (shared DbContext)
```

**Example:** `Repositories/Machines/MachineRepository.cs` + `Commands/MachineRepository.Add.cs`

**Rules:**
- Repository name: `{Entity}Repository` (singular)
- Repository folder: `Repositories/{EntityPlural}/` (plural)
- Configuration folder: `Persistence/Configurations/` (shared)
- Repository implements `I{Entity}Repository` from Application layer

---

### Tests

#### Application Tests (`CncApp.Application.Tests`)

```
CncApp.Application.Tests/
└── Services/
    └── {EntityPlural}/
        ├── {Entity}Tests.cs                   (base partial: shared setup/mocks)
        ├── Commands/
        │   ├── {Entity}Tests.Create.cs
        │   └── {Entity}Tests.Inactivate.cs
        └── Queries/
            ├── {Entity}Tests.Get.cs
            ├── {Entity}Tests.ListActive.cs
            └── {Entity}Tests.ListAll.cs
```

**Example:** `Services/Machines/MachineTests.cs` + `Commands/MachineTests.Inactivate.cs`

**Rules:**
- Test class name: `{Entity}Tests` (plural, e.g., `MachineTests`)
- Test folder: `Services/{EntityPlural}/` (plural)
- Base file contains shared mocks and service initialization
- Each method gets its own test file in appropriate Commands/Queries folder

#### Domain Tests (`CncApp.Domain.Tests`)

```
CncApp.Domain.Tests/
└── Entities/
    └── {Entity}Tests.cs
```

**Example:** `Entities/MachineTests.cs`

**Rules:**
- Test class name: `{Entity}Tests` (plural, e.g., `MachineTests`)
- All domain tests in shared `Entities/` folder
- Tests domain invariants only (no database, no mocks)
- Uses `#region` to organize: Constructor Tests, Property Setter Tests, Method Tests

---

## 4. Commands vs Queries Convention

### Where "Commands" Live

**Commands** = Write operations (mutations)

**Application Services:**
- Folder: `Application/Services/{EntityPlural}/Commands/`
- Files: `{Entity}Service.{MethodName}.cs`
- Examples: `MachineService.Create.cs`, `MachineService.Inactivate.cs`
- Methods: `CreateAsync`, `InactivateAsync`, `UpdateAsync` (if applicable)

**Infrastructure Repositories:**
- Folder: `Infrastructure/Repositories/{EntityPlural}/Commands/`
- Files: `{Entity}Repository.{MethodName}.cs`
- Examples: `MachineRepository.Add.cs`, `MachineRepository.Inactivate.cs`, `MachineRepository.SaveChanges.cs`
- Methods: `AddAsync`, `InactivateAsync`, `SaveChangesAsync`

**Application Tests:**
- Folder: `Application.Tests/Services/{EntityPlural}/Commands/`
- Files: `{Entity}Tests.{MethodName}.cs`
- Examples: `MachineTests.Create.cs`, `MachineTests.Inactivate.cs`

### Where "Queries" Live

**Queries** = Read operations (no mutations)

**Application Services:**
- Folder: `Application/Services/{EntityPlural}/Queries/`
- Files: `{Entity}Service.{MethodName}.cs`
- Examples: `MachineService.Get.cs`, `MachineService.ListActive.cs`, `MachineService.ListAll.cs`
- Methods: `GetAsync`, `ListActiveAsync`, `ListAllAsync`

**Infrastructure Repositories:**
- Folder: `Infrastructure/Repositories/{EntityPlural}/Queries/`
- Files: `{Entity}Repository.{MethodName}.cs`
- Examples: `MachineRepository.GetById.cs`, `MachineRepository.ListActive.cs`, `MachineRepository.ListAll.cs`
- Methods: `GetByIdAsync`, `ListActiveAsync`, `ListAllAsync`

**Application Tests:**
- Folder: `Application.Tests/Services/{EntityPlural}/Queries/`
- Files: `{Entity}Tests.{MethodName}.cs`
- Examples: `MachineTests.Get.cs`, `MachineTests.ListActive.cs`, `MachineTests.ListAll.cs`

### Expected File Naming Patterns

**Service Methods:**
- Command: `{Entity}Service.{CommandName}.cs` → `MachineService.Create.cs`
- Query: `{Entity}Service.{QueryName}.cs` → `MachineService.Get.cs`

**Repository Methods:**
- Command: `{Entity}Repository.{CommandName}.cs` → `MachineRepository.Add.cs`
- Query: `{Entity}Repository.{QueryName}.cs` → `MachineRepository.GetById.cs`

**Test Methods:**
- Command: `{Entity}Tests.{CommandName}.cs` → `MachineTests.Create.cs`
- Query: `{Entity}Tests.{QueryName}.cs` → `MachineTests.Get.cs`

### Partial Class Patterns

**What Files Split:**
- **Services:** Base file (`{Entity}Service.cs`) + one file per method in Commands/Queries folders
- **Repositories:** Base file (`{Entity}Repository.cs`) + one file per method in Commands/Queries folders
- **Application Tests:** Base file (`{Entity}Tests.cs`) + one file per method in Commands/Queries folders

**What Stays Together:**
- **Domain Entities:** Single file (`{Entity}.cs`) - no partial classes
- **Domain Tests:** Single file (`{Entity}Tests.cs`) - no partial classes (uses `#region` instead)
- **DTOs:** One file per DTO type
- **Mapping Profiles:** Single file (`{Entity}Profile.cs`)
- **EF Configurations:** Single file (`{Entity}Configuration.cs`)
- **Controllers:** Single file (`{EntityPlural}Controller.cs`)

**Partial Class Requirements:**
- All partial class files must share the **same namespace** (no `.Commands` or `.Queries` in namespace)
- Base file contains: constructor, private readonly fields for dependencies
- Method files contain: one method implementation per file
- All files must be marked `public partial class {Entity}Service` (or Repository/Tests)

---

## 5. Namespace Rules

### Prefactored Namespace Correction Result

**CRITICAL:** All partial class files for a given type must share the **same namespace** as the slice root. Do NOT include `.Commands` or `.Queries` in the namespace.

**Application Services:**
- ✅ Correct: `namespace CncApp.Application.Services.Machines;`
- ❌ Wrong: `namespace CncApp.Application.Services.Machines.Commands;`
- ❌ Wrong: `namespace CncApp.Application.Services.Machines.Queries;`

**Infrastructure Repositories:**
- ✅ Correct: `namespace CncApp.Infrastructure.Repositories;`
- ❌ Wrong: `namespace CncApp.Infrastructure.Repositories.Machines.Commands;`
- ❌ Wrong: `namespace CncApp.Infrastructure.Repositories.Machines.Queries;`

**Application Tests:**
- ✅ Correct: `namespace CncApp.Application.Tests.Services.Machines;`
- ❌ Wrong: `namespace CncApp.Application.Tests.Services.Machines.Commands;`
- ❌ Wrong: `namespace CncApp.Application.Tests.Services.Machines.Queries;`

**Complete Namespace Map:**

| Layer | File Location | Namespace |
|-------|---------------|-----------|
| API Controller | `Api/Controllers/{EntityPlural}Controller.cs` | `CncApp.Api.Controllers` |
| Application DTO | `Application/Dtos/{EntityPlural}/{Entity}Dto.cs` | `CncApp.Application.Dtos.{EntityPlural}` |
| Application Service | `Application/Services/{EntityPlural}/{Entity}Service.cs` | `CncApp.Application.Services.{EntityPlural}` |
| Application Service (Commands/Queries) | `Application/Services/{EntityPlural}/Commands/{Entity}Service.*.cs` | `CncApp.Application.Services.{EntityPlural}` |
| Application Mapping | `Application/Mapping/{Entity}Profile.cs` | `CncApp.Application.Mapping` |
| Application Interface | `Application/Interfaces/Repositories/I{Entity}Repository.cs` | `CncApp.Application.Interfaces.Repositories` |
| Domain Entity | `Domain/Entities/{Entity}.cs` | `CncApp.Domain.Entities` |
| Infrastructure Repository | `Infrastructure/Repositories/{EntityPlural}/{Entity}Repository.cs` | `CncApp.Infrastructure.Repositories` |
| Infrastructure Repository (Commands/Queries) | `Infrastructure/Repositories/{EntityPlural}/Commands/{Entity}Repository.*.cs` | `CncApp.Infrastructure.Repositories` |
| Infrastructure Configuration | `Infrastructure/Persistence/Configurations/{Entity}Configuration.cs` | `CncApp.Infrastructure.Persistence.Configurations` |
| Application Tests | `Application.Tests/Services/{EntityPlural}/{Entity}Tests.cs` | `CncApp.Application.Tests.Services.{EntityPlural}` |
| Application Tests (Commands/Queries) | `Application.Tests/Services/{EntityPlural}/Commands/{Entity}Tests.*.cs` | `CncApp.Application.Tests.Services.{EntityPlural}` |
| Domain Tests | `Domain.Tests/Entities/{Entity}Tests.cs` | `CncApp.Domain.Tests.Entities` |

**File Header Pattern:**
```csharp
using System;
using AutoMapper;

namespace CncApp.Application.Services.Machines;

public partial class MachineService
{
    // ...
}
```

---

## 6. Test Structure Rules

### Prefactored Machine Test Result

**Application Tests Structure:**
- Base file (`{Entity}Tests.cs`) contains shared setup: mocks, service initialization, constructor
- Each method gets its own file in Commands/ or Queries/ folder
- All test files are partial classes of `{Entity}Tests`
- Test files mirror the service method structure exactly

**Example Structure:**
```
Application.Tests/Services/Machines/
├── MachineTests.cs                    (shared mocks, service setup)
├── Commands/
│   ├── MachineTests.Create.cs         (CreateAsync tests only)
│   └── MachineTests.Inactivate.cs      (InactivateAsync tests only)
└── Queries/
    ├── MachineTests.Get.cs             (GetAsync tests only)
    ├── MachineTests.ListActive.cs      (ListActiveAsync tests only)
    └── MachineTests.ListAll.cs         (ListAllAsync tests only)
```

**Domain Tests Structure:**
- Single file (`{Entity}Tests.cs`) - no partial classes
- Uses `#region` to organize test groups:
  - `#region Constructor Tests`
  - `#region Property Setter Tests`
  - `#region Method Tests`
- Tests domain invariants only (no database, no mocks)

**Test Boundaries:**
- **Domain Tests:** Test entity invariants, validation, domain methods. NO database, NO mocks.
- **Application Tests:** Test service workflows, repository interaction, DTO mapping. Use mocks for dependencies.

---

## 7. Checklist: When Creating a New Slice/Table

Use this checklist when implementing a new slice. Replace `{Entity}` with your entity name (singular) and `{EntityPlural}` with the plural form.

### API Layer
- [ ] `Api/Controllers/{EntityPlural}Controller.cs`
  - Class: `{EntityPlural}Controller`
  - Namespace: `CncApp.Api.Controllers`
  - Route: `[Route("api/[controller]")]`
  - Injects: `{Entity}Service` (concrete)

### Application Layer - DTOs
- [ ] `Application/Dtos/{EntityPlural}/{Entity}Dto.cs`
  - Namespace: `CncApp.Application.Dtos.{EntityPlural}`
- [ ] `Application/Dtos/{EntityPlural}/Create{Entity}RequestDto.cs`
  - Namespace: `CncApp.Application.Dtos.{EntityPlural}`

### Application Layer - Services
- [ ] `Application/Services/{EntityPlural}/{Entity}Service.cs`
  - Partial class, constructor + dependencies
  - Namespace: `CncApp.Application.Services.{EntityPlural}`
- [ ] `Application/Services/{EntityPlural}/Commands/{Entity}Service.Create.cs`
  - Partial class, `CreateAsync` method
  - Namespace: `CncApp.Application.Services.{EntityPlural}`
- [ ] `Application/Services/{EntityPlural}/Commands/{Entity}Service.Inactivate.cs`
  - Partial class, `InactivateAsync` method
  - Namespace: `CncApp.Application.Services.{EntityPlural}`
- [ ] `Application/Services/{EntityPlural}/Queries/{Entity}Service.Get.cs`
  - Partial class, `GetAsync` method
  - Namespace: `CncApp.Application.Services.{EntityPlural}`
- [ ] `Application/Services/{EntityPlural}/Queries/{Entity}Service.ListActive.cs`
  - Partial class, `ListActiveAsync` method
  - Namespace: `CncApp.Application.Services.{EntityPlural}`
- [ ] `Application/Services/{EntityPlural}/Queries/{Entity}Service.ListAll.cs`
  - Partial class, `ListAllAsync` method
  - Namespace: `CncApp.Application.Services.{EntityPlural}`

### Application Layer - Mapping
- [ ] `Application/Mapping/{Entity}Profile.cs`
  - Class: `{Entity}Profile` : `Profile`
  - Namespace: `CncApp.Application.Mapping`
  - Maps: `{Entity}` → `{Entity}Dto`, `Create{Entity}RequestDto` → `{Entity}`

### Application Layer - Interfaces
- [ ] `Application/Interfaces/Repositories/I{Entity}Repository.cs`
  - Interface: `I{Entity}Repository`
  - Namespace: `CncApp.Application.Interfaces.Repositories`
  - Methods: `GetByIdAsync`, `ListActiveAsync`, `ListAllAsync`, `AddAsync`, `InactivateAsync`, `SaveChangesAsync`

### Domain Layer
- [ ] `Domain/Entities/{Entity}.cs`
  - Class: `{Entity}` : `AuditableEntityBase`
  - Namespace: `CncApp.Domain.Entities`
  - Private constructor for EF Core
  - Public constructor with validation
  - Domain methods (e.g., `Inactivate()`)

### Infrastructure Layer - Repositories
- [ ] `Infrastructure/Repositories/{EntityPlural}/{Entity}Repository.cs`
  - Partial class, implements `I{Entity}Repository`
  - Constructor + `_context` field
  - Namespace: `CncApp.Infrastructure.Repositories`
- [ ] `Infrastructure/Repositories/{EntityPlural}/Commands/{Entity}Repository.Add.cs`
  - Partial class, `AddAsync` method
  - Namespace: `CncApp.Infrastructure.Repositories`
- [ ] `Infrastructure/Repositories/{EntityPlural}/Commands/{Entity}Repository.Inactivate.cs`
  - Partial class, `InactivateAsync` method
  - Namespace: `CncApp.Infrastructure.Repositories`
- [ ] `Infrastructure/Repositories/{EntityPlural}/Commands/{Entity}Repository.SaveChanges.cs`
  - Partial class, `SaveChangesAsync` method
  - Namespace: `CncApp.Infrastructure.Repositories`
- [ ] `Infrastructure/Repositories/{EntityPlural}/Queries/{Entity}Repository.GetById.cs`
  - Partial class, `GetByIdAsync` method
  - Namespace: `CncApp.Infrastructure.Repositories`
- [ ] `Infrastructure/Repositories/{EntityPlural}/Queries/{Entity}Repository.ListActive.cs`
  - Partial class, `ListActiveAsync` method
  - Namespace: `CncApp.Infrastructure.Repositories`
- [ ] `Infrastructure/Repositories/{EntityPlural}/Queries/{Entity}Repository.ListAll.cs`
  - Partial class, `ListAllAsync` method
  - Namespace: `CncApp.Infrastructure.Repositories`

### Infrastructure Layer - Persistence
- [ ] `Infrastructure/Persistence/Configurations/{Entity}Configuration.cs`
  - Class: `{Entity}Configuration` : `IEntityTypeConfiguration<{Entity}>`
  - Namespace: `CncApp.Infrastructure.Persistence.Configurations`
  - Configures: primary key, required properties, max lengths, relationships

### Infrastructure Layer - DbContext
- [ ] Update `Infrastructure/Persistence/AppDbContext.cs`
  - Add: `public DbSet<{Entity}> {EntityPlural} { get; set; }`

### Infrastructure Layer - Dependency Injection
- [ ] Update `Infrastructure/DependencyInjection.cs`
  - Add: `services.AddScoped<I{Entity}Repository, {Entity}Repository>();`

### Application Layer - Dependency Injection
- [ ] Update `Application/DependencyInjection.cs`
  - Add: `services.AddScoped<{Entity}Service>();`

### Application Tests
- [ ] `Application.Tests/Services/{EntityPlural}/{Entity}Tests.cs`
  - Partial class, shared mocks and setup
  - Namespace: `CncApp.Application.Tests.Services.{EntityPlural}`
- [ ] `Application.Tests/Services/{EntityPlural}/Commands/{Entity}Tests.Create.cs`
  - Partial class, `CreateAsync` tests
  - Namespace: `CncApp.Application.Tests.Services.{EntityPlural}`
- [ ] `Application.Tests/Services/{EntityPlural}/Commands/{Entity}Tests.Inactivate.cs`
  - Partial class, `InactivateAsync` tests
  - Namespace: `CncApp.Application.Tests.Services.{EntityPlural}`
- [ ] `Application.Tests/Services/{EntityPlural}/Queries/{Entity}Tests.Get.cs`
  - Partial class, `GetAsync` tests
  - Namespace: `CncApp.Application.Tests.Services.{EntityPlural}`
- [ ] `Application.Tests/Services/{EntityPlural}/Queries/{Entity}Tests.ListActive.cs`
  - Partial class, `ListActiveAsync` tests
  - Namespace: `CncApp.Application.Tests.Services.{EntityPlural}`
- [ ] `Application.Tests/Services/{EntityPlural}/Queries/{Entity}Tests.ListAll.cs`
  - Partial class, `ListAllAsync` tests
  - Namespace: `CncApp.Application.Tests.Services.{EntityPlural}`

### Domain Tests
- [ ] `Domain.Tests/Entities/{Entity}Tests.cs`
  - Class: `{Entity}Tests` (not partial)
  - Namespace: `CncApp.Domain.Tests.Entities`
  - Uses `#region` for: Constructor Tests, Property Setter Tests, Method Tests

### Database Migration
- [ ] Create EF Core migration: `dotnet ef migrations add Add{Entity}Table`
- [ ] Migration file in `Infrastructure/Migrations/`

---

## 8. "Don'ts" (Critical Rules)

### ❌ Do NOT Refactor Existing Code
- When creating a new slice, do NOT refactor or "improve" existing Machines code
- Do NOT change existing file structures
- Do NOT rename existing files or folders

### ❌ Do NOT Deviate from Machines Pattern
- Mirror Machines slice structure exactly
- Use the same naming conventions
- Follow the same partial class patterns
- Use the same namespace patterns

### ❌ Do NOT Include Commands/Queries in Namespace
- All partial class files must share the slice root namespace
- Do NOT use `namespace ...Commands;` or `namespace ...Queries;`
- Use `namespace CncApp.Application.Services.{EntityPlural};` for all service partials

### ❌ Do NOT Mix Test Methods
- Each test file should contain tests for exactly one method
- Do NOT put multiple method tests in one file
- Base test file should contain only shared setup, no test methods

### ❌ Do NOT Create Per-Entity Subfolders in Shared Folders
- `Domain/Entities/` - no subfolders, all entities in root
- `Application/Mapping/` - no subfolders, all profiles in root
- `Infrastructure/Persistence/Configurations/` - no subfolders, all configs in root
- `Domain.Tests/Entities/` - no subfolders, all tests in root

### ❌ Do NOT Use Interfaces for Services
- Services are registered as concrete classes: `services.AddScoped<{Entity}Service>();`
- Controllers inject concrete service classes, not interfaces
- Only repositories use interfaces (`I{Entity}Repository`)

### ❌ Do NOT Call SaveChanges in Repository Commands
- Repository commands (Add, Inactivate) do NOT call `SaveChangesAsync`
- Service layer calls `SaveChangesAsync` after repository operations
- `SaveChangesAsync` is a separate repository method

### ❌ Do NOT Bypass Domain Logic in Repositories
- Repositories should call domain methods (e.g., `machine.Inactivate(userId)`) when available
- Do NOT directly set domain properties (e.g., `InactivatedDateTime`) in repositories
- Let domain entities enforce invariants

---

## Summary

**Golden Rule:** When creating a new slice, open the Machines slice files and mirror them exactly. Replace `Machine`/`Machines` with your entity name, but keep the structure, naming, and patterns identical.

**Key Patterns:**
- Plural folder names (`Machines/`, `Jobs/`)
- Singular class names (`MachineService`, `JobService`)
- Partial classes for Services, Repositories, and Application Tests
- Shared namespaces for all partials (no `.Commands`/`.Queries` in namespace)
- One method per file in Commands/Queries folders
- Commands = write operations, Queries = read operations

**Reference Files:**
- `docs/MachinesStructure.md` - Complete file listing
- `docs/MachinesConventions.md` - Detailed conventions
- `backend/CncApp/CncApp.*/` - Actual Machines implementation

