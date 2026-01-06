# Materials Slice Audit

## 1. Purpose

This audit analyzes the **current scaffolded Materials slice** (folders + empty/placeholder files) and compares it against the canonical **Machines slice pattern** and **SliceMap rules** to identify:
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
- ✅ `backend/CncApp/CncApp.Api/Controllers/MaterialsController.cs`
  - Class: `MaterialsController`
  - Namespace: `CncApp.Api.Controllers`
  - Status: Stub with constructor only, no actions
  - Injects: `MaterialService` (concrete) ✅

### Application Layer - DTOs
- ✅ `backend/CncApp/CncApp.Application/Dtos/Materials/MaterialDto.cs`
  - Class: `MaterialDto`
  - Namespace: `CncApp.Application.Dtos.Materials`
  - Status: Stub with `Id` property, TODO for other properties
- ✅ `backend/CncApp/CncApp.Application/Dtos/Materials/CreateMaterialRequestDto.cs`
  - Class: `CreateMaterialRequestDto`
  - Namespace: `CncApp.Application.Dtos.Materials`
  - Status: Stub with TODO for properties

### Application Layer - Services
- ✅ `backend/CncApp/CncApp.Application/Services/Materials/MaterialService.cs`
  - Class: `MaterialService` (partial)
  - Namespace: `CncApp.Application.Services.Materials`
  - Status: Base file with constructor + dependencies (`IMaterialRepository`, `IMapper`)
- ✅ `backend/CncApp/CncApp.Application/Services/Materials/Commands/MaterialService.PlaceholderCommand.cs`
  - Class: `MaterialService` (partial)
  - Namespace: `CncApp.Application.Services.Materials` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Application/Services/Materials/Queries/MaterialService.PlaceholderQuery.cs`
  - Class: `MaterialService` (partial)
  - Namespace: `CncApp.Application.Services.Materials` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Application Layer - Mapping
- ✅ `backend/CncApp/CncApp.Application/Mapping/MaterialProfile.cs`
  - Class: `MaterialProfile` : `Profile`
  - Namespace: `CncApp.Application.Mapping`
  - Status: Stub with empty constructor, TODO for CreateMap calls

### Application Layer - Interfaces
- ✅ `backend/CncApp/CncApp.Application/Interfaces/Repositories/IMaterialRepository.cs`
  - Interface: `IMaterialRepository`
  - Namespace: `CncApp.Application.Interfaces.Repositories`
  - Status: Stub with TODO for method signatures

### Domain Layer
- ✅ `backend/CncApp/CncApp.Domain/Entities/Material.cs`
  - Class: `Material` : `AuditableEntityBase`
  - Namespace: `CncApp.Domain.Entities`
  - Status: Entity exists with properties, but **lacks domain validation** (no private constructor, no public constructor with validation, no domain methods like `Inactivate()`)
  - Properties: `HeatNumber`, `MaterialName`, `StockLots` (navigation)

### Infrastructure Layer - Repositories
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Materials/MaterialRepository.cs`
  - Class: `MaterialRepository` (partial) : `IMaterialRepository`
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct)
  - Status: Base file with constructor + `_context` field
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Materials/Commands/MaterialRepository.PlaceholderCommand.cs`
  - Class: `MaterialRepository` (partial)
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Materials/Queries/MaterialRepository.PlaceholderQuery.cs`
  - Class: `MaterialRepository` (partial)
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Infrastructure Layer - Persistence
- ✅ `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/MaterialConfiguration.cs`
  - Class: `MaterialConfiguration` : `IEntityTypeConfiguration<Material>`
  - Namespace: `CncApp.Infrastructure.Persistence.Configurations`
  - Status: **Fully configured** with primary key, properties (HeatNumber, MaterialName), relationships (StockLots), max lengths (100)
- ✅ `backend/CncApp/CncApp.Infrastructure/Persistence/AppDbContext.cs`
  - Contains: `public DbSet<Material> Materials => Set<Material>();`
  - Status: ✅ Registered

### Application Tests
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Materials/MaterialTests.cs`
  - Class: `MaterialTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Materials`
  - Status: Stub with TODO comment, **missing shared setup** (mocks, service initialization)
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Materials/Commands/MaterialTests.PlaceholderCommand.cs`
  - Class: `MaterialTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Materials` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Materials/Queries/MaterialTests.PlaceholderQuery.cs`
  - Class: `MaterialTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Materials` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Domain Tests
- ✅ `backend/CncApp/CncApp.Domain.Tests/Entities/MaterialTests.cs`
  - Class: `MaterialTests` (not partial)
  - Namespace: `CncApp.Domain.Tests.Entities`
  - Status: Stub with `#region` structure (Constructor Tests, Property Setter Tests, Method Tests), all TODOs

---

## 3. Conformance Check vs Machines

### ✅ Commands/Queries Folder Presence
- **PASS**: `Application/Services/Materials/Commands/` exists
- **PASS**: `Application/Services/Materials/Queries/` exists
- **PASS**: `Infrastructure/Repositories/Materials/Commands/` exists
- **PASS**: `Infrastructure/Repositories/Materials/Queries/` exists
- **PASS**: `Application.Tests/Services/Materials/Commands/` exists
- **PASS**: `Application.Tests/Services/Materials/Queries/` exists

### ✅ Partial Class Split Correctness
- **PASS**: `MaterialService.cs` is partial (base file)
- **PASS**: `MaterialService.PlaceholderCommand.cs` is partial
- **PASS**: `MaterialService.PlaceholderQuery.cs` is partial
- **PASS**: `MaterialRepository.cs` is partial (base file)
- **PASS**: `MaterialRepository.PlaceholderCommand.cs` is partial
- **PASS**: `MaterialRepository.PlaceholderQuery.cs` is partial
- **PASS**: `MaterialTests.cs` is partial (base file)
- **PASS**: `MaterialTests.PlaceholderCommand.cs` is partial
- **PASS**: `MaterialTests.PlaceholderQuery.cs` is partial

### ✅ Namespace Correctness (Prefactored Namespace Correction Applied)
- **PASS**: All `MaterialService` partials use `namespace CncApp.Application.Services.Materials;` (no `.Commands`/`.Queries`)
- **PASS**: All `MaterialRepository` partials use `namespace CncApp.Infrastructure.Repositories;` (no `.Commands`/`.Queries`)
- **PASS**: All `MaterialTests` partials use `namespace CncApp.Application.Tests.Services.Materials;` (no `.Commands`/`.Queries`)

### ⚠️ Test Structure Correctness
- **PASS**: Test folder structure mirrors `Application/Services/Materials/` structure
- **PASS**: Test files are partial classes
- **FAIL**: `MaterialTests.cs` base file is missing shared setup (mocks, service initialization)
  - **Expected**: Should contain `Mock<IMaterialRepository>`, `Mock<IMapper>`, `MaterialService` initialization like `MachineTests.cs`
  - **Current**: Only contains TODO comment

### ⚠️ Domain Entity Correctness
- **FAIL**: `Material.cs` lacks domain validation patterns
  - **Missing**: Private constructor for EF Core materialization
  - **Missing**: Public constructor with validation
  - **Missing**: Domain methods (e.g., `Inactivate()`)
  - **Missing**: Property setter validation using `Guard` class
  - **Current**: Simple POCO with public setters (no domain invariants)

### ❌ Dependency Injection Registration
- **FAIL**: `MaterialService` not registered in `Application/DependencyInjection.cs`
  - **Expected**: `services.AddScoped<MaterialService>();`
- **FAIL**: `IMaterialRepository` not registered in `Infrastructure/DependencyInjection.cs`
  - **Expected**: `services.AddScoped<IMaterialRepository, MaterialRepository>();`

---

## 4. Placeholder Inventory

| File Path | Placeholder Type | Why It's Placeholder | What It Should Become |
|-----------|------------------|----------------------|----------------------|
| `Application/Services/Materials/Commands/MaterialService.PlaceholderCommand.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `MaterialService.Create.cs`, `MaterialService.Inactivate.cs` (one file per method) |
| `Application/Services/Materials/Queries/MaterialService.PlaceholderQuery.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `MaterialService.Get.cs`, `MaterialService.ListActive.cs`, `MaterialService.ListAll.cs` (one file per method) |
| `Infrastructure/Repositories/Materials/Commands/MaterialRepository.PlaceholderCommand.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `MaterialRepository.Add.cs`, `MaterialRepository.Inactivate.cs`, `MaterialRepository.SaveChanges.cs` (one file per method) |
| `Infrastructure/Repositories/Materials/Queries/MaterialRepository.PlaceholderQuery.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `MaterialRepository.GetById.cs`, `MaterialRepository.ListActive.cs`, `MaterialRepository.ListAll.cs` (one file per method) |
| `Application.Tests/Services/Materials/Commands/MaterialTests.PlaceholderCommand.cs` | Test Placeholder | Empty partial class with TODO | **DELETE** and replace with: `MaterialTests.Create.cs`, `MaterialTests.Inactivate.cs` (one file per method test) |
| `Application.Tests/Services/Materials/Queries/MaterialTests.PlaceholderQuery.cs` | Test Placeholder | Empty partial class with TODO | **DELETE** and replace with: `MaterialTests.Get.cs`, `MaterialTests.ListActive.cs`, `MaterialTests.ListAll.cs` (one file per method test) |

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
**No changes needed** - `MaterialsController.cs` is correctly structured as a stub.

### Application Layer - DTOs
**No changes needed** - DTOs are correctly structured as stubs. Properties need to be added based on `MaterialConfiguration.cs` and `Material.cs` entity:
- `MaterialDto` should include: `Id`, `HeatNumber`, `MaterialName`, audit fields (if exposed)
- `CreateMaterialRequestDto` should include: `HeatNumber`, `MaterialName`

### Application Layer - Services
**REPLACE (delete placeholders, create method files):**
1. **DELETE**: `Application/Services/Materials/Commands/MaterialService.PlaceholderCommand.cs`
2. **CREATE**: `Application/Services/Materials/Commands/MaterialService.Create.cs` (mirror `MachineService.Create.cs`)
3. **CREATE**: `Application/Services/Materials/Commands/MaterialService.Inactivate.cs` (mirror `MachineService.Inactivate.cs`)
4. **DELETE**: `Application/Services/Materials/Queries/MaterialService.PlaceholderQuery.cs`
5. **CREATE**: `Application/Services/Materials/Queries/MaterialService.Get.cs` (mirror `MachineService.Get.cs`)
6. **CREATE**: `Application/Services/Materials/Queries/MaterialService.ListActive.cs` (mirror `MachineService.ListActive.cs`)
7. **CREATE**: `Application/Services/Materials/Queries/MaterialService.ListAll.cs` (mirror `MachineService.ListAll.cs`)

### Application Layer - Mapping
**No changes needed** - `MaterialProfile.cs` is correctly structured as a stub. CreateMap calls need to be added when implementing:
- `CreateMap<Material, MaterialDto>();`
- `CreateMap<CreateMaterialRequestDto, Material>();`

### Application Layer - Interfaces
**No changes needed** - `IMaterialRepository.cs` is correctly structured as a stub. Method signatures need to be added (mirror `IMachineRepository.cs`):
- `Task<Material?> GetByIdAsync(int id, CancellationToken ct = default);`
- `Task<List<Material>> ListActiveAsync(CancellationToken ct = default);`
- `Task<List<Material>> ListAllAsync(CancellationToken ct = default);`
- `Task AddAsync(Material material, CancellationToken ct = default);`
- `Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);`
- `Task SaveChangesAsync(CancellationToken ct = default);`

### Domain Layer
**ENHANCE (add domain validation):**
1. **UPDATE**: `Domain/Entities/Material.cs`
   - Add private constructor for EF Core materialization
   - Add public constructor with validation (using `Guard` class)
   - Add property setter validation for `HeatNumber` and `MaterialName` (max length 100, required)
   - Add domain methods (e.g., `Inactivate()` method)
   - Mirror `Machine.cs` domain validation patterns

### Infrastructure Layer - Repositories
**REPLACE (delete placeholders, create method files):**
1. **DELETE**: `Infrastructure/Repositories/Materials/Commands/MaterialRepository.PlaceholderCommand.cs`
2. **CREATE**: `Infrastructure/Repositories/Materials/Commands/MaterialRepository.Add.cs` (mirror `MachineRepository.Add.cs`)
3. **CREATE**: `Infrastructure/Repositories/Materials/Commands/MaterialRepository.Inactivate.cs` (mirror `MachineRepository.Inactivate.cs`)
4. **CREATE**: `Infrastructure/Repositories/Materials/Commands/MaterialRepository.SaveChanges.cs` (mirror `MachineRepository.SaveChanges.cs`)
5. **DELETE**: `Infrastructure/Repositories/Materials/Queries/MaterialRepository.PlaceholderQuery.cs`
6. **CREATE**: `Infrastructure/Repositories/Materials/Queries/MaterialRepository.GetById.cs` (mirror `MachineRepository.GetById.cs`)
7. **CREATE**: `Infrastructure/Repositories/Materials/Queries/MaterialRepository.ListActive.cs` (mirror `MachineRepository.ListActive.cs`)
8. **CREATE**: `Infrastructure/Repositories/Materials/Queries/MaterialRepository.ListAll.cs` (mirror `MachineRepository.ListAll.cs`)

### Infrastructure Layer - Persistence
**No changes needed** - `MaterialConfiguration.cs` is fully configured and correct.

### Infrastructure Layer - Dependency Injection
**ADD (register repository):**
1. **UPDATE**: `Infrastructure/DependencyInjection.cs`
   - Add: `services.AddScoped<IMaterialRepository, MaterialRepository>();`

### Application Layer - Dependency Injection
**ADD (register service):**
1. **UPDATE**: `Application/DependencyInjection.cs`
   - Add: `services.AddScoped<MaterialService>();`

### Application Tests
**REPLACE (delete placeholders, create method test files) AND FIX (add shared setup):**
1. **UPDATE**: `Application.Tests/Services/Materials/MaterialTests.cs`
   - Add shared setup: `Mock<IMaterialRepository>`, `Mock<IMapper>`, `MaterialService` initialization (mirror `MachineTests.cs`)
2. **DELETE**: `Application.Tests/Services/Materials/Commands/MaterialTests.PlaceholderCommand.cs`
3. **CREATE**: `Application.Tests/Services/Materials/Commands/MaterialTests.Create.cs` (mirror `MachineTests.Create.cs`)
4. **CREATE**: `Application.Tests/Services/Materials/Commands/MaterialTests.Inactivate.cs` (mirror `MachineTests.Inactivate.cs`)
5. **DELETE**: `Application.Tests/Services/Materials/Queries/MaterialTests.PlaceholderQuery.cs`
6. **CREATE**: `Application.Tests/Services/Materials/Queries/MaterialTests.Get.cs` (mirror `MachineTests.Get.cs`)
7. **CREATE**: `Application.Tests/Services/Materials/Queries/MaterialTests.ListActive.cs` (mirror `MachineTests.ListActive.cs`)
8. **CREATE**: `Application.Tests/Services/Materials/Queries/MaterialTests.ListAll.cs` (mirror `MachineTests.ListAll.cs`)

### Domain Tests
**No changes needed** - `MaterialTests.cs` is correctly structured with `#region` organization. Tests need to be implemented.

---

## 6. Materials Slice "Ready-to-Implement?" Verdict

### ❌ **NOT READY** - Minimum Cleanup Required

**Critical Blockers:**
1. **Missing Dependency Injection Registrations** - Service and repository not registered
2. **Missing Domain Validation** - `Material.cs` lacks domain invariants (no private constructor, no validation, no domain methods)
3. **Placeholder Files Present** - 6 placeholder files must be deleted and replaced with method-specific files
4. **Missing Test Setup** - `MaterialTests.cs` base file lacks shared mocks and service initialization

**Minimum Cleanup Before Implementation:**
1. ✅ **Fix Dependency Injection** (5 minutes)
   - Add `MaterialService` registration in `Application/DependencyInjection.cs`
   - Add `IMaterialRepository` registration in `Infrastructure/DependencyInjection.cs`
2. ✅ **Enhance Domain Entity** (30 minutes)
   - Add private constructor for EF Core
   - Add public constructor with validation
   - Add property setter validation for `HeatNumber` and `MaterialName` (max length 100, required)
   - Add `Inactivate()` domain method
3. ✅ **Add Test Base Setup** (10 minutes)
   - Add mocks and service initialization to `MaterialTests.cs`
4. ⚠️ **Replace Placeholders** (can be done incrementally during implementation)
   - Delete placeholder files as you create real method files
   - Create method files one at a time as you implement functionality

**Recommended Implementation Order:**
1. Fix Dependency Injection registrations
2. Enhance `Material.cs` domain entity with validation
3. Add test base setup to `MaterialTests.cs`
4. Implement repository methods (delete placeholders, create method files)
5. Implement service methods (delete placeholders, create method files)
6. Implement controller actions
7. Implement tests (delete placeholders, create method test files)

---

## 7. Golden Reference Pointers

When implementing Materials slice functionality, mirror these **Machines slice files exactly**:

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

**Verdict:** Materials slice is **NOT ready** for implementation without the critical blockers being fixed first. Once fixed, it can be implemented incrementally by replacing placeholders with real method files as functionality is added.

