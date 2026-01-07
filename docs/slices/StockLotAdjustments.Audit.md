# StockLotAdjustments Slice Audit

## 1. Purpose
 
This audit analyzes the **current scaffolded StockLotAdjustments slice** (folders + empty/placeholder files) and compares it against the canonical **Machines slice pattern** and **SliceMap rules** to identify:
- What's correct and matches the pattern
- What's wrong or missing
- What needs to be renamed/replaced before implementing real functionality

**Reference Documents:**
- `/docs/SliceMap.md` - Canonical structure rules
- `/docs/MachinesStructure.md` - Complete Machines file listing
- `/docs/MachinesConventions.md` - Detailed conventions
- `/docs/Final-scaffold-corrections/Pre_factored_Namespace_correction.md` - Namespace rules

---

## 2. Snapshot: What Exists Today

### API Layer
- ✅ `backend/CncApp/CncApp.Api/Controllers/StockLotAdjustmentsController.cs`
  - Class: `StockLotAdjustmentsController`
  - Namespace: `CncApp.Api.Controllers`
  - Status: Stub with constructor only, no actions (TODO comment present)
  - Injects: `StockLotAdjustmentService` (concrete) ✅

### Application Layer - DTOs
- ✅ `backend/CncApp/CncApp.Application/Dtos/StockLotAdjustments/StockLotAdjustmentResultDto.cs`
  - Class: `StockLotAdjustmentResultDto`
  - Namespace: `CncApp.Application.Dtos.StockLotAdjustments`
  - Status: Stub with `Id` property, TODO for other properties
  - ⚠️ **Note**: Name is `StockLotAdjustmentResultDto` but should be `StockLotAdjustmentDto` to match Machines pattern
- ✅ `backend/CncApp/CncApp.Application/Dtos/StockLotAdjustments/CreateStockLotAdjustmentRequestDto.cs`
  - Class: `CreateStockLotAdjustmentRequestDto`
  - Namespace: `CncApp.Application.Dtos.StockLotAdjustments`
  - Status: Stub with TODO for properties

### Application Layer - Services
- ✅ `backend/CncApp/CncApp.Application/Services/StockLotAdjustments/StockLotAdjustmentService.cs`
  - Class: `StockLotAdjustmentService` (partial)
  - Namespace: `CncApp.Application.Services.StockLotAdjustments`
  - Status: Base file with constructor + dependencies (`IStockLotAdjustmentRepository`, `IMapper`)
- ✅ `backend/CncApp/CncApp.Application/Services/StockLotAdjustments/Commands/StockLotAdjustmentService.PlaceholderCommand.cs`
  - Class: `StockLotAdjustmentService` (partial)
  - Namespace: `CncApp.Application.Services.StockLotAdjustments` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Application/Services/StockLotAdjustments/Queries/StockLotAdjustmentService.PlaceholderQuery.cs`
  - Class: `StockLotAdjustmentService` (partial)
  - Namespace: `CncApp.Application.Services.StockLotAdjustments` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Application Layer - Mapping
- ✅ `backend/CncApp/CncApp.Application/Mapping/StockLotAdjustmentProfile.cs`
  - Class: `StockLotAdjustmentProfile` : `Profile`
  - Namespace: `CncApp.Application.Mapping`
  - Status: Stub with empty constructor, TODO for CreateMap calls

### Application Layer - Interfaces
- ✅ `backend/CncApp/CncApp.Application/Interfaces/Repositories/IStockLotAdjustmentRepository.cs`
  - Interface: `IStockLotAdjustmentRepository`
  - Namespace: `CncApp.Application.Interfaces.Repositories`
  - Status: Stub with TODO for method signatures

### Domain Layer
- ✅ `backend/CncApp/CncApp.Domain/Entities/StockLotAdjustment.cs`
  - Class: `StockLotAdjustment` : `AuditableEntityBase`
  - Namespace: `CncApp.Domain.Entities`
  - Status: Entity exists with properties, but **lacks domain validation** (no private constructor, no public constructor with validation, no domain methods like `Inactivate()`)
  - Properties: `StockLotId` (required), `JobId` (nullable), `DeltaBars` (required), `Reason` (required, enum), `Notes` (nullable, max 2000), `StockLot` (navigation)
- ✅ `backend/CncApp/CncApp.Domain/Enums/StockLotAdjustmentReasonEnum.cs`
  - Enum: `StockLotAdjustmentReasonEnum` : `byte`
  - Values: `Received = 1`, `Consumed = 2`, `Adjusted = 3`, `Scrap = 4`, `Return = 5`
  - Status: ✅ Complete

### Infrastructure Layer - Repositories
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/StockLotAdjustments/StockLotAdjustmentRepository.cs`
  - Class: `StockLotAdjustmentRepository` (partial) : `IStockLotAdjustmentRepository`
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct)
  - Status: Base file with constructor + `_context` field
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/StockLotAdjustments/Commands/StockLotAdjustmentRepository.PlaceholderCommand.cs`
  - Class: `StockLotAdjustmentRepository` (partial)
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/StockLotAdjustments/Queries/StockLotAdjustmentRepository.PlaceholderQuery.cs`
  - Class: `StockLotAdjustmentRepository` (partial)
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Infrastructure Layer - Persistence
- ✅ `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/StockLotAdjustmentConfiguration.cs`
  - Class: `StockLotAdjustmentConfiguration` : `IEntityTypeConfiguration<StockLotAdjustment>`
  - Namespace: `CncApp.Infrastructure.Persistence.Configurations`
  - Status: **Fully configured** with primary key, properties (StockLotId, JobId, DeltaBars, Reason, Notes), relationships (StockLot), max lengths (Notes: 2000), index on StockLotId
- ✅ `backend/CncApp/CncApp.Infrastructure/Persistence/AppDbContext.cs`
  - Contains: `public DbSet<StockLotAdjustment> StockLotAdjustments => Set<StockLotAdjustment>();`
  - Status: ✅ Registered

### Application Tests
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/StockLotAdjustments/StockLotAdjustmentTests.cs`
  - Class: `StockLotAdjustmentTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.StockLotAdjustments`
  - Status: Stub with TODO comment, **missing shared setup** (mocks, service initialization)
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/StockLotAdjustments/Commands/StockLotAdjustmentTests.PlaceholderCommand.cs`
  - Class: `StockLotAdjustmentTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.StockLotAdjustments` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/StockLotAdjustments/Queries/StockLotAdjustmentTests.PlaceholderQuery.cs`
  - Class: `StockLotAdjustmentTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.StockLotAdjustments` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Domain Tests
- ✅ `backend/CncApp/CncApp.Domain.Tests/Entities/StockLotAdjustmentTests.cs`
  - Class: `StockLotAdjustmentTests` (not partial)
  - Namespace: `CncApp.Domain.Tests.Entities`
  - Status: Stub with `#region` structure (Constructor Tests, Property Setter Tests, Method Tests), all TODOs

---

## 3. Conformance Check vs Machines

### ✅ Commands/Queries Folder Presence
- **PASS**: `Application/Services/StockLotAdjustments/Commands/` exists
- **PASS**: `Application/Services/StockLotAdjustments/Queries/` exists
- **PASS**: `Infrastructure/Repositories/StockLotAdjustments/Commands/` exists
- **PASS**: `Infrastructure/Repositories/StockLotAdjustments/Queries/` exists
- **PASS**: `Application.Tests/Services/StockLotAdjustments/Commands/` exists
- **PASS**: `Application.Tests/Services/StockLotAdjustments/Queries/` exists

### ✅ Partial Class Split Correctness
- **PASS**: `StockLotAdjustmentService.cs` is partial (base file)
- **PASS**: `StockLotAdjustmentService.PlaceholderCommand.cs` is partial
- **PASS**: `StockLotAdjustmentService.PlaceholderQuery.cs` is partial
- **PASS**: `StockLotAdjustmentRepository.cs` is partial (base file)
- **PASS**: `StockLotAdjustmentRepository.PlaceholderCommand.cs` is partial
- **PASS**: `StockLotAdjustmentRepository.PlaceholderQuery.cs` is partial
- **PASS**: `StockLotAdjustmentTests.cs` is partial (base file)
- **PASS**: `StockLotAdjustmentTests.PlaceholderCommand.cs` is partial
- **PASS**: `StockLotAdjustmentTests.PlaceholderQuery.cs` is partial

### ✅ Namespace Correctness (Prefactored Namespace Correction Applied)
- **PASS**: All `StockLotAdjustmentService` partials use `namespace CncApp.Application.Services.StockLotAdjustments;` (no `.Commands`/`.Queries`)
- **PASS**: All `StockLotAdjustmentRepository` partials use `namespace CncApp.Infrastructure.Repositories;` (no `.Commands`/`.Queries`)
- **PASS**: All `StockLotAdjustmentTests` partials use `namespace CncApp.Application.Tests.Services.StockLotAdjustments;` (no `.Commands`/`.Queries`)

### ⚠️ Test Structure Correctness
- **PASS**: Test folder structure mirrors `Application/Services/StockLotAdjustments/` structure
- **PASS**: Test files are partial classes
- **FAIL**: `StockLotAdjustmentTests.cs` base file is missing shared setup (mocks, service initialization)
  - **Expected**: Should contain `Mock<IStockLotAdjustmentRepository>`, `Mock<IMapper>`, `StockLotAdjustmentService` initialization like `MachineTests.cs`
  - **Current**: Only contains TODO comment

### ⚠️ Domain Entity Correctness
- **FAIL**: `StockLotAdjustment.cs` lacks domain validation patterns
  - **Missing**: Private constructor for EF Core materialization
  - **Missing**: Public constructor with validation
  - **Missing**: Domain methods (e.g., `Inactivate()`)
  - **Missing**: Property setter validation using `Guard` class
  - **Current**: Simple POCO with public setters (no domain invariants)

### ⚠️ DTO Naming Convention
- **FAIL**: `StockLotAdjustmentResultDto` should be renamed to `StockLotAdjustmentDto` to match Machines pattern
  - **Expected**: `StockLotAdjustmentDto` (mirrors `MachineDto`)
  - **Current**: `StockLotAdjustmentResultDto`

### ❌ Dependency Injection Registration
- **FAIL**: `StockLotAdjustmentService` not registered in `Application/DependencyInjection.cs`
  - **Expected**: `services.AddScoped<StockLotAdjustmentService>();`
- **FAIL**: `IStockLotAdjustmentRepository` not registered in `Infrastructure/DependencyInjection.cs`
  - **Expected**: `services.AddScoped<IStockLotAdjustmentRepository, StockLotAdjustmentRepository>();`

---

## 4. Placeholder Inventory

| File Path | Placeholder Type | Why It's Placeholder | What It Should Become |
|-----------|------------------|----------------------|----------------------|
| `Application/Services/StockLotAdjustments/Commands/StockLotAdjustmentService.PlaceholderCommand.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `StockLotAdjustmentService.Create.cs`, `StockLotAdjustmentService.Inactivate.cs` (one file per method) |
| `Application/Services/StockLotAdjustments/Queries/StockLotAdjustmentService.PlaceholderQuery.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `StockLotAdjustmentService.Get.cs`, `StockLotAdjustmentService.ListActive.cs`, `StockLotAdjustmentService.ListAll.cs` (one file per method) |
| `Infrastructure/Repositories/StockLotAdjustments/Commands/StockLotAdjustmentRepository.PlaceholderCommand.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `StockLotAdjustmentRepository.Add.cs`, `StockLotAdjustmentRepository.Inactivate.cs`, `StockLotAdjustmentRepository.SaveChanges.cs` (one file per method) |
| `Infrastructure/Repositories/StockLotAdjustments/Queries/StockLotAdjustmentRepository.PlaceholderQuery.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `StockLotAdjustmentRepository.GetById.cs`, `StockLotAdjustmentRepository.ListActive.cs`, `StockLotAdjustmentRepository.ListAll.cs` (one file per method) |
| `Application.Tests/Services/StockLotAdjustments/Commands/StockLotAdjustmentTests.PlaceholderCommand.cs` | Test Placeholder | Empty partial class with TODO | **DELETE** and replace with: `StockLotAdjustmentTests.Create.cs`, `StockLotAdjustmentTests.Inactivate.cs` (one file per method test) |
| `Application.Tests/Services/StockLotAdjustments/Queries/StockLotAdjustmentTests.PlaceholderQuery.cs` | Test Placeholder | Empty partial class with TODO | **DELETE** and replace with: `StockLotAdjustmentTests.Get.cs`, `StockLotAdjustmentTests.ListActive.cs`, `StockLotAdjustmentTests.ListAll.cs` (one file per method test) |

**Placeholder Files That Should Be Deleted:**
All 6 placeholder files listed above must be **deleted** when implementing real functionality. They are temporary scaffolding files that prevent empty folders but must be replaced with actual method-specific files.

**Reference:** Machines slice has NO placeholder files - it has real method files:
- `MachineService.Create.cs`, `MachineService.Inactivate.cs`
- `MachineService.Get.cs`, `MachineService.ListActive.cs`, `MachineService.ListAll.cs`
- `MachineRepository.Add.cs`, `MachineRepository.Inactivate.cs`, `MachineRepository.SaveChanges.cs`
- `MachineRepository.GetById.cs`, `MachineRepository.ListActive.cs`, `MachineRepository.ListAll.cs`
- `MachineTests.Create.cs`, `MachineTests.Inactivate.cs`
- `MachineTests.Get.cs`, `MachineTests.ListActive.cs`, `MachineTests.ListAll.cs`

---

## 5. Required Renames / Deletes / Replacements

### API Layer
**No changes needed** - `StockLotAdjustmentsController.cs` is correctly structured as a stub. Actions need to be added (mirror `MachinesController.cs`).

### Application Layer - DTOs
**RENAME and ENHANCE:**
1. **RENAME**: `StockLotAdjustmentResultDto.cs` → `StockLotAdjustmentDto.cs`
   - Rename class: `StockLotAdjustmentResultDto` → `StockLotAdjustmentDto`
2. **UPDATE**: `StockLotAdjustmentDto.cs` should include properties based on `StockLotAdjustmentConfiguration.cs` and `StockLotAdjustment.cs` entity:
   - `Id`, `StockLotId`, `JobId` (nullable), `DeltaBars`, `Reason` (enum), `Notes` (nullable), audit fields (if exposed)
3. **UPDATE**: `CreateStockLotAdjustmentRequestDto.cs` should include:
   - `StockLotId`, `JobId` (nullable), `DeltaBars`, `Reason` (enum), `Notes` (nullable)
   - Add DataAnnotations validation to mirror EF configuration

### Application Layer - Services
**REPLACE (delete placeholders, create method files):**
1. **DELETE**: `Application/Services/StockLotAdjustments/Commands/StockLotAdjustmentService.PlaceholderCommand.cs`
2. **CREATE**: `Application/Services/StockLotAdjustments/Commands/StockLotAdjustmentService.Create.cs` (mirror `MachineService.Create.cs`)
3. **CREATE**: `Application/Services/StockLotAdjustments/Commands/StockLotAdjustmentService.Inactivate.cs` (mirror `MachineService.Inactivate.cs`)
4. **DELETE**: `Application/Services/StockLotAdjustments/Queries/StockLotAdjustmentService.PlaceholderQuery.cs`
5. **CREATE**: `Application/Services/StockLotAdjustments/Queries/StockLotAdjustmentService.Get.cs` (mirror `MachineService.Get.cs`)
6. **CREATE**: `Application/Services/StockLotAdjustments/Queries/StockLotAdjustmentService.ListActive.cs` (mirror `MachineService.ListActive.cs`)
7. **CREATE**: `Application/Services/StockLotAdjustments/Queries/StockLotAdjustmentService.ListAll.cs` (mirror `MachineService.ListAll.cs`)

### Application Layer - Mapping
**No changes needed** - `StockLotAdjustmentProfile.cs` is correctly structured as a stub. CreateMap calls need to be added when implementing:
- `CreateMap<StockLotAdjustment, StockLotAdjustmentDto>();`
- `CreateMap<CreateStockLotAdjustmentRequestDto, StockLotAdjustment>();`

### Application Layer - Interfaces
**No changes needed** - `IStockLotAdjustmentRepository.cs` is correctly structured as a stub. Method signatures need to be added (mirror `IMachineRepository.cs`):
- `Task<StockLotAdjustment?> GetByIdAsync(int id, CancellationToken ct = default);`
- `Task<List<StockLotAdjustment>> ListActiveAsync(CancellationToken ct = default);`
- `Task<List<StockLotAdjustment>> ListAllAsync(CancellationToken ct = default);`
- `Task AddAsync(StockLotAdjustment stockLotAdjustment, CancellationToken ct = default);`
- `Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);`
- `Task SaveChangesAsync(CancellationToken ct = default);`

### Domain Layer
**ENHANCE (add domain validation):**
1. **UPDATE**: `Domain/Entities/StockLotAdjustment.cs`
   - Add private constructor for EF Core materialization
   - Add public constructor with validation (using `Guard` class)
   - Add property setter validation:
     - `StockLotId`: required (must be > 0)
     - `DeltaBars`: required (can be negative for consumption, positive for receipt)
     - `Reason`: required (enum value)
     - `Notes`: optional, max length 2000 (using `Guard.AgainstMaxLength`)
   - Add domain methods (e.g., `Inactivate()` method)
   - Mirror `Machine.cs` domain validation patterns

### Infrastructure Layer - Repositories
**REPLACE (delete placeholders, create method files):**
1. **DELETE**: `Infrastructure/Repositories/StockLotAdjustments/Commands/StockLotAdjustmentRepository.PlaceholderCommand.cs`
2. **CREATE**: `Infrastructure/Repositories/StockLotAdjustments/Commands/StockLotAdjustmentRepository.Add.cs` (mirror `MachineRepository.Add.cs`)
3. **CREATE**: `Infrastructure/Repositories/StockLotAdjustments/Commands/StockLotAdjustmentRepository.Inactivate.cs` (mirror `MachineRepository.Inactivate.cs`)
4. **CREATE**: `Infrastructure/Repositories/StockLotAdjustments/Commands/StockLotAdjustmentRepository.SaveChanges.cs` (mirror `MachineRepository.SaveChanges.cs`)
5. **DELETE**: `Infrastructure/Repositories/StockLotAdjustments/Queries/StockLotAdjustmentRepository.PlaceholderQuery.cs`
6. **CREATE**: `Infrastructure/Repositories/StockLotAdjustments/Queries/StockLotAdjustmentRepository.GetById.cs` (mirror `MachineRepository.GetById.cs`)
7. **CREATE**: `Infrastructure/Repositories/StockLotAdjustments/Queries/StockLotAdjustmentRepository.ListActive.cs` (mirror `MachineRepository.ListActive.cs`)
8. **CREATE**: `Infrastructure/Repositories/StockLotAdjustments/Queries/StockLotAdjustmentRepository.ListAll.cs` (mirror `MachineRepository.ListAll.cs`)

### Infrastructure Layer - Persistence
**No changes needed** - `StockLotAdjustmentConfiguration.cs` is fully configured and correct.

### Infrastructure Layer - Dependency Injection
**ADD (register repository):**
1. **UPDATE**: `Infrastructure/DependencyInjection.cs`
   - Add: `services.AddScoped<IStockLotAdjustmentRepository, StockLotAdjustmentRepository>();`

### Application Layer - Dependency Injection
**ADD (register service):**
1. **UPDATE**: `Application/DependencyInjection.cs`
   - Add: `services.AddScoped<StockLotAdjustmentService>();`

### Application Tests
**REPLACE (delete placeholders, create method test files) AND FIX (add shared setup):**
1. **UPDATE**: `Application.Tests/Services/StockLotAdjustments/StockLotAdjustmentTests.cs`
   - Add shared setup: `Mock<IStockLotAdjustmentRepository>`, `Mock<IMapper>`, `StockLotAdjustmentService` initialization (mirror `MachineTests.cs`)
2. **DELETE**: `Application.Tests/Services/StockLotAdjustments/Commands/StockLotAdjustmentTests.PlaceholderCommand.cs`
3. **CREATE**: `Application.Tests/Services/StockLotAdjustments/Commands/StockLotAdjustmentTests.Create.cs` (mirror `MachineTests.Create.cs`)
4. **CREATE**: `Application.Tests/Services/StockLotAdjustments/Commands/StockLotAdjustmentTests.Inactivate.cs` (mirror `MachineTests.Inactivate.cs`)
5. **DELETE**: `Application.Tests/Services/StockLotAdjustments/Queries/StockLotAdjustmentTests.PlaceholderQuery.cs`
6. **CREATE**: `Application.Tests/Services/StockLotAdjustments/Queries/StockLotAdjustmentTests.Get.cs` (mirror `MachineTests.Get.cs`)
7. **CREATE**: `Application.Tests/Services/StockLotAdjustments/Queries/StockLotAdjustmentTests.ListActive.cs` (mirror `MachineTests.ListActive.cs`)
8. **CREATE**: `Application.Tests/Services/StockLotAdjustments/Queries/StockLotAdjustmentTests.ListAll.cs` (mirror `MachineTests.ListAll.cs`)

### Domain Tests
**No changes needed** - `StockLotAdjustmentTests.cs` is correctly structured with `#region` organization. Tests need to be implemented.

---

## 6. StockLotAdjustments Slice "Ready-to-Implement?" Verdict

### ❌ **NOT READY** - Minimum Cleanup Required

**Critical Blockers:**
1. **Missing Dependency Injection Registrations** - Service and repository not registered
2. **Missing Domain Validation** - `StockLotAdjustment.cs` lacks domain invariants (no private constructor, no validation, no domain methods)
3. **Placeholder Files Present** - 6 placeholder files must be deleted and replaced with method-specific files
4. **Missing Test Setup** - `StockLotAdjustmentTests.cs` base file lacks shared mocks and service initialization
5. **DTO Naming Mismatch** - `StockLotAdjustmentResultDto` should be `StockLotAdjustmentDto` to match Machines pattern

**Minimum Cleanup Before Implementation:**
1. ✅ **Fix Dependency Injection** (5 minutes)
   - Add `StockLotAdjustmentService` registration in `Application/DependencyInjection.cs`
   - Add `IStockLotAdjustmentRepository` registration in `Infrastructure/DependencyInjection.cs`
2. ✅ **Rename DTO** (2 minutes)
   - Rename `StockLotAdjustmentResultDto` → `StockLotAdjustmentDto`
3. ✅ **Enhance Domain Entity** (30 minutes)
   - Add private constructor for EF Core
   - Add public constructor with validation
   - Add property setter validation for `StockLotId`, `DeltaBars`, `Reason`, `Notes`
   - Add `Inactivate()` domain method
4. ✅ **Add Test Base Setup** (10 minutes)
   - Add mocks and service initialization to `StockLotAdjustmentTests.cs`
5. ⚠️ **Replace Placeholders** (can be done incrementally during implementation)
   - Delete placeholder files as you create real method files
   - Create method files one at a time as you implement functionality

**Recommended Implementation Order:**
1. Fix Dependency Injection registrations
2. Rename DTO to match naming convention
3. Enhance `StockLotAdjustment.cs` domain entity with validation
4. Add test base setup to `StockLotAdjustmentTests.cs`
5. Implement repository methods (delete placeholders, create method files)
6. Implement service methods (delete placeholders, create method files)
7. Implement controller actions
8. Implement tests (delete placeholders, create method test files)

---

## 7. Golden Reference Pointers

When implementing StockLotAdjustments slice functionality, mirror these **Machines slice files exactly**:

### Application Services
- **Base**: `backend/CncApp/CncApp.Application/Services/Machines/MachineService.cs`
- **Commands**: 
  - `backend/CncApp/CncApp.Application/Services/Machines/Commands/MachineService.Create.cs`
  - `backend/CncApp/CncApp.Application/Services/Machines/Commands/MachineService.Inactivate.cs`
- **Queries**:
  - `backend/CncApp/CncApp.Application/Services/Machines/Queries/MachineService.Get.cs`
  - `backend/CncApp/CncApp.Application/Services/Machines/Queries/MachineService.ListActive.cs`
  - `backend/CncApp/CncApp.Application/Services/Machines/Queries/MachineService.ListAll.cs`

### Infrastructure Repositories
- **Base**: `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/MachineRepository.cs`
- **Commands**:
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Commands/MachineRepository.Add.cs`
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Commands/MachineRepository.Inactivate.cs`
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Commands/MachineRepository.SaveChanges.cs`
- **Queries**:
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Queries/MachineRepository.GetById.cs`
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Queries/MachineRepository.ListActive.cs`
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Queries/MachineRepository.ListAll.cs`

### Domain Entity
- **Reference**: `backend/CncApp/CncApp.Domain/Entities/Machine.cs`
  - Shows: private constructor, public constructor with validation, property setter validation, `Inactivate()` method

### Application Tests
- **Base**: `backend/CncApp/CncApp.Application.Tests/Services/Machines/MachineTests.cs`
- **Commands**:
  - `backend/CncApp/CncApp.Application.Tests/Services/Machines/Commands/MachineTests.Create.cs`
  - `backend/CncApp/CncApp.Application.Tests/Services/Machines/Commands/MachineTests.Inactivate.cs`
- **Queries**:
  - `backend/CncApp/CncApp.Application.Tests/Services/Machines/Queries/MachineTests.Get.cs`
  - `backend/CncApp/CncApp.Application.Tests/Services/Machines/Queries/MachineTests.ListActive.cs`
  - `backend/CncApp/CncApp.Application.Tests/Services/Machines/Queries/MachineTests.ListAll.cs`

### API Controller
- **Reference**: `backend/CncApp/CncApp.Api/Controllers/MachinesController.cs`
  - Shows: controller structure, action methods, authorization, response types

### Application Interfaces
- **Reference**: `backend/CncApp/CncApp.Application/Interfaces/Repositories/IMachineRepository.cs`
  - Shows: interface method signatures

### Application Mapping
- **Reference**: `backend/CncApp/CncApp.Application/Mapping/MachineProfile.cs`
  - Shows: CreateMap patterns

### Application DTOs
- **Reference**: `backend/CncApp/CncApp.Application/Dtos/Machines/MachineDto.cs`
  - Shows: DTO structure and naming convention
- **Reference**: `backend/CncApp/CncApp.Application/Dtos/Machines/CreateMachineRequestDto.cs`
  - Shows: Request DTO structure

### Domain Tests
- **Reference**: `backend/CncApp/CncApp.Domain.Tests/Entities/MachineTests.cs`
  - Shows: `#region` organization, test patterns for domain invariants

---

## Summary

**What's Right:**
- ✅ Folder structure matches Machines pattern exactly
- ✅ All namespaces are correct (prefactored namespace correction applied)
- ✅ Partial class structure is correct
- ✅ Commands/Queries folder separation is correct
- ✅ EF Core configuration is complete
- ✅ DbContext registration exists
- ✅ Enum is complete

**What's Wrong:**
- ❌ Missing Dependency Injection registrations
- ❌ Domain entity lacks validation patterns
- ❌ Test base file missing shared setup
- ❌ DTO naming doesn't match convention (`StockLotAdjustmentResultDto` should be `StockLotAdjustmentDto`)
- ❌ 6 placeholder files must be replaced with method-specific files

**What Needs to Happen:**
1. Fix DI registrations (critical blocker)
2. Rename DTO to match naming convention (critical blocker)
3. Enhance domain entity with validation (critical blocker)
4. Add test base setup (critical blocker)
5. Replace placeholders with real method files (during implementation)

**Verdict:** StockLotAdjustments slice is **NOT ready** for implementation without the critical blockers being fixed first. Once fixed, it can be implemented incrementally by replacing placeholders with real method files as functionality is added.

