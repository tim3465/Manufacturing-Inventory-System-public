# Machines Structure

This document lists all files involved in the Machines slice across the codebase.

## API Layer

### `backend/CncApp/CncApp.Api/Controllers/MachinesController.cs`
- **Class:** `MachinesController`
- Handles HTTP requests for machine operations (Create, List, Get, Delete)
- Exposes REST endpoints with authorization attributes (Admin role for create/delete, AllowAnonymous for read operations)

## Application Layer

### DTOs

#### `backend/CncApp/CncApp.Application/Dtos/Machines/MachineDto.cs`
- **Class:** `MachineDto`
- Data transfer object for machine data returned to clients
- Contains Id, SerialNumber, and ModelNumber with validation attributes

#### `backend/CncApp/CncApp.Application/Dtos/Machines/CreateMachineRequestDto.cs`
- **Class:** `CreateMachineRequestDto`
- Data transfer object for machine creation requests
- Contains SerialNumber and ModelNumber with validation attributes

### Services

#### `backend/CncApp/CncApp.Application/Services/Machines/MachineService.cs`
- **Class:** `MachineService` (partial)
- Base partial class containing constructor and dependencies (IMachineRepository, IMapper)

#### `backend/CncApp/CncApp.Application/Services/Machines/Commands/MachineService.Create.cs`
- **Class:** `MachineService` (partial)
- Implements CreateAsync method to create new machines using AutoMapper and repository

#### `backend/CncApp/CncApp.Application/Services/Machines/Commands/MachineService.Inactivate.cs`
- **Class:** `MachineService` (partial)
- Implements InactivateAsync method to soft-delete machines by calling repository and saving changes

#### `backend/CncApp/CncApp.Application/Services/Machines/Queries/MachineService.Get.cs`
- **Class:** `MachineService` (partial)
- Implements GetAsync method to retrieve a single machine by ID and map to DTO

#### `backend/CncApp/CncApp.Application/Services/Machines/Queries/MachineService.ListActive.cs`
- **Class:** `MachineService` (partial)
- Implements ListActiveAsync method to retrieve all active machines and map to DTOs

#### `backend/CncApp/CncApp.Application/Services/Machines/Queries/MachineService.ListAll.cs`
- **Class:** `MachineService` (partial)
- Implements ListAllAsync method to retrieve all machines (including inactivated) and map to DTOs

### Mapping

#### `backend/CncApp/CncApp.Application/Mapping/MachineProfile.cs`
- **Class:** `MachineProfile`
- AutoMapper profile defining mappings between Machine entity and MachineDto, and CreateMachineRequestDto to Machine entity

### Interfaces

#### `backend/CncApp/CncApp.Application/Interfaces/Repositories/IMachineRepository.cs`
- **Interface:** `IMachineRepository`
- Defines repository contract for machine data access operations (GetById, ListActive, ListAll, Add, Inactivate, SaveChanges)

## Domain Layer

### Entities

#### `backend/CncApp/CncApp.Domain/Entities/Machine.cs`
- **Class:** `Machine`
- Domain entity representing a machine with SerialNumber, ModelNumber, and Jobs collection
- Inherits from AuditableEntityBase for audit fields
- Enforces domain invariants through property setters and Inactivate method
- Uses private constructor for EF Core materialization

## Infrastructure Layer

### Repositories

#### `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/MachineRepository.cs`
- **Class:** `MachineRepository` (partial)
- Base partial class containing constructor and AppDbContext dependency

#### `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Commands/MachineRepository.Add.cs`
- **Class:** `MachineRepository` (partial)
- Implements AddAsync method to add machine entity to DbContext

#### `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Commands/MachineRepository.Inactivate.cs`
- **Class:** `MachineRepository` (partial)
- Implements InactivateAsync method to find machine by ID and set InactivatedDateTime

#### `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Commands/MachineRepository.SaveChanges.cs`
- **Class:** `MachineRepository` (partial)
- Implements SaveChangesAsync method to persist changes to database

#### `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Queries/MachineRepository.GetById.cs`
- **Class:** `MachineRepository` (partial)
- Implements GetByIdAsync method to retrieve machine by ID from DbContext

#### `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Queries/MachineRepository.ListActive.cs`
- **Class:** `MachineRepository` (partial)
- Implements ListActiveAsync method to query machines where InactivatedDateTime is null

#### `backend/CncApp/CncApp.Infrastructure/Repositories/Machines/Queries/MachineRepository.ListAll.cs`
- **Class:** `MachineRepository` (partial)
- Implements ListAllAsync method to retrieve all machines from DbContext

### Persistence

#### `backend/CncApp/CncApp.Infrastructure/Persistence/Configurations/MachineConfiguration.cs`
- **Class:** `MachineConfiguration`
- EF Core entity type configuration for Machine entity
- Configures primary key, required properties with max length, and relationship to Jobs with cascade delete

#### `backend/CncApp/CncApp.Infrastructure/Migrations/20251231204924_InitialCreate.cs`
- Contains migration code that creates the Machines table in the database
- Defines table schema with Id, SerialNumber, ModelNumber, and audit fields

## Tests

### Application Tests

#### `backend/CncApp/CncApp.Application.Tests/Services/Machines/Commands/InactivateMachineTests.cs`
- **Class:** `InactivateMachineTests`
- Unit tests for MachineService.InactivateAsync method
- Tests successful inactivation and handling of non-existent machines using mocked repository

#### `backend/CncApp/CncApp.Application.Tests/Services/Machines/Queries/GetMachineTests.cs`
- **Class:** `GetMachineTests`
- Unit tests for MachineService.GetAsync method
- Tests retrieval of existing machines and null handling for non-existent machines using mocked repository and mapper

### Domain Tests

#### `backend/CncApp/CncApp.Domain.Tests/Entities/MachineTests.cs`
- **Class:** `MachineTests`
- Domain unit tests for Machine entity invariants
- Tests constructor validation, property setter validation, and Inactivate method behavior including double-inactivation prevention

