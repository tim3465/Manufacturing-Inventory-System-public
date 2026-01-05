# StockLots Slice Audit (Phase 1 - Read-Only)

**Date:** 2025-01-XX  
**Status:** Phase 1 - Read-Only Audit  
**Reference:** Machines slice (golden reference)

---

## Executive Summary

The StockLot slice is in **scaffold state** with basic structure in place but **not ready for implementation**. All core files exist but contain placeholders, TODOs, or incomplete implementations. The slice requires structural cleanup (Phase 2) before backend implementation (Phase 3) can begin.

**Verdict:** ❌ **NOT Ready-to-Implement**  
**Blockers:**
1. Placeholder files must be replaced with correctly named method files
2. Domain entity lacks invariants, constructors, and domain methods
3. DTOs are incomplete (TODOs only)
4. Repository interface is empty (TODOs only)
5. Mapping profile is empty (TODOs only)
6. DI registrations missing
7. Application tests base lacks shared setup (mocks)

---

## Full Path Listing Per Layer

### API Layer (`CncApp.Api`)

#### ✅ Controller Exists
- **Path:** `backend/CncApp/CncApp.Api/Controllers/StockLotsController.cs`
- **Class:** `StockLotsController` ✅ (correct plural)
- **Namespace:** `CncApp.Api.Controllers` ✅ (correct)
- **Route:** `[Route("api/[controller]")]` ✅ (correct)
- **Dependency Injection:** Injects `StockLotService` (concrete) ✅ (correct)
- **Status:** ❌ **Incomplete** - Contains only constructor and TODO comment
- **Missing:** All endpoint methods (`CreateAsync`, `GetAsync`, `ListAsync`, `ListAllAsync`, `DeleteAsync`)

---

### Application Layer (`CncApp.Application`)

#### DTOs

##### ✅ StockLotDto Exists
- **Path:** `backend/CncApp/CncApp.Application/Dtos/StockLots/StockLotDto.cs`
- **Namespace:** `CncApp.Application.Dtos.StockLots` ✅ (correct)
- **Status:** ❌ **Incomplete** - Contains only `Id` property and TODOs
- **Missing Properties:**
  - `LotNumber` (string, required, max 100)
  - `MaterialId` (int, required)
  - `AmountOfBars` (int, required)
  - `Diameter` (decimal, precision 18,4)
  - `BarLength` (decimal, precision 18,4)
  - `Condition` (StockLotConditionEnum, required)
  - `CheckedInDateTime` (DateTime, required)
  - DataAnnotations validation attributes

##### ✅ CreateStockLotRequestDto Exists
- **Path:** `backend/CncApp/CncApp.Application/Dtos/StockLots/CreateStockLotRequestDto.cs`
- **Namespace:** `CncApp.Application.Dtos.StockLots` ✅ (correct)
- **Status:** ❌ **Incomplete** - Contains only TODOs
- **Missing Properties:**
  - `LotNumber` (string, required, max 100)
  - `MaterialId` (int, required)
  - `AmountOfBars` (int, required, non-negative)
  - `Diameter` (decimal, required, precision 18,4)
  - `BarLength` (decimal, required, precision 18,4)
  - `Condition` (StockLotConditionEnum, required)
  - `CheckedInDateTime` (DateTime, required)
  - DataAnnotations validation attributes

#### Services

##### ✅ Service Base Exists
- **Path:** `backend/CncApp/CncApp.Application/Services/StockLots/StockLotService.cs`
- **Class:** `StockLotService` (partial) ✅ (correct singular)
- **Namespace:** `CncApp.Application.Services.StockLots` ✅ (correct)
- **Dependencies:** `IStockLotRepository`, `IMapper` ✅ (correct)
- **Status:** ✅ **Complete** - Constructor and dependencies match Machines pattern

##### ❌ Placeholder Command File
- **Path:** `backend/CncApp/CncApp.Application/Services/StockLots/Commands/StockLotService.PlaceholderCommand.cs`
- **Namespace:** `CncApp.Application.Services.StockLots` ✅ (correct - no `.Commands` in namespace)
- **Status:** ❌ **Placeholder** - Must be deleted and replaced with:
  - `StockLotService.Create.cs`
  - `StockLotService.Inactivate.cs`

##### ❌ Placeholder Query File
- **Path:** `backend/CncApp/CncApp.Application/Services/StockLots/Queries/StockLotService.PlaceholderQuery.cs`
- **Namespace:** `CncApp.Application.Services.StockLots` ✅ (correct - no `.Queries` in namespace)
- **Status:** ❌ **Placeholder** - Must be deleted and replaced with:
  - `StockLotService.Get.cs`
  - `StockLotService.ListActive.cs`
  - `StockLotService.ListAll.cs`

#### Mapping

##### ✅ Mapping Profile Exists
- **Path:** `backend/CncApp/CncApp.Application/Mapping/StockLotProfile.cs`
- **Class:** `StockLotProfile : Profile` ✅ (correct)
- **Namespace:** `CncApp.Application.Mapping` ✅ (correct)
- **Status:** ❌ **Incomplete** - Contains only empty constructor with TODOs
- **Missing:** `CreateMap` calls for:
  - `StockLot` → `StockLotDto`
  - `CreateStockLotRequestDto` → `StockLot`

#### Interfaces

##### ✅ Repository Interface Exists
- **Path:** `backend/CncApp/CncApp.Application/Interfaces/Repositories/IStockLotRepository.cs`
- **Namespace:** `CncApp.Application.Interfaces.Repositories` ✅ (correct)
- **Status:** ❌ **Incomplete** - Contains only TODOs, no method signatures
- **Missing Methods:**
  - `Task<StockLot?> GetByIdAsync(int id, CancellationToken ct = default);`
  - `Task<List<StockLot>> ListActiveAsync(CancellationToken ct = default);`
  - `Task<List<StockLot>> ListAllAsync(CancellationToken ct = default);`
  - `Task AddAsync(StockLot stockLot, CancellationToken ct = default);`
  - `Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);`
  - `Task SaveChangesAsync(CancellationToken ct = default);`

#### Dependency Injection

##### ❌ Missing DI Registration
- **Path:** `backend/CncApp/CncApp.Application/DependencyInjection.cs`
- **Status:** ❌ **Missing** - No registration found for `StockLotService`
- **Required:** `services.AddScoped<StockLotService>();`

---

### Domain Layer (`CncApp.Domain`)

#### ❌ Entity Incomplete
- **Path:** `backend/CncApp/CncApp.Domain/Entities/StockLot.cs`
- **Class:** `StockLot : AuditableEntityBase` ✅ (correct inheritance)
- **Namespace:** `CncApp.Domain.Entities` ✅ (correct)
- **Status:** ❌ **Incomplete** - Missing domain invariants and methods
- **Current State:** Public auto-properties only (no validation, no constructors)
- **Missing:**
  1. **Private constructor** for EF Core materialization
  2. **Public constructor** with validation for domain creation
  3. **Property setter validation** (using backing fields and Guard methods)
  4. **Domain method:** `Inactivate(int? inactivatedByUserId = null)`
  5. **Invariant enforcement** (e.g., non-negative AmountOfBars, required fields)

**Comparison to Machine.cs:**
- Machine has private constructor, public constructor, property setters with validation, and `Inactivate()` method
- StockLot has none of these patterns

---

### Infrastructure Layer (`CncApp.Infrastructure`)

#### Repositories

##### ✅ Repository Base Exists
- **Path:** `backend/CncApp/CncApp.Infrastructure/Repositories/StockLots/StockLotRepository.cs`
- **Class:** `StockLotRepository` (partial) ✅ (correct singular)
- **Implements:** `IStockLotRepository` ✅ (correct)
- **Namespace:** `CncApp.Infrastructure.Repositories` ✅ (correct)
- **Dependency:** `AppDbContext` ✅ (correct)
- **Status:** ✅ **Complete** - Constructor and `_context` field match Machines pattern

##### ❌ Placeholder Command File
- **Path:** `backend/CncApp/CncApp.Infrastructure/Repositories/StockLots/Commands/StockLotRepository.PlaceholderCommand.cs`
- **Namespace:** `CncApp.Infrastructure.Repositories` ✅ (correct - no `.Commands` in namespace)
- **Status:** ❌ **Placeholder** - Must be deleted and replaced with:
  - `StockLotRepository.Add.cs`
  - `StockLotRepository.Inactivate.cs`
  - `StockLotRepository.SaveChanges.cs`

##### ❌ Placeholder Query File
- **Path:** `backend/CncApp/CncApp.Infrastructure/Repositories/StockLots/Queries/StockLotRepository.PlaceholderQuery.cs`
- **Namespace:** `CncApp.Infrastructure.Repositories` ✅ (correct - no `.Queries` in namespace)
- **Status:** ❌ **Placeholder** - Must be deleted and replaced with:
  - `StockLotRepository.GetById.cs`
  - `StockLotRepository.ListActive.cs`
  - `StockLotRepository.ListAll.cs`

#### Persistence

##### ✅ EF Configuration Exists
- **Path:** `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/StockLotConfiguration.cs`
- **Class:** `StockLotConfiguration : IEntityTypeConfiguration<StockLot>` ✅ (correct)
- **Namespace:** `CncApp.Infrastructure.Persistence.Configurations` ✅ (correct)
- **Status:** ✅ **Complete** - Configuration appears complete with:
  - Primary key
  - Required properties
  - Max lengths
  - Precision for decimals
  - Relationships (Material, StockLotAdjustments)
  - Indexes

##### ✅ DbSet Registered
- **Path:** `backend/CncApp/CncApp.Infrastructure/Persistence/AppDbContext.cs`
- **Status:** ✅ **Complete** - `public DbSet<StockLot> StockLots => Set<StockLot>();` exists

#### Dependency Injection

##### ❌ Missing DI Registration
- **Path:** `backend/CncApp/CncApp.Infrastructure/DependencyInjection.cs`
- **Status:** ❌ **Missing** - No registration found for `IStockLotRepository` → `StockLotRepository`
- **Required:** `services.AddScoped<IStockLotRepository, StockLotRepository>();`

---

### Tests

#### Application Tests (`CncApp.Application.Tests`)

##### ⚠️ Test Base Incomplete
- **Path:** `backend/CncApp/CncApp.Application.Tests/Services/StockLots/StockLotTests.cs`
- **Class:** `StockLotTests` (partial) ✅ (correct plural)
- **Namespace:** `CncApp.Application.Tests.Services.StockLots` ✅ (correct)
- **Status:** ⚠️ **Incomplete** - Contains only comment, missing shared setup
- **Missing:**
  - `Mock<IStockLotRepository> MockRepository` field
  - `Mock<IMapper> MockMapper` field
  - `StockLotService StockLotService` field
  - Constructor that initializes mocks and service

**Comparison to MachineTests.cs:**
- MachineTests has all mocks and service initialization in constructor
- StockLotTests has none of these

##### ❌ Placeholder Command Test File
- **Path:** `backend/CncApp/CncApp.Application.Tests/Services/StockLots/Commands/StockLotTests.PlaceholderCommand.cs`
- **Namespace:** `CncApp.Application.Tests.Services.StockLots` ✅ (correct - no `.Commands` in namespace)
- **Status:** ❌ **Placeholder** - Must be deleted and replaced with:
  - `StockLotTests.Create.cs`
  - `StockLotTests.Inactivate.cs`

##### ❌ Placeholder Query Test File
- **Path:** `backend/CncApp/CncApp.Application.Tests/Services/StockLots/Queries/StockLotTests.PlaceholderQuery.cs`
- **Namespace:** `CncApp.Application.Tests.Services.StockLots` ✅ (correct - no `.Queries` in namespace)
- **Status:** ❌ **Placeholder** - Must be deleted and replaced with:
  - `StockLotTests.Get.cs`
  - `StockLotTests.ListActive.cs`
  - `StockLotTests.ListAll.cs`

#### Domain Tests (`CncApp.Domain.Tests`)

##### ✅ Domain Test File Exists
- **Path:** `backend/CncApp/CncApp.Domain.Tests/Entities/StockLotTests.cs`
- **Class:** `StockLotTests` ✅ (correct plural, not partial)
- **Namespace:** `CncApp.Domain.Tests.Entities` ✅ (correct)
- **Structure:** Uses `#region` for organization ✅ (correct)
- **Status:** ❌ **Incomplete** - Contains only TODOs in all regions
- **Missing Tests:**
  - Constructor Tests region: validation tests for public constructor
  - Property Setter Tests region: validation tests for property setters
  - Method Tests region: tests for `Inactivate()` method (once implemented)

---

## Placeholder Inventory

### Files to Delete and Replace

#### Application Services
1. ❌ `Application/Services/StockLots/Commands/StockLotService.PlaceholderCommand.cs`
   - Replace with: `StockLotService.Create.cs`, `StockLotService.Inactivate.cs`

2. ❌ `Application/Services/StockLots/Queries/StockLotService.PlaceholderQuery.cs`
   - Replace with: `StockLotService.Get.cs`, `StockLotService.ListActive.cs`, `StockLotService.ListAll.cs`

#### Infrastructure Repositories
3. ❌ `Infrastructure/Repositories/StockLots/Commands/StockLotRepository.PlaceholderCommand.cs`
   - Replace with: `StockLotRepository.Add.cs`, `StockLotRepository.Inactivate.cs`, `StockLotRepository.SaveChanges.cs`

4. ❌ `Infrastructure/Repositories/StockLots/Queries/StockLotRepository.PlaceholderQuery.cs`
   - Replace with: `StockLotRepository.GetById.cs`, `StockLotRepository.ListActive.cs`, `StockLotRepository.ListAll.cs`

#### Application Tests
5. ❌ `Application.Tests/Services/StockLots/Commands/StockLotTests.PlaceholderCommand.cs`
   - Replace with: `StockLotTests.Create.cs`, `StockLotTests.Inactivate.cs`

6. ❌ `Application.Tests/Services/StockLots/Queries/StockLotTests.PlaceholderQuery.cs`
   - Replace with: `StockLotTests.Get.cs`, `StockLotTests.ListActive.cs`, `StockLotTests.ListAll.cs`

**Total Placeholder Files:** 6

---

## Conformance Check vs Machines + SliceMap

### ✅ Conforming Elements

1. **Folder Structure:**
   - ✅ `Services/StockLots/` (plural folder)
   - ✅ `Repositories/StockLots/` (plural folder)
   - ✅ `Dtos/StockLots/` (plural folder)
   - ✅ `Commands/` and `Queries/` subfolders exist

2. **Naming Conventions:**
   - ✅ `StockLotService` (singular class in plural folder)
   - ✅ `StockLotRepository` (singular class in plural folder)
   - ✅ `StockLotsController` (plural controller)
   - ✅ `StockLotDto` (singular DTO)
   - ✅ `CreateStockLotRequestDto` (Create prefix pattern)

3. **Namespace Patterns:**
   - ✅ All partial class files use correct namespaces (no `.Commands` or `.Queries` in namespace)
   - ✅ Service namespace: `CncApp.Application.Services.StockLots`
   - ✅ Repository namespace: `CncApp.Infrastructure.Repositories`
   - ✅ Test namespace: `CncApp.Application.Tests.Services.StockLots`

4. **Partial Class Pattern:**
   - ✅ Service base file exists
   - ✅ Repository base file exists
   - ✅ Test base file exists
   - ✅ All partials marked correctly

5. **EF Configuration:**
   - ✅ Configuration file exists and appears complete
   - ✅ DbSet registered in AppDbContext

### ❌ Non-Conforming Elements

1. **Domain Entity:**
   - ❌ Missing private constructor for EF Core
   - ❌ Missing public constructor with validation
   - ❌ Missing property setter validation (backing fields + Guard)
   - ❌ Missing `Inactivate()` domain method
   - ❌ No invariant enforcement

2. **DTOs:**
   - ❌ Incomplete properties (TODOs only)
   - ❌ Missing DataAnnotations validation

3. **Repository Interface:**
   - ❌ Empty (TODOs only, no method signatures)

4. **Mapping Profile:**
   - ❌ Empty (TODOs only, no CreateMap calls)

5. **Service Methods:**
   - ❌ Placeholder files instead of real method files

6. **Repository Methods:**
   - ❌ Placeholder files instead of real method files

7. **Test Setup:**
   - ❌ Application test base missing mocks and service initialization

8. **Dependency Injection:**
   - ❌ Missing service registration in Application layer
   - ❌ Missing repository registration in Infrastructure layer

9. **Controller:**
   - ❌ Empty (TODOs only, no endpoints)

10. **Domain Tests:**
    - ❌ Empty (TODOs only, no actual tests)

---

## Ready-to-Implement? Verdict

### ❌ **NOT Ready-to-Implement**

**Blockers (must be resolved in Phase 2):**

1. **Placeholder Files (6 files):** Must be deleted and replaced with correctly named method files (empty skeletons OK)
2. **Domain Entity:** Must implement invariants, constructors, and domain methods
3. **DTOs:** Must be completed with all properties and validation
4. **Repository Interface:** Must define all method signatures
5. **Mapping Profile:** Must define all CreateMap calls
6. **DI Registrations:** Must add service and repository registrations
7. **Application Tests Base:** Must add shared setup (mocks, service initialization)
8. **Controller:** Must implement endpoints (can be empty skeletons in Phase 2)

**After Phase 2 Cleanup:**
- All placeholder files replaced
- All namespaces correct
- All DI registrations in place
- Domain entity has structure (even if methods are TODO)
- DTOs have all properties (even if validation is TODO)
- Repository interface has all signatures (even if implementations are TODO)
- Mapping profile has all CreateMap calls (even if mappings are TODO)
- Application tests base has shared setup
- Controller has endpoint skeletons

**Then Phase 3 (Implementation) can begin.**

---

## Summary Statistics

| Category | Status | Count |
|----------|--------|-------|
| **Files Exists** | ✅ | 18 |
| **Files Complete** | ✅ | 4 (Service base, Repository base, EF Config, DbSet) |
| **Files Incomplete** | ❌ | 14 |
| **Placeholder Files** | ❌ | 6 |
| **Missing DI Registrations** | ❌ | 2 |
| **Missing Domain Invariants** | ❌ | 1 (StockLot entity) |
| **Missing Test Setup** | ❌ | 1 (Application test base) |

---

## Next Steps (Phase 2 - Structural Cleanup)

1. Delete 6 placeholder files
2. Create 13 correctly named method files (empty skeletons OK):
   - Service: 5 files (Create, Inactivate, Get, ListActive, ListAll)
   - Repository: 6 files (Add, Inactivate, SaveChanges, GetById, ListActive, ListAll)
   - Tests: 5 files (Create, Inactivate, Get, ListActive, ListAll)
3. Complete DTOs with all properties
4. Complete repository interface with all method signatures
5. Complete mapping profile with CreateMap calls
6. Add DI registrations (Application + Infrastructure)
7. Add shared setup to Application test base
8. Add endpoint skeletons to controller
9. Verify compilation succeeds

---

## Reference Files (Machines Slice)

For comparison, reference these Machines slice files:
- `Domain/Entities/Machine.cs` - Domain entity pattern
- `Application/Services/Machines/MachineService.cs` - Service base
- `Application/Services/Machines/Commands/MachineService.Create.cs` - Command method
- `Application/Services/Machines/Queries/MachineService.Get.cs` - Query method
- `Infrastructure/Repositories/Machines/MachineRepository.cs` - Repository base
- `Infrastructure/Repositories/Machines/Commands/MachineRepository.Add.cs` - Repository command
- `Infrastructure/Repositories/Machines/Queries/MachineRepository.GetById.cs` - Repository query
- `Application.Tests/Services/Machines/MachineTests.cs` - Test base with mocks
- `Application.Tests/Services/Machines/Commands/MachineTests.Create.cs` - Test method file

---

**End of Audit**

