## User creation workflow (Backend)

This document describes how the CNC App backend provisions a new user, including **Identity user creation**, **Domain user creation**, and **role assignment**.

### Entry point: API

- **Endpoint**: `POST /api/users`
- **Controller**: `CncApp.Api.Controllers.UsersController`
- **Authorization**: `[Authorize(Roles = "Admin")]` (admin-only)
- **DTOs**:
  - Request: `CncApp.Application.Dtos.Users.CreateUserRequestDto`
  - Response: `CncApp.Application.Dtos.Users.CreateUserResponseDto`

At the controller level, the handler delegates to the application service:

- `UsersController.CreateAsync(dto)` → `_userService.CreateAsync(dto)`

### Application layer orchestration: `UserService.CreateAsync`

`CncApp.Application.Services.Users.UserService` is a partial class; the create workflow lives in:

- `CncApp.Application/Services/Users/Commands/UserService.Create.cs`

The service depends on:

- `IIdentityProvisioningService` (to avoid Application depending directly on ASP.NET Identity)
- `IUserRepository` (Domain user persistence)
- (also injected but not used in create): `ICurrentUserService`, `IMapper`

#### Step-by-step flow

1) **Create Identity user**

- Calls `IIdentityProvisioningService.CreateIdentityUserAsync(...)` with:
  - `email = dto.Email`
  - `userName = dto.Email` (by convention: **Identity UserName == Email**)
  - `password = dto.TemporaryPassword`

The result is the new **Identity user id** (`int identityUserId`).

2) **Assign roles (Identity roles are the source of truth for authorization)**

- If `dto.Roles.Any()`:
  - Calls `IIdentityProvisioningService.AssignRolesAsync(identityUserId, dto.Roles)`

3) **Create Domain user**

Creates a `CncApp.Domain.Entities.User` (Domain user/operator record) linked to the Identity user via `IdentityUserId`.

Key behaviors and conventions:

- `domainUser.IdentityUserId = identityUserId`
- `domainUser.UserName = dto.Email` (matches Identity username/email)
- `domainUser.FirstName/LastName = dto.FirstName/dto.LastName`
- Domain `User` explicitly **does not store email** (comment in entity + in service); Identity owns email as the source of truth.

4) **Persist Domain user**

- `IUserRepository.AddAsync(domainUser)`
- `IUserRepository.SaveChangesAsync()`

5) **Return response**

Returns:

- `IdentityUserId` (from Identity)
- `DomainUserId` (Domain user primary key after EF save)
- `UserName` (email)

### Identity provisioning details: `IdentityProvisioningService`

Interface:

- `CncApp.Application.Interfaces.IIdentityProvisioningService`

Implementation:

- `CncApp.Infrastructure.Services.IdentityProvisioningService`

This service uses ASP.NET Core Identity:

- `UserManager<IdentityUser<int>>`

#### CreateIdentityUserAsync

- Builds a new `IdentityUser<int>` with:
  - `UserName = email`
  - `Email = email`
  - `EmailConfirmed = true` (admin-provisioned users skip confirmation)
- Calls `_userManager.CreateAsync(identityUser, password)`
- If creation fails, throws `InvalidOperationException` with concatenated Identity error descriptions.
- Returns `identityUser.Id`

#### AssignRolesAsync

- Loads the user: `_userManager.FindByIdAsync(identityUserId.ToString())`
- If not found, throws `InvalidOperationException`
- Removes any existing roles first (`RemoveFromRolesAsync`) to enforce a “replace roles” behavior
- Adds the requested roles (`AddToRolesAsync`)
- Any failure throws `InvalidOperationException` with Identity error descriptions

### Domain persistence details: `UserRepository` + `AppDbContext`

Repository interface:

- `CncApp.Application.Interfaces.Repositories.IUserRepository`

Repository implementation (partial):

- `CncApp.Infrastructure.Repositories.UserRepository`
- In `UserRepository.AddAsync`, the entity is added to `AppDbContext.DomainUsers`.

DbContext:

- `CncApp.Infrastructure.Persistence.AppDbContext`
- Inherits `IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>` and also includes `DbSet<User> DomainUsers`.

#### Audit fields

Domain entities inherit `AuditableEntityBase`. `AppDbContext.SaveChangesAsync` populates audit fields by:

- Resolving the **current DomainUserId** from the JWT’s Identity user id using `ICurrentUserService`
- Setting `CreatedDateTime/CreatedByUserId` on add
- Setting `UpdatedDateTime/UpdatedByUserId` on update
- Handling inactivation audit (`InactivatedByUserId`) when `InactivatedDateTime` transitions from null → set

Note: the create-user workflow is an admin-only provisioning flow; the created Domain user is linked to Identity via `IdentityUserId` so future requests can be audited properly.

### Role data: where “roles” live

In the current design:

- **Authorization roles are Identity roles** (strings assigned through ASP.NET Identity and emitted into JWT role claims at login).
- The Domain contains a `RoleType` enum (`CncApp.Domain.Enums.RoleType`) but the provisioning flow assigns Identity roles from `CreateUserRequestDto.Roles` (string list).

This means:

- To authorize routes/endpoints, the system relies on **Identity roles** (`[Authorize(Roles = "Admin")]`, `ClaimTypes.Role`, etc.).
- Any role names sent from the client must match the Identity role names seeded/registered in the system.

### Dependency wiring

Infrastructure DI registers:

- `IIdentityProvisioningService` → `IdentityProvisioningService`
- `IUserRepository` → `UserRepository`
- `ICurrentUserService` → `CurrentUserService`

via `CncApp.Infrastructure.DependencyInjection.AddInfrastructureServices(...)`.


