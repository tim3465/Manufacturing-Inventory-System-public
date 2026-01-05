# Machines Conventions

This document defines explicit rules that other slices must follow, based on the Machines slice implementation.

## Naming Conventions

### Classes and Interfaces
- **Controllers**: `{Entity}Controller` (plural form, e.g., `MachinesController`)
- **Services**: `{Entity}Service` (singular form, e.g., `MachineService`)
- **Repositories**: `{Entity}Repository` (singular form, e.g., `MachineRepository`)
- **Repository Interfaces**: `I{Entity}Repository` (e.g., `IMachineRepository`)
- **DTOs**: 
  - Response DTOs: `{Entity}Dto` (e.g., `MachineDto`)
  - Request DTOs: `Create{Entity}RequestDto` (e.g., `CreateMachineRequestDto`)
- **Mapping Profiles**: `{Entity}Profile` (e.g., `MachineProfile`)
- **EF Core Configurations**: `{Entity}Configuration` (e.g., `MachineConfiguration`)
- **Domain Entities**: `{Entity}` (singular, e.g., `Machine`)

### Test Classes
- **Domain Tests**: `{Entity}Tests` (e.g., `MachineTests`)
- **Application Tests**: `{Operation}{Entity}Tests` (e.g., `InactivateMachineTests`, `GetMachineTests`)

### Methods
- All async methods must end with `Async` suffix
- Service methods: `CreateAsync`, `GetAsync`, `ListActiveAsync`, `ListAllAsync`, `InactivateAsync`
- Repository methods: `GetByIdAsync`, `ListActiveAsync`, `ListAllAsync`, `AddAsync`, `InactivateAsync`, `SaveChangesAsync`
- Controller methods: `CreateAsync`, `GetAsync`, `ListAsync`, `ListAllAsync`, `DeleteAsync`

## Folder Conventions

### Application Layer
- **DTOs**: `Application/Dtos/{Entity}/` (e.g., `Application/Dtos/Machines/`)
- **Services**: `Application/Services/{Entity}/` with subfolders:
  - `Commands/` for write operations
  - `Queries/` for read operations
- **Mapping**: `Application/Mapping/` (single folder, not per entity)
- **Interfaces**: `Application/Interfaces/Repositories/`

### Infrastructure Layer
- **Repositories**: `Infrastructure/Repositories/{Entity}/` with subfolders:
  - `Commands/` for write operations
  - `Queries/` for read operations
- **Persistence Configurations**: `Infrastructure/Persistence/Configurations/`
- **Migrations**: `Infrastructure/Migrations/`

### Domain Layer
- **Entities**: `Domain/Entities/` (single folder, not per entity)

### Tests
- **Application Tests**: `Application.Tests/Services/{Entity}/Commands/` and `Application.Tests/Services/{Entity}/Queries/`
- **Domain Tests**: `Domain.Tests/Entities/`

## Partial Class Split Rules

### Service Partial Classes
- **Base file**: `{Entity}Service.cs` contains:
  - Constructor
  - Private readonly fields for dependencies (repository, mapper)
- **Command files**: `Commands/{Entity}Service.{MethodName}.cs` (e.g., `Commands/MachineService.Create.cs`)
- **Query files**: `Queries/{Entity}Service.{MethodName}.cs` (e.g., `Queries/MachineService.Get.cs`)
- Each method implementation gets its own file
- All partial classes must be in the same namespace

### Repository Partial Classes
- **Base file**: `{Entity}Repository.cs` contains:
  - Constructor
  - Private readonly field for AppDbContext
- **Command files**: `Commands/{Entity}Repository.{MethodName}.cs` (e.g., `Commands/MachineRepository.Add.cs`)
- **Query files**: `Queries/{Entity}Repository.{MethodName}.cs` (e.g., `Queries/MachineRepository.GetById.cs`)
- Each method implementation gets its own file
- All partial classes must be in the same namespace

## Repository Rules

### Interface Definition
- Repository interface defined in `Application/Interfaces/Repositories/I{Entity}Repository.cs`
- Interface methods must match implementation signatures exactly
- Methods must include `CancellationToken ct = default` parameter

### Implementation
- Repository implements interface from Application layer
- Uses `AppDbContext` as dependency (injected via constructor)
- **Commands** (write operations):
  - Return `Task` or `Task<bool>`
  - Do NOT call `SaveChangesAsync` (service layer responsibility)
  - Examples: `AddAsync`, `InactivateAsync`
- **Queries** (read operations):
  - Return `Task<T>` or `Task<List<T>>`
  - Use LINQ queries on `_context.{Entity}Set`
  - Examples: `GetByIdAsync`, `ListActiveAsync`, `ListAllAsync`
- **SaveChanges**: Separate method `SaveChangesAsync` called by service layer

### Query Patterns
- Active entities: Filter by `!entity.InactivatedDateTime.HasValue`
- All entities: Use `ToListAsync()` without filtering

## Service Rules

### Dependencies
- Service depends on repository interface (`I{Entity}Repository`)
- Service depends on `IMapper` (AutoMapper)
- Both injected via constructor

### Method Patterns
- **Commands** (write operations):
  - Call repository method
  - Call `SaveChangesAsync` after successful repository operation
  - Return appropriate type (e.g., `Task<int>` for Create, `Task<bool>` for Inactivate)
- **Queries** (read operations):
  - Call repository method
  - Map result to DTO using AutoMapper
  - Return `Task<Dto>` or `Task<List<Dto>>`
  - Return `null` if entity not found (for single-item queries)

### Error Handling
- Services do NOT catch domain exceptions (let them bubble up to GlobalExceptionHandler)
- Services return `null` or `false` for "not found" scenarios (not exceptions)

## Controller Rules

### Dependencies
- Controller depends on concrete service class (NOT interface)
- Service injected via constructor

### Method Signatures
- All methods are `async Task<ActionResult>` or `async Task<ActionResult<T>>`
- Methods accept `CancellationToken ct = default` as last parameter
- Request DTOs use `[FromBody]` attribute

### HTTP Methods and Routes
- **POST** `/api/{controller}` for Create operations
- **GET** `/api/{controller}` for List operations (active only)
- **GET** `/api/{controller}/all` for List All operations (including inactivated)
- **GET** `/api/{controller}/{id:int}` for Get by ID operations
- **DELETE** `/api/{controller}/{id:int}` for Delete/Inactivate operations
- Use `[Route("api/[controller]")]` attribute on controller class

### Authorization
- Create/Delete operations: `[Authorize(Roles = "Admin")]`
- Read operations: `[AllowAnonymous]` or `[Authorize(Roles = "Admin")]` depending on endpoint

### Response Types
- **201 Created**: Use `CreatedAtRoute` with route name for Create operations
- **200 OK**: Use `Ok()` for successful queries
- **204 NoContent**: Use `NoContent()` for successful Delete operations
- **404 NotFound**: Use `NotFound()` when entity not found
- Use `[ProducesResponseType]` attributes for API documentation

### Error Handling
- Controllers do NOT catch exceptions (handled by GlobalExceptionHandler)
- Check service return values (null/false) and return appropriate HTTP status codes

## Exception Rules

### Domain Exceptions
- Domain entities throw `DomainException` when invariants are violated
- Domain exceptions are thrown from:
  - Entity constructors (invalid parameters)
  - Property setters (invalid values)
  - Domain methods (invalid operations, e.g., double-inactivation)
- Domain exceptions use `Guard` class for common validations

### Exception Handling
- Domain exceptions bubble up through service and controller layers
- `GlobalExceptionHandler` middleware catches all exceptions
- `DomainException` maps to HTTP 400 BadRequest with error code "DOMAIN_ERROR"
- Controllers and services do NOT catch domain exceptions

### Not Found Scenarios
- Services return `null` for single-item queries when not found
- Services return `false` for command operations when entity not found
- Controllers check return values and return HTTP 404 NotFound

## Mapping Rules (AutoMapper)

### Allowed
- AutoMapper is used for entity-to-DTO and DTO-to-entity mapping
- Mapping profiles defined in `Application/Mapping/{Entity}Profile.cs`
- Profiles inherit from `Profile` base class
- Simple property mappings: `CreateMap<Entity, Dto>()`
- Ignore navigation properties: `.ForMember(dest => dest.NavigationProperty, opt => opt.Ignore())`
- Map from request DTOs to entities for creation

### Forbidden
- Do NOT map audit fields (CreatedDateTime, CreatedByUserId, etc.) from DTOs to entities
- Do NOT map identity fields (Id) from request DTOs to entities
- Do NOT perform business logic in mapping profiles
- Do NOT map complex transformations (use service layer instead)

### Mapping Patterns
- Entity → DTO: Direct property mapping, includes Id
- CreateRequestDto → Entity: Map only client-provided fields, ignore navigation properties and audit fields

## Test Boundaries

### Domain Tests (`Domain.Tests/Entities/`)
- **Purpose**: Test domain entity invariants and business rules
- **Scope**: 
  - Constructor validation
  - Property setter validation
  - Domain method behavior (e.g., `Inactivate`)
- **Rules**:
  - NO database access
  - NO mocks
  - NO application services
  - Test that invalid states throw `DomainException`
  - Test that valid states are created correctly
  - Use `Assert.Throws<DomainException>()` for validation tests
  - Use `#region` to organize test groups (Constructor Tests, Property Setter Tests, Method Tests)

### Application Tests (`Application.Tests/Services/{Entity}/`)
- **Purpose**: Test application service workflows
- **Scope**:
  - Service method behavior
  - Repository interaction
  - DTO mapping
- **Rules**:
  - Use mocks for dependencies (`Mock<IRepository>`, `Mock<IMapper>`)
  - NO database access
  - NO domain entity instantiation in service tests (mocked repository returns entities)
  - Test successful operations
  - Test error scenarios (null returns, false returns)
  - Verify mock interactions using `Verify()`
  - Organize by Commands/Queries subfolders

### Test Naming
- Domain tests: `{Scenario}_When{Condition}_ThrowsDomainException()` or `{Scenario}_When{Condition}_Creates{Entity}()`
- Application tests: `{MethodName}_When{Condition}_Returns{Result}()`

## DTO Validation Rules

### Validation Attributes
- Use `[Required]` attribute with error message
- Use `[MaxLength]` attribute with error message matching domain constraints
- Validation attributes mirror EF Core configuration constraints
- Include comments noting validation mirrors Infrastructure configuration

### DTO Structure
- Response DTOs include `Id` property
- Request DTOs do NOT include `Id` (server-assigned)
- Request DTOs do NOT include audit fields (server-controlled)

## Entity Framework Configuration Rules

### Configuration Class
- One configuration class per entity: `{Entity}Configuration`
- Implements `IEntityTypeConfiguration<{Entity}>`
- Located in `Infrastructure/Persistence/Configurations/`

### Configuration Patterns
- Configure primary key: `builder.HasKey(e => e.Id)`
- Configure required properties: `.IsRequired().HasMaxLength(n)`
- Configure relationships: `HasMany/WithOne/HasForeignKey/OnDelete`
- Use cascade delete for dependent entities (e.g., Jobs → Machine)

## Dependency Injection Rules

### Service Registration
- Services registered in `Application/DependencyInjection.cs`
- Use `services.AddScoped<{Entity}Service>()` (concrete class, not interface)

### Repository Registration
- Repositories registered in `Infrastructure/DependencyInjection.cs`
- Use `services.AddScoped<I{Entity}Repository, {Entity}Repository>()`

## Inconsistencies (Drift)

### 1. Repository Inactivate Method Bypasses Domain Logic
**Location**: `Infrastructure/Repositories/Machines/Commands/MachineRepository.Inactivate.cs`

**Issue**: The repository directly sets `InactivatedDateTime = DateTimeOffset.UtcNow` instead of calling the domain entity's `Inactivate()` method.

**Expected**: Repository should call `machine.Inactivate(inactivatedByUserId)` to enforce domain invariants (e.g., preventing double-inactivation).

**Current Code**:
```csharp
machine.InactivatedDateTime = DateTimeOffset.UtcNow;
```

**Should Be**:
```csharp
machine.Inactivate(inactivatedByUserId);
```

**Impact**: This bypasses the domain validation that prevents double-inactivation, potentially allowing invalid state.

### 2. Service Create Method Uses Fully Qualified Type Name
**Location**: `Application/Services/Machines/Commands/MachineService.Create.cs`

**Issue**: Uses `CncApp.Domain.Entities.Machine?` instead of just `Machine?` after importing the namespace.

**Current Code**:
```csharp
CncApp.Domain.Entities.Machine? machine = _mapper.Map<Machine>(dto);
```

**Expected**: Should use `Machine?` since the namespace is imported.

**Impact**: Minor inconsistency, but violates typical C# naming conventions.

