# Jobs Slice Audit

## 1. Purpose

This audit analyzes the **current scaffolded Jobs slice** (folders + empty/placeholder files) and compares it against the canonical **Machines slice pattern** and **SliceMap rules** to identify:
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
- ✅ `backend/CncApp/CncApp.Api/Controllers/JobsController.cs`
  - Class: `JobsController`
  - Namespace: `CncApp.Api.Controllers`
  - Status: Stub with constructor only, no actions

### Application Layer - DTOs
- ✅ `backend/CncApp/CncApp.Application/Dtos/Jobs/JobDto.cs`
  - Class: `JobDto`
  - Namespace: `CncApp.Application.Dtos.Jobs`
  - Status: Stub with `Id` property, TODO for other properties
- ✅ `backend/CncApp/CncApp.Application/Dtos/Jobs/CreateJobRequestDto.cs`
  - Class: `CreateJobRequestDto`
  - Namespace: `CncApp.Application.Dtos.Jobs`
  - Status: Stub with TODO for properties

### Application Layer - Services
- ✅ `backend/CncApp/CncApp.Application/Services/Jobs/JobService.cs`
  - Class: `JobService` (partial)
  - Namespace: `CncApp.Application.Services.Jobs`
  - Status: Base file with constructor + dependencies (`IJobRepository`, `IMapper`)
- ✅ `backend/CncApp/CncApp.Application/Services/Jobs/Commands/JobService.PlaceholderCommand.cs`
  - Class: `JobService` (partial)
  - Namespace: `CncApp.Application.Services.Jobs` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Application/Services/Jobs/Queries/JobService.PlaceholderQuery.cs`
  - Class: `JobService` (partial)
  - Namespace: `CncApp.Application.Services.Jobs` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Application Layer - Mapping
- ✅ `backend/CncApp/CncApp.Application/Mapping/JobProfile.cs`
  - Class: `JobProfile` : `Profile`
  - Namespace: `CncApp.Application.Mapping`
  - Status: Stub with empty constructor, TODO for CreateMap calls

### Application Layer - Interfaces
- ✅ `backend/CncApp/CncApp.Application/Interfaces/Repositories/IJobRepository.cs`
  - Interface: `IJobRepository`
  - Namespace: `CncApp.Application.Interfaces.Repositories`
  - Status: Stub with TODO for method signatures

### Domain Layer
- ✅ `backend/CncApp/CncApp.Domain/Entities/Job.cs`
  - Class: `Job` : `AuditableEntityBase`
  - Namespace: `CncApp.Domain.Entities`
  - Status: Entity exists with properties, but **lacks domain validation** (no private constructor, no public constructor with validation, no domain methods like `Inactivate()`)

### Infrastructure Layer - Repositories
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Jobs/JobRepository.cs`
  - Class: `JobRepository` (partial) : `IJobRepository`
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct)
  - Status: Base file with constructor + `_context` field
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Jobs/Commands/JobRepository.PlaceholderCommand.cs`
  - Class: `JobRepository` (partial)
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Infrastructure/Repositories/Jobs/Queries/JobRepository.PlaceholderQuery.cs`
  - Class: `JobRepository` (partial)
  - Namespace: `CncApp.Infrastructure.Repositories` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Infrastructure Layer - Persistence
- ✅ `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/JobConfiguration.cs`
  - Class: `JobConfiguration` : `IEntityTypeConfiguration<Job>`
  - Namespace: `CncApp.Infrastructure.Persistence.Configurations`
  - Status: **Fully configured** with primary key, properties, relationships, indexes
- ✅ `backend/CncApp/CncApp.Infrastructure/Persistence/AppDbContext.cs`
  - Contains: `public DbSet<Job> Jobs => Set<Job>();`
  - Status: ✅ Registered

### Application Tests
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Jobs/JobTests.cs`
  - Class: `JobTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Jobs`
  - Status: Stub with TODO comment, **missing shared setup** (mocks, service initialization)
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Jobs/Commands/JobTests.PlaceholderCommand.cs`
  - Class: `JobTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Jobs` ✅ (correct - no `.Commands`)
  - Status: Placeholder file, must be replaced
- ✅ `backend/CncApp/CncApp.Application.Tests/Services/Jobs/Queries/JobTests.PlaceholderQuery.cs`
  - Class: `JobTests` (partial)
  - Namespace: `CncApp.Application.Tests.Services.Jobs` ✅ (correct - no `.Queries`)
  - Status: Placeholder file, must be replaced

### Domain Tests
- ✅ `backend/CncApp/CncApp.Domain.Tests/Entities/JobTests.cs`
  - Class: `JobTests` (not partial)
  - Namespace: `CncApp.Domain.Tests.Entities`
  - Status: Stub with `#region` structure (Constructor Tests, Property Setter Tests, Method Tests), all TODOs

---

## 3. Conformance Check vs Machines

### ✅ Commands/Queries Folder Presence
- **PASS**: `Application/Services/Jobs/Commands/` exists
- **PASS**: `Application/Services/Jobs/Queries/` exists
- **PASS**: `Infrastructure/Repositories/Jobs/Commands/` exists
- **PASS**: `Infrastructure/Repositories/Jobs/Queries/` exists
- **PASS**: `Application.Tests/Services/Jobs/Commands/` exists
- **PASS**: `Application.Tests/Services/Jobs/Queries/` exists

### ✅ Partial Class Split Correctness
- **PASS**: `JobService.cs` is partial (base file)
- **PASS**: `JobService.PlaceholderCommand.cs` is partial
- **PASS**: `JobService.PlaceholderQuery.cs` is partial
- **PASS**: `JobRepository.cs` is partial (base file)
- **PASS**: `JobRepository.PlaceholderCommand.cs` is partial
- **PASS**: `JobRepository.PlaceholderQuery.cs` is partial
- **PASS**: `JobTests.cs` is partial (base file)
- **PASS**: `JobTests.PlaceholderCommand.cs` is partial
- **PASS**: `JobTests.PlaceholderQuery.cs` is partial

### ✅ Namespace Correctness (Prefactored Namespace Correction Applied)
- **PASS**: All `JobService` partials use `namespace CncApp.Application.Services.Jobs;` (no `.Commands`/`.Queries`)
- **PASS**: All `JobRepository` partials use `namespace CncApp.Infrastructure.Repositories;` (no `.Commands`/`.Queries`)
- **PASS**: All `JobTests` partials use `namespace CncApp.Application.Tests.Services.Jobs;` (no `.Commands`/`.Queries`)

### ⚠️ Test Structure Correctness
- **PASS**: Test folder structure mirrors `Application/Services/Jobs/` structure
- **PASS**: Test files are partial classes
- **FAIL**: `JobTests.cs` base file is missing shared setup (mocks, service initialization)
  - **Expected**: Should contain `Mock<IJobRepository>`, `Mock<IMapper>`, `JobService` initialization like `MachineTests.cs`
  - **Current**: Only contains TODO comment

### ⚠️ Domain Entity Correctness
- **FAIL**: `Job.cs` lacks domain validation patterns
  - **Missing**: Private constructor for EF Core materialization
  - **Missing**: Public constructor with validation
  - **Missing**: Domain methods (e.g., `Inactivate()`)
  - **Missing**: Property setter validation using `Guard` class
  - **Current**: Simple POCO with public setters (no domain invariants)

### ❌ Dependency Injection Registration
- **FAIL**: `JobService` not registered in `Application/DependencyInjection.cs`
  - **Expected**: `services.AddScoped<JobService>();`
- **FAIL**: `IJobRepository` not registered in `Infrastructure/DependencyInjection.cs`
  - **Expected**: `services.AddScoped<IJobRepository, JobRepository>();`

---

## 4. Placeholder Inventory

| File Path | Placeholder Type | Why It's Placeholder | What It Should Become |
|-----------|------------------|----------------------|----------------------|
| `Application/Services/Jobs/Commands/JobService.PlaceholderCommand.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `JobService.Create.cs`, `JobService.Inactivate.cs` (one file per method) |
| `Application/Services/Jobs/Queries/JobService.PlaceholderQuery.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `JobService.Get.cs`, `JobService.ListActive.cs`, `JobService.ListAll.cs` (one file per method) |
| `Infrastructure/Repositories/Jobs/Commands/JobRepository.PlaceholderCommand.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `JobRepository.Add.cs`, `JobRepository.Inactivate.cs`, `JobRepository.SaveChanges.cs` (one file per method) |
| `Infrastructure/Repositories/Jobs/Queries/JobRepository.PlaceholderQuery.cs` | Method Placeholder | Empty partial class with TODO | **DELETE** and replace with: `JobRepository.GetById.cs`, `JobRepository.ListActive.cs`, `JobRepository.ListAll.cs` (one file per method) |
| `Application.Tests/Services/Jobs/Commands/JobTests.PlaceholderCommand.cs` | Test Placeholder | Empty partial class with TODO | **DELETE** and replace with: `JobTests.Create.cs`, `JobTests.Inactivate.cs` (one file per method test) |
| `Application.Tests/Services/Jobs/Queries/JobTests.PlaceholderQuery.cs` | Test Placeholder | Empty partial class with TODO | **DELETE** and replace with: `JobTests.Get.cs`, `JobTests.ListActive.cs`, `JobTests.ListAll.cs` (one file per method test) |

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
**No changes needed** - `JobsController.cs` is correctly structured as a stub.

### Application Layer - DTOs
**No changes needed** - DTOs are correctly structured as stubs. Properties need to be added based on `JobConfiguration.cs` and `Job.cs` entity.

### Application Layer - Services
**REPLACE (delete placeholders, create method files):**
1. **DELETE**: `Application/Services/Jobs/Commands/JobService.PlaceholderCommand.cs`
2. **CREATE**: `Application/Services/Jobs/Commands/JobService.Create.cs` (mirror `MachineService.Create.cs`)
3. **CREATE**: `Application/Services/Jobs/Commands/JobService.Inactivate.cs` (mirror `MachineService.Inactivate.cs`)
4. **DELETE**: `Application/Services/Jobs/Queries/JobService.PlaceholderQuery.cs`
5. **CREATE**: `Application/Services/Jobs/Queries/JobService.Get.cs` (mirror `MachineService.Get.cs`)
6. **CREATE**: `Application/Services/Jobs/Queries/JobService.ListActive.cs` (mirror `MachineService.ListActive.cs`)
7. **CREATE**: `Application/Services/Jobs/Queries/JobService.ListAll.cs` (mirror `MachineService.ListAll.cs`)

### Application Layer - Mapping
**No changes needed** - `JobProfile.cs` is correctly structured as a stub. CreateMap calls need to be added when implementing.

### Application Layer - Interfaces
**No changes needed** - `IJobRepository.cs` is correctly structured as a stub. Method signatures need to be added (mirror `IMachineRepository.cs`).

### Domain Layer
**ENHANCE (add domain validation):**
1. **UPDATE**: `Domain/Entities/Job.cs`
   - Add private constructor for EF Core materialization
   - Add public constructor with validation (using `Guard` class)
   - Add property setter validation
   - Add domain methods (e.g., `Inactivate()` method)
   - Mirror `Machine.cs` domain validation patterns

### Infrastructure Layer - Repositories
**REPLACE (delete placeholders, create method files):**
1. **DELETE**: `Infrastructure/Repositories/Jobs/Commands/JobRepository.PlaceholderCommand.cs`
2. **CREATE**: `Infrastructure/Repositories/Jobs/Commands/JobRepository.Add.cs` (mirror `MachineRepository.Add.cs`)
3. **CREATE**: `Infrastructure/Repositories/Jobs/Commands/JobRepository.Inactivate.cs` (mirror `MachineRepository.Inactivate.cs`)
4. **CREATE**: `Infrastructure/Repositories/Jobs/Commands/JobRepository.SaveChanges.cs` (mirror `MachineRepository.SaveChanges.cs`)
5. **DELETE**: `Infrastructure/Repositories/Jobs/Queries/JobRepository.PlaceholderQuery.cs`
6. **CREATE**: `Infrastructure/Repositories/Jobs/Queries/JobRepository.GetById.cs` (mirror `MachineRepository.GetById.cs`)
7. **CREATE**: `Infrastructure/Repositories/Jobs/Queries/JobRepository.ListActive.cs` (mirror `MachineRepository.ListActive.cs`)
8. **CREATE**: `Infrastructure/Repositories/Jobs/Queries/JobRepository.ListAll.cs` (mirror `MachineRepository.ListAll.cs`)

### Infrastructure Layer - Persistence
**No changes needed** - `JobConfiguration.cs` is fully configured and correct.

### Infrastructure Layer - Dependency Injection
**ADD (register repository):**
1. **UPDATE**: `Infrastructure/DependencyInjection.cs`
   - Add: `services.AddScoped<IJobRepository, JobRepository>();`

### Application Layer - Dependency Injection
**ADD (register service):**
1. **UPDATE**: `Application/DependencyInjection.cs`
   - Add: `services.AddScoped<JobService>();`

### Application Tests
**REPLACE (delete placeholders, create method test files) AND FIX (add shared setup):**
1. **UPDATE**: `Application.Tests/Services/Jobs/JobTests.cs`
   - Add shared setup: `Mock<IJobRepository>`, `Mock<IMapper>`, `JobService` initialization (mirror `MachineTests.cs`)
2. **DELETE**: `Application.Tests/Services/Jobs/Commands/JobTests.PlaceholderCommand.cs`
3. **CREATE**: `Application.Tests/Services/Jobs/Commands/JobTests.Create.cs` (mirror `MachineTests.Create.cs`)
4. **CREATE**: `Application.Tests/Services/Jobs/Commands/JobTests.Inactivate.cs` (mirror `MachineTests.Inactivate.cs`)
5. **DELETE**: `Application.Tests/Services/Jobs/Queries/JobTests.PlaceholderQuery.cs`
6. **CREATE**: `Application.Tests/Services/Jobs/Queries/JobTests.Get.cs` (mirror `MachineTests.Get.cs`)
7. **CREATE**: `Application.Tests/Services/Jobs/Queries/JobTests.ListActive.cs` (mirror `MachineTests.ListActive.cs`)
8. **CREATE**: `Application.Tests/Services/Jobs/Queries/JobTests.ListAll.cs` (mirror `MachineTests.ListAll.cs`)

### Domain Tests
**No changes needed** - `JobTests.cs` is correctly structured with `#region` organization. Tests need to be implemented.

---

## 6. Jobs Slice "Ready-to-Implement?" Verdict

### ❌ **NOT READY** - Minimum Cleanup Required

**Critical Blockers:**
1. **Missing Dependency Injection Registrations** - Service and repository not registered
2. **Missing Domain Validation** - `Job.cs` lacks domain invariants (no private constructor, no validation, no domain methods)
3. **Placeholder Files Present** - 6 placeholder files must be deleted and replaced with method-specific files
4. **Missing Test Setup** - `JobTests.cs` base file lacks shared mocks and service initialization

**Minimum Cleanup Before Implementation:**
1. ✅ **Fix Dependency Injection** (5 minutes)
   - Add `JobService` registration in `Application/DependencyInjection.cs`
   - Add `IJobRepository` registration in `Infrastructure/DependencyInjection.cs`
2. ✅ **Enhance Domain Entity** (30 minutes)
   - Add private constructor for EF Core
   - Add public constructor with validation
   - Add property setter validation
   - Add `Inactivate()` domain method
3. ✅ **Add Test Base Setup** (10 minutes)
   - Add mocks and service initialization to `JobTests.cs`
4. ⚠️ **Replace Placeholders** (can be done incrementally during implementation)
   - Delete placeholder files as you create real method files
   - Create method files one at a time as you implement functionality

**Recommended Implementation Order:**
1. Fix Dependency Injection registrations
2. Enhance `Job.cs` domain entity with validation
3. Add test base setup to `JobTests.cs`
4. Implement repository methods (delete placeholders, create method files)
5. Implement service methods (delete placeholders, create method files)
6. Implement controller actions
7. Implement tests (delete placeholders, create method test files)

---

## 7. Golden Reference Pointers

When implementing Jobs slice functionality, mirror these **Machines slice files exactly**:

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

**Verdict:** Jobs slice is **NOT ready** for implementation without the critical blockers being fixed first. Once fixed, it can be implemented incrementally by replacing placeholders with real method files as functionality is added.

