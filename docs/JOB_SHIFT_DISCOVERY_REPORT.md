# Job and Shift Discovery Report

## Executive Summary

This report documents the current state of **Job** and **Shift** entities across the Application, Infrastructure, and API layers. Both entities exist in the Domain and Infrastructure layers but have **no Application layer services, DTOs, mappings, or API controllers** implemented yet.

---

## 1. Domain Entities

### Job Entity
**Location:** `backend/CncApp/CncApp.Domain/Entities/Job.cs`

**Properties:**
- `Id` (int) - Inherited from `EntityBase`
- `OrderId` (int, required)
- `StockLotId` (int, required)
- `MachineId` (int, required)
- `PartAmountPlanned` (int, required)
- `BarAmountPlanned` (int, required)
- `BarCycleTime` (TimeSpan, required)
- `Shifts` (ICollection<Shift>, navigation property)

**Audit Fields (from `AuditableEntityBase`):**
- `CreatedDateTime` (DateTimeOffset)
- `CreatedByUserId` (int?)
- `UpdatedDateTime` (DateTimeOffset?)
- `UpdatedByUserId` (int?)
- `InactivatedDateTime` (DateTimeOffset?)
- `InactivatedByUserId` (int?)

**Navigation Properties:**
- `Order` (Order, required)
- `StockLot` (StockLot, required)
- `Machine` (Machine, required)
- `Shifts` (ICollection<Shift>)

**Relationships:**
- One-to-Many with `Order` (Cascade delete)
- One-to-Many with `StockLot` (Cascade delete)
- One-to-Many with `Machine` (Cascade delete)
- One-to-Many with `Shift` (Cascade delete)

### Shift Entity
**Location:** `backend/CncApp/CncApp.Domain/Entities/Shift.cs`

**Properties:**
- `Id` (int) - Inherited from `EntityBase`
- `JobId` (int, required)
- `OperatorId` (int, required)
- `PartsMade` (int)
- `Scrap` (int)
- `StartTime` (DateTime, required)
- `StopTime` (DateTime?, nullable)
- `Downtime` (TimeSpan?, nullable)

**Audit Fields (from `AuditableEntityBase`):**
- `CreatedDateTime` (DateTimeOffset)
- `CreatedByUserId` (int?)
- `UpdatedDateTime` (DateTimeOffset?)
- `UpdatedByUserId` (int?)
- `InactivatedDateTime` (DateTimeOffset?)
- `InactivatedByUserId` (int?)

**Navigation Properties:**
- `Job` (Job, required)
- `Operator` (User, required)

**Relationships:**
- Many-to-One with `Job` (Cascade delete)
- Many-to-One with `User` (Restrict delete - prevents deletion of user with shifts)

---

## 2. Infrastructure Layer

### Database Configuration

#### JobConfiguration
**Location:** `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/JobConfiguration.cs`

**Column Definitions:**
- `Id` (int, Primary Key, Identity)
- `OrderId` (int, Required, Indexed)
- `StockLotId` (int, Required, Indexed)
- `MachineId` (int, Required, Indexed)
- `PartAmountPlanned` (int, Required)
- `BarAmountPlanned` (int, Required)
- `BarCycleTime` (TimeSpan, Required)
- Audit columns: `CreatedDateTime`, `CreatedByUserId`, `UpdatedDateTime`, `UpdatedByUserId`, `InactivatedDateTime`, `InactivatedByUserId`

**Indexes:**
- `IX_Jobs_OrderId`
- `IX_Jobs_StockLotId`
- `IX_Jobs_MachineId`

#### ShiftConfiguration
**Location:** `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/ShiftConfiguration.cs`

**Column Definitions:**
- `Id` (int, Primary Key, Identity)
- `JobId` (int, Required, Indexed)
- `OperatorId` (int, Required, Indexed)
- `PartsMade` (int)
- `Scrap` (int)
- `StartTime` (DateTime, Required)
- `StopTime` (DateTime?, Nullable)
- `Downtime` (TimeSpan?, Nullable)
- Audit columns: `CreatedDateTime`, `CreatedByUserId`, `UpdatedDateTime`, `UpdatedByUserId`, `InactivatedDateTime`, `InactivatedByUserId`

**Indexes:**
- `IX_Shifts_JobId`
- `IX_Shifts_OperatorId`

### Database Context
**Location:** `backend/CncApp/CncApp.Infrastructure/Persistence/AppDbContext.cs`

**DbSets:**
- `public DbSet<Job> Jobs => Set<Job>();` (line 47)
- `public DbSet<Shift> Shifts => Set<Shift>();` (line 48)

**Audit Handling:**
- `AppDbContext.SaveChangesAsync()` automatically populates audit fields for both `Job` and `Shift` entities via `AuditableEntityBase` processing.

### Migrations
**Initial Migration:** `20251231204924_InitialCreate.cs`

**Job Table Schema:**
```sql
CREATE TABLE Jobs (
    Id int IDENTITY(1,1) PRIMARY KEY,
    OrderId int NOT NULL,
    StockLotId int NOT NULL,
    MachineId int NOT NULL,
    PartAmountPlanned int NOT NULL,
    BarAmountPlanned int NOT NULL,
    BarCycleTime time NOT NULL,
    CreatedDateTime datetimeoffset NOT NULL,
    CreatedByUserId int NULL,
    UpdatedDateTime datetimeoffset NULL,
    UpdatedByUserId int NULL,
    InactivatedDateTime datetimeoffset NULL,
    InactivatedByUserId int NULL
)
```

**Shift Table Schema:**
```sql
CREATE TABLE Shifts (
    Id int IDENTITY(1,1) PRIMARY KEY,
    JobId int NOT NULL,
    OperatorId int NOT NULL,
    PartsMade int NOT NULL,
    Scrap int NOT NULL,
    StartTime datetime2 NOT NULL,
    StopTime datetime2 NULL,
    Downtime time NULL,
    CreatedDateTime datetimeoffset NOT NULL,
    CreatedByUserId int NULL,
    UpdatedDateTime datetimeoffset NULL,
    UpdatedByUserId int NULL,
    InactivatedDateTime datetimeoffset NULL,
    InactivatedByUserId int NULL
)
```

### Repositories
**Status:** ❌ **NO REPOSITORIES EXIST**

No `IJobRepository` or `IShiftRepository` interfaces or implementations found.

**Pattern to Follow:**
- Interface: `CncApp.Application/Interfaces/Repositories/IMachineRepository.cs`
- Implementation: `CncApp.Infrastructure/Repositories/Machines/` (partial class with Commands/Queries folders)

---

## 3. Application Layer

### DTOs
**Status:** ❌ **NO DTOS EXIST**

No DTOs found for Job or Shift:
- No `JobDto.cs`
- No `ShiftDto.cs`
- No `CreateJobRequestDto.cs`
- No `UpdateJobRequestDto.cs`
- No `CreateShiftRequestDto.cs`
- No `UpdateShiftRequestDto.cs`

**Expected Location:** `backend/CncApp/CncApp.Application/Dtos/Jobs/` and `backend/CncApp/CncApp.Application/Dtos/Shifts/`

**Pattern to Follow:**
- `CncApp.Application/Dtos/Machines/MachineDto.cs`
- `CncApp.Application/Dtos/Machines/CreateMachineRequestDto.cs`

### AutoMapper Profiles
**Status:** ❌ **NO MAPPINGS EXIST**

No AutoMapper profiles for Job or Shift entities.

**Expected Location:** `backend/CncApp/CncApp.Application/Mapping/JobProfile.cs` and `ShiftProfile.cs`

**Pattern to Follow:**
- `CncApp.Application/Mapping/MachineProfile.cs` (maps `Machine` ↔ `MachineDto` and `CreateMachineRequestDto` → `Machine`)

### Services
**Status:** ❌ **NO SERVICES EXIST**

No service classes found for Job or Shift:
- No `JobService.cs`
- No `ShiftService.cs`

**Expected Location:** `backend/CncApp/CncApp.Application/Services/Jobs/` and `backend/CncApp/CncApp.Application/Services/Shifts/`

**Pattern to Follow:**
- `CncApp.Application/Services/Machines/MachineService.cs` (partial class)
  - Commands: `Create.cs`, `Inactivate.cs`
  - Queries: `Get.cs`, `ListActive.cs`, `ListAll.cs`

### Repository Interfaces
**Status:** ❌ **NO INTERFACES EXIST**

No repository interfaces for Job or Shift:
- No `IJobRepository.cs`
- No `IShiftRepository.cs`

**Expected Location:** `backend/CncApp/CncApp.Application/Interfaces/Repositories/`

**Pattern to Follow:**
- `CncApp.Application/Interfaces/Repositories/IMachineRepository.cs`

---

## 4. API Layer

### Controllers
**Status:** ❌ **NO CONTROLLERS EXIST**

No API controllers found for Job or Shift:
- No `JobsController.cs`
- No `ShiftsController.cs`

**Expected Location:** `backend/CncApp/CncApp.Api/Controllers/`

**Pattern to Follow:**
- `CncApp.Api/Controllers/MachinesController.cs` (CRUD operations with authorization)

### API DTOs
**Status:** ❌ **NO API DTOS EXIST**

No API-specific DTOs found (though Application DTOs are typically used directly).

**Expected Location:** `backend/CncApp/CncApp.Api/ApiDtos/` (if needed, though Application DTOs are typically sufficient)

---

## 5. Queries with Explicit Column Lists

**Status:** ✅ **NO EXPLICIT COLUMN SELECTIONS FOUND**

No queries found that explicitly select specific columns from Job or Shift tables. The codebase uses:
- Entity Framework Core with full entity materialization
- No raw SQL queries with explicit column lists
- No `.Select()` projections that would break with new columns

**Implications:**
- Adding new columns to Job or Shift entities will **NOT** break existing queries
- EF Core will automatically include new columns in queries
- No migration of existing queries required

**Example Pattern (from AppDbContext):**
```csharp
var domainUserId = await DomainUsers
    .AsNoTracking()
    .Where(u => u.IdentityUserId == identityUserId)
    .Select(u => (int?)u.Id)
    .SingleOrDefaultAsync(ct);
```
This is the only explicit `.Select()` found, and it's for User entities, not Job/Shift.

---

## 6. Create/Update Flows

### Current Status
**Status:** ❌ **NO CREATE/UPDATE FLOWS EXIST**

No create or update flows exist for Job or Shift entities.

### Where to Implement Defaults for New Fields

When adding new fields to Job or Shift, defaults should be set in the following locations:

#### For Job Entity:
1. **Domain Entity Constructor** (if adding constructor):
   - `backend/CncApp/CncApp.Domain/Entities/Job.cs`
   - Set default values in property initializers or constructor

2. **Infrastructure Configuration** (if field has database constraints):
   - `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/JobConfiguration.cs`
   - Set `.HasDefaultValue()` or `.HasDefaultValueSql()` if needed

3. **Application Service Create Method** (when implemented):
   - `backend/CncApp/CncApp.Application/Services/Jobs/Commands/JobService.Create.cs`
   - Set defaults after mapping from DTO, before saving

4. **AutoMapper Profile** (when implemented):
   - `backend/CncApp/CncApp.Application/Mapping/JobProfile.cs`
   - Use `.ForMember(dest => dest.NewField, opt => opt.MapFrom(src => defaultValue))` for DTO → Entity mapping

#### For Shift Entity:
1. **Domain Entity Constructor** (if adding constructor):
   - `backend/CncApp/CncApp.Domain/Entities/Shift.cs`
   - Set default values in property initializers or constructor

2. **Infrastructure Configuration** (if field has database constraints):
   - `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/ShiftConfiguration.cs`
   - Set `.HasDefaultValue()` or `.HasDefaultValueSql()` if needed

3. **Application Service Create Method** (when implemented):
   - `backend/CncApp/CncApp.Application/Services/Shifts/Commands/ShiftService.Create.cs`
   - Set defaults after mapping from DTO, before saving

4. **AutoMapper Profile** (when implemented):
   - `backend/CncApp/CncApp.Application/Mapping/ShiftProfile.cs`
   - Use `.ForMember(dest => dest.NewField, opt => opt.MapFrom(src => defaultValue))` for DTO → Entity mapping

### Audit Fields (Automatic)
**Note:** Audit fields (`CreatedDateTime`, `CreatedByUserId`, etc.) are automatically populated by `AppDbContext.SaveChangesAsync()` - no manual intervention needed.

---

## 7. Related Entity References

### Entities That Reference Job:
1. **Order** (`CncApp.Domain/Entities/Order.cs`)
   - Navigation: `ICollection<Job> Jobs`
   - Relationship: One-to-Many (Order → Jobs)

2. **Machine** (`CncApp.Domain/Entities/Machine.cs`)
   - Navigation: `ICollection<Job> Jobs`
   - Relationship: One-to-Many (Machine → Jobs)
   - Initialized in constructor: `Jobs = new List<Job>();`

3. **StockLot** (referenced but not shown in detail)
   - Relationship: One-to-Many (StockLot → Jobs)

### Entities That Reference Shift:
1. **Job** (`CncApp.Domain/Entities/Job.cs`)
   - Navigation: `ICollection<Shift> Shifts`
   - Relationship: One-to-Many (Job → Shifts)

2. **User** (`CncApp.Domain/Entities/User.cs`)
   - Navigation: `ICollection<Shift> Shifts`
   - Relationship: One-to-Many (User → Shifts)

---

## 8. Dependency Injection

**Status:** ❌ **NO REGISTRATIONS EXIST**

No Job or Shift services/repositories registered in DI container.

**Current Registrations** (`CncApp.Infrastructure/DependencyInjection.cs`):
- `IMachineRepository` → `MachineRepository`
- `IUserRepository` → `UserRepository`

**When Implemented, Add:**
- `IJobRepository` → `JobRepository`
- `IShiftRepository` → `ShiftRepository`
- `JobService` (if using service pattern)
- `ShiftService` (if using service pattern)

---

## 9. Testing

**Status:** ❌ **NO TESTS EXIST**

No tests found for Job or Shift entities, services, or repositories.

**Expected Location:**
- `backend/CncApp/CncApp.Domain.Tests/Entities/JobTests.cs`
- `backend/CncApp/CncApp.Application.Tests/Services/Jobs/`
- `backend/CncApp/CncApp.Application.Tests/Services/Shifts/`

**Pattern to Follow:**
- `CncApp.Domain.Tests/Entities/MachineTests.cs`

---

## 10. Summary of Findings

### ✅ What Exists:
- Domain entities (`Job.cs`, `Shift.cs`)
- EF Core configurations (`JobConfiguration.cs`, `ShiftConfiguration.cs`)
- Database migrations (tables created)
- DbSets in `AppDbContext`
- Navigation properties in related entities (Order, Machine, User)
- Automatic audit field population

### ❌ What's Missing:
- Application layer DTOs
- AutoMapper profiles
- Application services
- Repository interfaces and implementations
- API controllers
- Dependency injection registrations
- Unit tests
- Create/update flows

### 🔍 Key Observations:
1. **No explicit column selections** - Safe to add new columns without breaking queries
2. **No existing create/update flows** - New fields can be defaulted in any of the standard locations
3. **Clean slate** - Full implementation needed following Machine/User patterns
4. **Audit fields handled automatically** - No manual intervention needed for audit columns

---

## 11. Recommended Implementation Order

When implementing Job and Shift functionality:

1. **Repository Layer** (Infrastructure)
   - Create `IJobRepository` interface
   - Create `JobRepository` implementation
   - Create `IShiftRepository` interface
   - Create `ShiftRepository` implementation

2. **Application Layer**
   - Create DTOs (`JobDto`, `CreateJobRequestDto`, etc.)
   - Create AutoMapper profiles
   - Create services (`JobService`, `ShiftService`)

3. **API Layer**
   - Create controllers (`JobsController`, `ShiftsController`)

4. **Dependency Injection**
   - Register repositories and services

5. **Testing**
   - Create unit tests for entities, services, and repositories

---

**Report Generated:** Discovery scan completed
**Last Updated:** Current scan results

