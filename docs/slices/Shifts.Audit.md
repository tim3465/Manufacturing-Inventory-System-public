# Shifts Slice Audit

## 1. Purpose

This audit analyzes the **current scaffolded Shifts slice** (folders + empty/placeholder files) and compares it against the canonical **Machines slice pattern** and **SliceMap rules** to identify:
- What's correct and matches the pattern
- What's wrong or missing
- What needs to be renamed/replaced before implementing real functionality

**Reference Documents:**
- `/docs/SliceMap.md` - Canonical structure rules (**Shifts is currently missing from SliceMap; add entry before contracts**)
- `/docs/MachinesStructure.md` - Complete Machines file listing
- `/docs/MachinesConventions.md` - Detailed conventions
- `/docs/Final-scaffold-corrections/Pre_factored_Namespace_correction.md` - Namespace rules

---

## 2. Snapshot: What Exists Today

### API Layer
- ✅ `backend/CncApp/CncApp.Api/Controllers/ShiftsController.cs`
  - Class: `ShiftsController`
  - Namespace: `CncApp.Api.Controllers`
  - Status: Stub with constructor only; no actions
  - Injects: `ShiftService` (concrete)

### Application Layer - DTOs
- ✅ `backend/CncApp/CncApp.Application/Dtos/Shifts/ShiftResultDto.cs`
  - Class: `ShiftResultDto`
  - Namespace: `CncApp.Application.Dtos.Shifts`
  - Status: Stub (`Id` only, TODO for properties/annotations)
- ✅ `backend/CncApp/CncApp.Application/Dtos/Shifts/CreateShiftRequestDto.cs`
  - Class: `CreateShiftRequestDto`
  - Namespace: `CncApp.Application.Dtos.Shifts`
  - Status: Stub (TODO for properties/annotations)

### Application Layer - Services
- ✅ `backend/CncApp/CncApp.Application/Services/Shifts/ShiftService.cs`
  - Class: `ShiftService` (partial)
  - Namespace: `CncApp.Application.Services.Shifts`
  - Status: Base file with constructor + dependencies (`IShiftRepository`, `IMapper`)
- ✅ `backend/CncApp/CncApp.Application/Services/Shifts/Commands/ShiftService.PlaceholderCommand.cs`
  - Class: `ShiftService` (partial)
  - Namespace: `CncApp.Application.Services.Shifts`
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Application/Services/Shifts/Queries/ShiftService.PlaceholderQuery.cs`
  - Class: `ShiftService` (partial)
  - Namespace: `CncApp.Application.Services.Shifts`
  - Status: Placeholder file, must be replaced

### Application Layer - Mapping
- ✅ `backend/CncApp/CncApp.Application/Mapping/ShiftProfile.cs`
  - Class: `ShiftProfile` : `Profile`
  - Namespace: `CncApp.Application.Mapping`
  - Status: Stub with empty constructor, TODO for CreateMap calls

### Application Layer - Interfaces
- ✅ `backend/CncApp/CncApp.Application/Interfaces/Repositories/IShiftRepository.cs`
  - Interface: `IShiftRepository`
  - Namespace: `CncApp.Application.Interfaces.Repositories`
  - Status: Stub with TODO for method signatures

### Domain Layer
- ✅ `backend/CncApp/CncApp.Domain/Entities/Shift.cs`
  - Class: `Shift` : `AuditableEntityBase`
  - Namespace: `CncApp.Domain.Entities`
  - Status: Entity exists with properties, but **lacks domain validation** (no private constructor, no public constructor with validation, no domain methods like `Inactivate()`, no guarded setters)

### Infrastructure Layer - Repositories
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Shifts/ShiftRepository.cs`
  - Class: `ShiftRepository` (partial) : `IShiftRepository`
  - Namespace: `CncApp.Infrastructure.Repositories`
  - Status: Base file with constructor + `_context` field
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Shifts/Commands/ShiftRepository.PlaceholderCommand.cs`
  - Class: `ShiftRepository` (partial)
  - Namespace: `CncApp.Infrastructure.Repositories`
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Shifts/Queries/ShiftRepository.PlaceholderQuery.cs`
  - Class: `ShiftRepository` (partial)
  - Namespace: `CncApp.Infrastructure.Repositories`
  - Status: Placeholder file, must be replaced

### Infrastructure Layer - Persistence
- ✅ `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/ShiftConfiguration.cs`
  - Class: `ShiftConfiguration` : `IEntityTypeConfiguration<Shift>`
  - Namespace: `CncApp.Infrastructure.Persistence.Configurations`
  - Status: Configured with keys, required properties, relationships, indexes
- ✅ `backend/CncApp/CncApp.Infrastructure/Persistence/AppDbContext.cs`
  - Contains: `public DbSet<Shift> Shifts => Set<Shift>();`
  - Status: DbSet registered
- ℹ️ Migrations: `20260103164746_AddBarsAndPartsPerBarToJobAndShift` (code + designer) exists for Shift-related schema

### Application Tests
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Shifts/ShiftTests.cs`
  - Class: `ShiftTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Shifts`
  - Status: Base file comment only; **missing shared setup** (mocks + service initialization)
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Shifts/Commands/ShiftTests.PlaceholderCommand.cs`
  - Class: `ShiftTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Shifts`
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Shifts/Queries/ShiftTests.PlaceholderQuery.cs`
  - Class: `ShiftTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Shifts`
  - Status: Placeholder file, must be replaced

### Domain Tests
- ✅ `backend/CncApp/CncApp.Domain.Tests/Entities/ShiftTests.cs`
  - Class: `ShiftTests`
  - Namespace: `CncApp.Domain.Tests.Entities`
  - Status: Stub with TODO regions for constructors, setters, and methods

---

## 3. Conformance Check vs Machines

### ✅ Commands/Queries Folder Presence
- **PASS**: `Application/Services/Shifts/Commands/` and `Queries/` exist
- **PASS**: `Infrastructure/Repositories/Shifts/Commands/` and `Queries/` exist
- **PASS**: `Application.Tests/Services/Shifts/Commands/` and `Queries/` exist

### ✅ Partial Class Split Correctness
- **PASS**: `ShiftService` base + placeholder partials
- **PASS**: `ShiftRepository` base + placeholder partials
- **PASS**: `ShiftTests` base + placeholder partials

### ✅ Namespace Correctness
- **PASS**: Application services use `CncApp.Application.Services.Shifts` (no `.Commands`/`.Queries`)
- **PASS**: Repositories use `CncApp.Infrastructure.Repositories` (no `.Commands`/`.Queries`)
- **PASS**: Application tests use `CncApp.Application.Tests.Services.Shifts` (no `.Commands`/`.Queries`)

### ⚠️ Test Structure Correctness
- **FAIL**: `ShiftTests.cs` base file lacks shared setup (expected mocks + `ShiftService` initialization mirroring `MachineTests.cs`)

### ⚠️ Domain Entity Correctness
- **FAIL**: `Shift.cs` lacks domain validation patterns (no guarded constructors, no domain methods, public setters without validation)

### ❌ Dependency Injection Registration
- **FAIL**: `ShiftService` not registered in `Application/DependencyInjection.cs`
- **FAIL**: `IShiftRepository` not registered in `Infrastructure/DependencyInjection.cs`

### ❌ SliceMap Coverage
- **FAIL**: `Shifts` entry not present in `/docs/SliceMap.md` (add before defining contracts to avoid drift)

---

## 4. Placeholder Inventory

| File Path | Placeholder Type | Why It's Placeholder | What It Should Become |
|-----------|------------------|----------------------|----------------------|
| `Application/Services/Shifts/Commands/ShiftService.PlaceholderCommand.cs` | Method Placeholder | Empty partial with TODO | **DELETE** and replace with: `ShiftService.Create.cs`, `ShiftService.Inactivate.cs` (one file per command) |
| `Application/Services/Shifts/Queries/ShiftService.PlaceholderQuery.cs` | Method Placeholder | Empty partial with TODO | **DELETE** and replace with: `ShiftService.Get.cs`, `ShiftService.ListActive.cs`, `ShiftService.ListAll.cs` (one file per query) |
| `Infrastructure/Repositories/Shifts/Commands/ShiftRepository.PlaceholderCommand.cs` | Method Placeholder | Empty partial with TODO | **DELETE** and replace with: `ShiftRepository.Add.cs`, `ShiftRepository.Inactivate.cs`, `ShiftRepository.SaveChanges.cs` (one file per command) |
| `Infrastructure/Repositories/Shifts/Queries/ShiftRepository.PlaceholderQuery.cs` | Method Placeholder | Empty partial with TODO | **DELETE** and replace with: `ShiftRepository.GetById.cs`, `ShiftRepository.ListActive.cs`, `ShiftRepository.ListAll.cs` (one file per query) |
| `Application.Tests/Services/Shifts/Commands/ShiftTests.PlaceholderCommand.cs` | Test Placeholder | Empty partial with TODO | **DELETE** and replace with: `ShiftTests.Create.cs`, `ShiftTests.Inactivate.cs` (one file per command test) |
| `Application.Tests/Services/Shifts/Queries/ShiftTests.PlaceholderQuery.cs` | Test Placeholder | Empty partial with TODO | **DELETE** and replace with: `ShiftTests.Get.cs`, `ShiftTests.ListActive.cs`, `ShiftTests.ListAll.cs` (one file per query test) |

---

## 5. Required Renames / Deletes / Replacements

### API Layer
- **No changes needed** - `ShiftsController.cs` is correctly structured as a stub.

### Application Layer - DTOs
- **No changes needed** for structure. Add properties/DataAnnotations based on `ShiftConfiguration` + `Shift` entity when implementing.

### Application Layer - Services
- **REPLACE**: Delete placeholders and create method files:
  1. **DELETE** `Application/Services/Shifts/Commands/ShiftService.PlaceholderCommand.cs`
  2. **CREATE** `Application/Services/Shifts/Commands/ShiftService.Create.cs` (mirror `MachineService.Create.cs`)
  3. **CREATE** `Application/Services/Shifts/Commands/ShiftService.Inactivate.cs` (mirror `MachineService.Inactivate.cs`)
  4. **DELETE** `Application/Services/Shifts/Queries/ShiftService.PlaceholderQuery.cs`
  5. **CREATE** `Application/Services/Shifts/Queries/ShiftService.Get.cs` (mirror `MachineService.Get.cs`)
  6. **CREATE** `Application/Services/Shifts/Queries/ShiftService.ListActive.cs` (mirror `MachineService.ListActive.cs`)
  7. **CREATE** `Application/Services/Shifts/Queries/ShiftService.ListAll.cs` (mirror `MachineService.ListAll.cs`)

### Application Layer - Mapping
- **No structural changes needed** - `ShiftProfile.cs` ready for CreateMap calls when DTOs defined.

### Application Layer - Interfaces
- **No structural changes needed** - `IShiftRepository.cs` needs method signatures mirroring `IMachineRepository.cs`.

### Domain Layer
- **ENHANCE**: `Domain/Entities/Shift.cs`
  - Add private constructor for EF Core materialization
  - Add public constructor with validation (use `Guard`)
  - Add property setter validation
  - Add domain methods (e.g., `Inactivate()`)
  - Mirror `Machine.cs` patterns

### Infrastructure Layer - Repositories
- **REPLACE**: Delete placeholders, create method files:
  1. **DELETE** `Infrastructure/Repositories/Shifts/Commands/ShiftRepository.PlaceholderCommand.cs`
  2. **CREATE** `Infrastructure/Repositories/Shifts/Commands/ShiftRepository.Add.cs`
  3. **CREATE** `Infrastructure/Repositories/Shifts/Commands/ShiftRepository.Inactivate.cs`
  4. **CREATE** `Infrastructure/Repositories/Shifts/Commands/ShiftRepository.SaveChanges.cs`
  5. **DELETE** `Infrastructure/Repositories/Shifts/Queries/ShiftRepository.PlaceholderQuery.cs`
  6. **CREATE** `Infrastructure/Repositories/Shifts/Queries/ShiftRepository.GetById.cs`
  7. **CREATE** `Infrastructure/Repositories/Shifts/Queries/ShiftRepository.ListActive.cs`
  8. **CREATE** `Infrastructure/Repositories/Shifts/Queries/ShiftRepository.ListAll.cs`

### Infrastructure Layer - Persistence
- **No changes needed** - `ShiftConfiguration.cs` is fully configured; DbSet registered in `AppDbContext`.

### Infrastructure Layer - Dependency Injection
- **ADD**: Register repository
  - Update `Infrastructure/DependencyInjection.cs` with `services.AddScoped<IShiftRepository, ShiftRepository>();`

### Application Layer - Dependency Injection
- **ADD**: Register service
  - Update `Application/DependencyInjection.cs` with `services.AddScoped<ShiftService>();`

### Application Tests
- **REPLACE + FIX**:
  1. **UPDATE** `Application.Tests/Services/Shifts/ShiftTests.cs` with shared setup (`Mock<IShiftRepository>`, `Mock<IMapper>`, `ShiftService` init) mirroring `MachineTests.cs`
  2. **DELETE** placeholder command/query test files as real tests are added
  3. **CREATE** `ShiftTests.Create.cs`, `ShiftTests.Inactivate.cs`, `ShiftTests.Get.cs`, `ShiftTests.ListActive.cs`, `ShiftTests.ListAll.cs` (one file per method)

### Domain Tests
- **No structural changes needed** - Implement constructor/setter/method tests using `Shift` invariants once defined.

---

## 6. Shifts Slice "Ready-to-Implement?" Verdict

### ❌ **NOT READY** - Minimum Cleanup Required

**Critical Blockers:**
1. **Missing Dependency Injection Registrations** - `ShiftService` and `IShiftRepository` not registered
2. **Missing Domain Validation** - `Shift.cs` lacks invariants (constructors, guarded setters, domain methods)
3. **Placeholder Files Present** - 6 placeholder files must be deleted and replaced with method-specific files
4. **Missing Test Setup** - `ShiftTests.cs` base file lacks shared mocks and service initialization
5. **SliceMap Gap** - `Shifts` not listed in `SliceMap.md` (add to keep naming/contracts aligned)

**Minimum Cleanup Before Implementation:**
1. ✅ Add DI registrations for `ShiftService` and `IShiftRepository`
2. ✅ Enhance `Shift.cs` with constructors, validation, and domain methods
3. ✅ Add test base setup to `ShiftTests.cs`
4. ⚠️ Replace placeholders with method-specific files during implementation
5. ⚠️ Add `Shifts` entry to `SliceMap.md` for canonical naming/paths

**Recommended Implementation Order:**
1. Fix DI registrations
2. Enhance `Shift` domain entity with validation
3. Add test base setup
4. Replace repository placeholders with method files
5. Replace service placeholders with method files
6. Implement controller actions
7. Implement tests per method file

---

## 7. Golden Reference Pointers

When implementing Shifts slice functionality, mirror these **Machines slice files**:

### Application Services
- Base: `backend/CncApp/CncApp.Application/Services/Machines/MachineService.cs`
- Commands:
  - `backend/CncApp/CncApp.Application/Services/Machines/Commands/MachineService.Create.cs`
  - `backend/CncApp/CncApp.Application/Services/Machines/Commands/MachineService.Inactivate.cs`
- Queries:
  - `backend/CncApp/CncApp.Application/Services/Machines/Queries/MachineService.Get.cs`
  - `backend/CncApp/CncApp.Application/Services/Machines/Queries/MachineService.ListActive.cs`
  - `backend/CncApp/CncApp.Application/Services/Machines/Queries/MachineService.ListAll.cs`

### Infrastructure Repositories
- Base: `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/MachineRepository.cs`
- Commands:
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Commands/MachineRepository.Add.cs`
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Commands/MachineRepository.Inactivate.cs`
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Commands/MachineRepository.SaveChanges.cs`
- Queries:
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Queries/MachineRepository.GetById.cs`
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Queries/MachineRepository.ListActive.cs`
  - `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Queries/MachineRepository.ListAll.cs`

### Domain Entity
- Reference: `backend/CncApp/CncApp.Domain/Entities/Machine.cs`

### Application Tests
- Base: `backend/CncApp/CncApp.Application.Tests/Services/Machines/MachineTests.cs`
- Commands:
  - `backend/CncApp/CncApp.Application.Tests/Services/Machines/Commands/MachineTests.Create.cs`
  - `backend/CncApp/CncApp.Application.Tests/Services/Machines/Commands/MachineTests.Inactivate.cs`
- Queries:
  - `backend/CncApp/CncApp.Application.Tests/Services/Machines/Queries/MachineTests.Get.cs`
  - `backend/CncApp/CncApp.Application.Tests/Services/Machines/Queries/MachineTests.ListActive.cs`
  - `backend/CncApp/CncApp.Application.Tests/Services/Machines/Queries/MachineTests.ListAll.cs`

### API Controller
- Reference: `backend/CncApp/CncApp.Api/Controllers/MachinesController.cs`

### Application Interfaces
- Reference: `backend/CncApp/CncApp.Application/Interfaces/Repositories/IMachineRepository.cs`

### Application Mapping
- Reference: `backend/CncApp/CncApp.Application/Mapping/MachineProfile.cs`

### Domain Tests
- Reference: `backend/CncApp/CncApp.Domain.Tests/Entities/MachineTests.cs`

---

