# Parts Slice Audit

## 1. Purpose

This audit analyzes the **current scaffolded Parts slice** (folders + empty/placeholder files) and compares it against the canonical **Machines slice pattern** and **SliceMap rules** to identify:
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
- ✅ `backend/CncApp/CncApp.Api/Controllers/PartsController.cs`
  - Class: `PartsController`
  - Namespace: `CncApp.Api.Controllers`
  - Status: Stub with constructor only, no actions
  - Injects: `PartService` (concrete) ✅

### Application Layer - DTOs
- ✅ `backend/CncApp/CncApp.Application/Dtos/Parts/PartDto.cs`
  - Class: `PartDto`
  - Namespace: `CncApp.Application.Dtos.Parts`
  - Status: Stub with `Id` property, TODO for other properties
- ✅ `backend/CncApp/CncApp.Application/Dtos/Parts/CreatePartRequestDto.cs`
  - Class: `CreatePartRequestDto`
  - Namespace: `CncApp.Application.Dtos.Parts`
  - Status: Stub with TODO for properties

### Application Layer - Services
- ✅ `backend/CncApp/CncApp.Application/Services/Parts/PartService.cs`
  - Class: `PartService` (partial)
  - Namespace: `CncApp.Application.Services.Parts`
  - Status: Base file with constructor + dependencies (`IPartRepository`, `IMapper`)
- ✅ `backend/CncApp/CncApp.Application/Services/Parts/Commands/PartService.PlaceholderCommand.cs`
  - Class: `PartService` (partial)
  - Namespace: `CncApp.Application.Services.Parts` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Application/Services/Parts/Queries/PartService.PlaceholderQuery.cs`
  - Class: `PartService` (partial)
  - Namespace: `CncApp.Application.Services.Parts` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Application Layer - Mapping
- ✅ `backend/CncApp/CncApp.Application/Mapping/PartProfile.cs`
  - Class: `PartProfile` : `Profile`
  - Namespace: `CncApp.Application.Mapping`
  - Status: Stub with empty constructor, TODO for CreateMap calls

### Application Layer - Interfaces
- ✅ `backend/CncApp/CncApp.Application/Interfaces/Repositories/IPartRepository.cs`
  - Interface: `IPartRepository`
  - Namespace: `CncApp.Application.Interfaces.Repositories`
  - Status: Stub with TODO for method signatures

### Domain Layer
- ✅ `backend/CncApp/CncApp.Domain/Entities/Part.cs`
  - Class: `Part` : `AuditableEntityBase`
  - Namespace: `CncApp.Domain.Entities`
  - Status: Entity exists with properties, but **lacks domain validation** (no private constructor, no public constructor with validation, no domain methods like `Inactivate()`)
  - Properties: `ApproxPartCycleTime` (TimeSpan), `CheckPerPart` (int), `Orders` (navigation)

### Infrastructure Layer - Repositories
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Parts/PartRepository.cs`
  - Class: `PartRepository` (partial) : `IPartRepository`
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct)
  - Status: Base file with constructor + `_context` field
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Parts/Commands/PartRepository.PlaceholderCommand.cs`
  - Class: `PartRepository` (partial)
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Parts/Queries/PartRepository.PlaceholderQuery.cs`
  - Class: `PartRepository` (partial)
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Infrastructure Layer - Persistence
- ✅ `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/PartConfiguration.cs`
  - Class: `PartConfiguration` : `IEntityTypeConfiguration<Part>`
  - Namespace: `CncApp.Infrastructure.Persistence.Configurations`
  - Status: **Fully configured** with primary key, properties (ApproxPartCycleTime, CheckPerPart), relationships (Orders), required constraints
- ✅ `backend/CncApp/CncApp.Infrastructure/Persistence/AppDbContext.cs`
  - Contains: `public DbSet<Part> Parts => Set<Part>();`
  - Status: ✅ Registered

### Application Tests
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Parts/PartTests.cs`
  - Class: `PartTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Parts`
  - Status: Stub with TODO comment, **missing shared setup** (mocks, service initialization)
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Parts/Commands/PartTests.PlaceholderCommand.cs`
  - Class: `PartTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Parts` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Parts/Queries/PartTests.PlaceholderQuery.cs`
  - Class: `PartTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Parts` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Domain Tests
- ✅ `backend/CncApp/CncApp.Domain.Tests/Entities/PartTests.cs`
  - Class: `PartTests` (not partial)
  - Namespace: `CncApp.Domain.Tests.Entities`
  - Status: Stub with `#region` structure (Constructor Tests, Property Setter Tests, Method Tests), all TODOs

---

## 3. Conformance Check vs Machines

### ✅ Commands/Queries Folder Presence
- **PASS**: `Application/Services/Parts/Commands/` exists
- **PASS**: `Application/Services/Parts/Queries/` exists
- **PASS**: `Infrastructure/Repositories/Parts/Commands/` exists
- **PASS**: `Infrastructure/Repositories/Parts/Queries/` exists
- **PASS**: `Application.Tests/Services/Parts/Commands/` exists
- **PASS**: `Application.Tests/Services/Parts/Queries/` exists

### ✅ Partial Class Split Correctness
- **PASS**: `PartService.cs` is partial (base file)
- **PASS**: `PartService.PlaceholderCommand.cs` is partial
- **PASS**: `PartService.PlaceholderQuery.cs` is partial
- **PASS**: `PartRepository.cs` is partial (base file)
- **PASS**: `PartRepository.PlaceholderCommand.cs` is partial
- **PASS**: `PartRepository.PlaceholderQuery.cs` is partial
- **PASS**: `PartTests.cs` is partial (base file)
- **PASS**: `PartTests.PlaceholderCommand.cs` is partial
- **PASS**: `PartTests.PlaceholderQuery.cs` is partial

### ✅ Namespace Correctness (Prefactored Namespace Correction Applied)
- **PASS**: All `PartService` partials use `namespace CncApp.Application.Services.Parts;` (no `.Commands`/`.Queries`)
- **PASS**: All `PartRepository` partials use `namespace CncApp.Infrastructure.Repositories;` (no `.Commands`/`.Queries`)
- **PASS**: All `PartTests` partials use `namespace CncApp.Application.Tests.Services.Parts;` (no `.Commands`/`.Queries`)

### ⚠️ Test Structure Correctness
- **PASS**: Test folder structure mirrors `Application/Services/Parts/` structure
- **PASS**: Test files are partial classes
- **FAIL**: `PartTests.cs` base file is missing shared setup (mocks, service initialization)
  - **Expected**: Should contain `Mock<IPartRepository>`, `Mock<IMapper>`, `PartService` initialization like `MachineTests.cs`
  - **Current**: Only contains TODO comment

### ⚠️ Domain Entity Correctness
- **FAIL**: `Part.cs` lacks domain validation patterns
  - **Missing**: Private constructor for EF Core materialization
  - **Missing**: Public constructor with validation
  - **Missing**: Domain methods (e.g., `Inactivate()`)
  - **Missing**: Property setter validation using `Guard` class
  - **Current**: Simple POCO with public setters (no domain invariants)
  - **Note**: `ApproxPartCycleTime` is TimeSpan (must be non-negative), `CheckPerPart` is int (must be non-negative)

### ❌ Dependency Injection Registration
- **FAIL**: `PartService` not registered in `Application/DependencyInjection.cs`
  - **Expected**: `services.AddScoped<PartService>();`
- **FAIL**: `IPartRepository` not registered in `Infrastructure/DependencyInjection.cs`
  - **Expected**: `services.AddScoped<IPartRepository, PartRepository>();`

---

## 4. Placeholder Inventory

| File Path | Placeholder Type | Why It's Placeholder | What It Should Become |
|-----------|------------------|----------------------|----------------------|
| `Application/Services/Parts/Commands/PartService.PlaceholderCommand.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `PartService.Create.cs`, `PartService.Inactivate.cs` (one file per method) |
| `Application/Services/Parts/Queries/PartService.PlaceholderQuery.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `PartService.Get.cs`, `PartService.ListActive.cs`, `PartService.ListAll.cs` (one file per method) |
| `Infrastructure/Repositories/Parts/Commands/PartRepository.PlaceholderCommand.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `PartRepository.Add.cs`, `PartRepository.Inactivate.cs`, `PartRepository.SaveChanges.cs` (one file per method) |
| `Infrastructure/Repositories/Parts/Queries/PartRepository.PlaceholderQuery.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `PartRepository.GetById.cs`, `PartRepository.ListActive.cs`, `PartRepository.ListAll.cs` (one file per method) |
| `Application.Tests/Services/Parts/Commands/PartTests.PlaceholderCommand.cs` | Test Placeholder | Empty partial class with TODO | **DELETE** and replace with: `PartTests.Create.cs`, `PartTests.Inactivate.cs` (one file per method test) |
| `Application.Tests/Services/Parts/Queries/PartTests.PlaceholderQuery.cs` | Test Placeholder | Empty partial class with TODO | **DELETE** and replace with: `PartTests.Get.cs`, `PartTests.ListActive.cs`, `PartTests.ListAll.cs` (one file per method test) |

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
**No changes needed** - `PartsController.cs` is correctly structured as a stub.

### Application Layer - DTOs
**No changes needed** - DTOs are correctly structured as stubs. Properties need to be added based on `PartConfiguration.cs` and `Part.cs` entity:
- `PartDto` should include: `Id`, `ApproxPartCycleTime`, `CheckPerPart`, audit fields (if exposed)
- `CreatePartRequestDto` should include: `ApproxPartCycleTime`, `CheckPerPart`

### Application Layer - Services
**REPLACE (delete placeholders, create method files):**
1. **DELETE**: `Application/Services/Parts/Commands/PartService.PlaceholderCommand.cs`
2. **CREATE**: `Application/Services/Parts/Commands/PartService.Create.cs` (mirror `MachineService.Create.cs`)
3. **CREATE**: `Application/Services/Parts/Commands/PartService.Inactivate.cs` (mirror `MachineService.Inactivate.cs`)
4. **DELETE**: `Application/Services/Parts/Queries/PartService.PlaceholderQuery.cs`
5. **CREATE**: `Application/Services/Parts/Queries/PartService.Get.cs` (mirror `MachineService.Get.cs`)
6. **CREATE**: `Application/Services/Parts/Queries/PartService.ListActive.cs` (mirror `MachineService.ListActive.cs`)
7. **CREATE**: `Application/Services/Parts/Queries/PartService.ListAll.cs` (mirror `MachineService.ListAll.cs`)

### Application Layer - Mapping
**No changes needed** - `PartProfile.cs` is correctly structured as a stub. CreateMap calls need to be added when implementing:
- `CreateMap<Part, PartDto>();`
- `CreateMap<CreatePartRequestDto, Part>();`

### Application Layer - Interfaces
**No changes needed** - `IPartRepository.cs` is correctly structured as a stub. Method signatures need to be added (mirror `IMachineRepository.cs`):
- `Task<Part?> GetByIdAsync(int id, CancellationToken ct = default);`
- `Task<List<Part>> ListActiveAsync(CancellationToken ct = default);`
- `Task<List<Part>> ListAllAsync(CancellationToken ct = default);`
- `Task AddAsync(Part part, CancellationToken ct = default);`
- `Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);`
- `Task SaveChangesAsync(CancellationToken ct = default);`

### Domain Layer
**ENHANCE (add domain validation):**
1. **UPDATE**: `Domain/Entities/Part.cs`
   - Add private constructor for EF Core materialization
   - Add public constructor with validation (using `Guard` class)
   - Add property setter validation for `ApproxPartCycleTime` (must be non-negative TimeSpan) and `CheckPerPart` (must be non-negative int)
   - Add domain methods (e.g., `Inactivate()` method)
   - Mirror `Machine.cs` domain validation patterns

### Infrastructure Layer - Repositories
**REPLACE (delete placeholders, create method files):**
1. **DELETE**: `Infrastructure/Repositories/Parts/Commands/PartRepository.PlaceholderCommand.cs`
2. **CREATE**: `Infrastructure/Repositories/Parts/Commands/PartRepository.Add.cs` (mirror `MachineRepository.Add.cs`)
3. **CREATE**: `Infrastructure/Repositories/Parts/Commands/PartRepository.Inactivate.cs` (mirror `MachineRepository.Inactivate.cs`)
4. **CREATE**: `Infrastructure/Repositories/Parts/Commands/PartRepository.SaveChanges.cs` (mirror `MachineRepository.SaveChanges.cs`)
5. **DELETE**: `Infrastructure/Repositories/Parts/Queries/PartRepository.PlaceholderQuery.cs`
6. **CREATE**: `Infrastructure/Repositories/Parts/Queries/PartRepository.GetById.cs` (mirror `MachineRepository.GetById.cs`)
7. **CREATE**: `Infrastructure/Repositories/Parts/Queries/PartRepository.ListActive.cs` (mirror `MachineRepository.ListActive.cs`)
8. **CREATE**: `Infrastructure/Repositories/Parts/Queries/PartRepository.ListAll.cs` (mirror `MachineRepository.ListAll.cs`)

### Infrastructure Layer - Persistence
**No changes needed** - `PartConfiguration.cs` is fully configured and correct.

### Infrastructure Layer - Dependency Injection
**ADD (register repository):**
1. **UPDATE**: `Infrastructure/DependencyInjection.cs`
   - Add: `services.AddScoped<IPartRepository, PartRepository>();`

### Application Layer - Dependency Injection
**ADD (register service):**
1. **UPDATE**: `Application/DependencyInjection.cs`
   - Add: `services.AddScoped<PartService>();`

### Application Tests
**REPLACE (delete placeholders, create method test files) AND FIX (add shared setup):**
1. **UPDATE**: `Application.Tests/Services/Parts/PartTests.cs`
   - Add shared setup: `Mock<IPartRepository>`, `Mock<IMapper>`, `PartService` initialization (mirror `MachineTests.cs`)
2. **DELETE**: `Application.Tests/Services/Parts/Commands/PartTests.PlaceholderCommand.cs`
3. **CREATE**: `Application.Tests/Services/Parts/Commands/PartTests.Create.cs` (mirror `MachineTests.Create.cs`)
4. **CREATE**: `Application.Tests/Services/Parts/Commands/PartTests.Inactivate.cs` (mirror `MachineTests.Inactivate.cs`)
5. **DELETE**: `Application.Tests/Services/Parts/Queries/PartTests.PlaceholderQuery.cs`
6. **CREATE**: `Application.Tests/Services/Parts/Queries/PartTests.Get.cs` (mirror `MachineTests.Get.cs`)
7. **CREATE**: `Application.Tests/Services/Parts/Queries/PartTests.ListActive.cs` (mirror `MachineTests.ListActive.cs`)
8. **CREATE**: `Application.Tests/Services/Parts/Queries/PartTests.ListAll.cs` (mirror `MachineTests.ListAll.cs`)

### Domain Tests
**No changes needed** - `PartTests.cs` is correctly structured with `#region` organization. Tests need to be implemented.

---

## 6. Parts Slice "Ready-to-Implement?" Verdict

### ❌ **NOT READY** - Minimum Cleanup Required

**Critical Blockers:**
1. **Missing Dependency Injection Registrations** - Service and repository not registered
2. **Missing Domain Validation** - `Part.cs` lacks domain invariants (no private constructor, no validation, no domain methods)
3. **Placeholder Files Present** - 6 placeholder files must be deleted and replaced with method-specific files
4. **Missing Test Setup** - `PartTests.cs` base file lacks shared mocks and service initialization

**Minimum Cleanup Before Implementation:**
1. ✅ **Fix Dependency Injection** (5 minutes)
   - Add `PartService` registration in `Application/DependencyInjection.cs`
   - Add `IPartRepository` registration in `Infrastructure/DependencyInjection.cs`
2. ✅ **Enhance Domain Entity** (30 minutes)
   - Add private constructor for EF Core
   - Add public constructor with validation
   - Add property setter validation for `ApproxPartCycleTime` (non-negative TimeSpan) and `CheckPerPart` (non-negative int)
   - Add `Inactivate()` domain method
3. ✅ **Add Test Base Setup** (10 minutes)
   - Add mocks and service initialization to `PartTests.cs`
4. ⚠️ **Replace Placeholders** (can be done incrementally during implementation)
   - Delete placeholder files as you create real method files
   - Create method files one at a time as you implement functionality

**Recommended Implementation Order:**
1. Fix Dependency Injection registrations
2. Enhance `Part.cs` domain entity with validation
3. Add test base setup to `PartTests.cs`
4. Implement repository methods (delete placeholders, create method files)
5. Implement service methods (delete placeholders, create method files)
6. Implement controller actions
7. Implement tests (delete placeholders, create method test files)

---

## 7. Golden Reference Pointers

When implementing Parts slice functionality, mirror these **Machines slice files exactly**:

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

**What's Wrong:**
- ❌ Missing Dependency Injection registrations
- ❌ Domain entity lacks validation patterns
- ❌ Test base file missing shared setup
- ❌ 6 placeholder files must be replaced with method-specific files

**What Needs to Happen:**
1. Fix DI registrations (critical blocker)
2. Enhance domain entity with validation (critical blocker)
3. Add test base setup (critical blocker)
4. Replace placeholders with real method files (during implementation)

**Verdict:** Parts slice is **NOT ready** for implementation without the critical blockers being fixed first. Once fixed, it can be implemented incrementally by replacing placeholders with real method files as functionality is added.

