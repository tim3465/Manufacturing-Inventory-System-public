# CNC Shop Inventory Management System

Full-stack CNC shop inventory system built with ASP.NET Core (.NET 8) and Angular 21, designed with clean architecture, strict layer boundaries, and production-style patterns.

---

## Tech Stack

### Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- ASP.NET Identity
- JWT Authentication (role claims)
- AutoMapper
- xUnit + Moq (unit testing)
- Swagger / OpenAPI

### Frontend
- Angular 21 (standalone components)
- Angular Signals
- Reactive Forms
- Tailwind CSS (custom styling, no component library)
- Vitest (unit testing)
- HTTP Interceptors (JWT attachment + 401 handling)
- Custom API client with caching layer (TTL + invalidation)
- Dark mode via ThemeService

---

## Key Features

- Role-based authentication and authorization (JWT with role claims)
- Dual-user system (Identity user + Domain user)
- Soft delete pattern across all entities
- Full audit trail (Created/Updated/Inactivated with user tracking)
- Global exception handler returning RFC 7807 ProblemDetails
- Development-only database seeding (roles + admin user)
- API request caching layer (frontend)
- 60+ application-layer unit tests
- Strict Domain / Application / Infrastructure / API separation

---

## Roles & Access Control

Supported roles:

- Machinist
- Shipping & Receiving
- Supervisor
- Administrator
- User (baseline authenticated access)

Pages are restricted by role both:
- Backend (JWT role enforcement)
- Frontend (route guards + conditional navigation)

---

## Architecture Overview

The backend follows a layered Clean Architecture structure:

```
backend/CncApp/
├── CncApp.Api/               # Controllers, JWT config, Swagger
├── CncApp.Application/       # Services (Commands/Queries), DTOs, AutoMapper
├── CncApp.Infrastructure/    # EF Core, Repositories, Identity integration
├── CncApp.Domain/            # Entities, guards, domain exceptions
├── CncApp.Domain.Tests/      # Domain unit tests (xUnit)
└── CncApp.Application.Tests/ # Service tests (xUnit + Moq)
```

The frontend is organized by feature:

```
frontend/angular/
├── core/        # auth, api client, interceptors, theme
├── features/    # dashboard, machinist, shipping, supervisor, admin
├── shared/      # DTOs, reusable components
```

---

## Design Highlights

### Dual User Model
Authentication uses `IdentityUser<int>`, while business logic uses a separate `Domain.User` entity linked by `IdentityUserId`. This keeps authentication concerns separate from business concerns.

### Soft Delete + Audit Trail
All entities inherit from `AuditableEntityBase` and include:
- CreatedDateTime / CreatedByUserId
- UpdatedDateTime / UpdatedByUserId
- InactivatedDateTime / InactivatedByUserId

Audit fields are automatically populated in `SaveChangesAsync`.

### Global Exception Handling
Unhandled exceptions are mapped to structured RFC 7807 `ProblemDetails` responses with trace IDs for debugging.

### Service-per-Aggregate Pattern
The Application layer uses a service-per-aggregate structure, with partial classes organized into `Commands/` and `Queries/` folders.  
This is not CQRS with MediatR — it maintains a straightforward orchestration model without mediator abstraction.

---

## Testing

- xUnit for backend tests
- Moq for mocking dependencies
- 60+ application-layer test files
- Domain tests validate invariants without touching persistence
- Application tests validate orchestration without re-testing domain rules
- Vitest for frontend unit tests

The Domain layer has zero external NuGet dependencies.

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js (v18+ recommended)
- SQL Server or SQL Server LocalDB
- Angular CLI

---

### Run Backend

```
cd backend/CncApp
dotnet restore
dotnet ef database update
dotnet run --project CncApp.Api
```

Backend runs on:
```
https://localhost:7136
```

Swagger UI:
```
https://localhost:7136/swagger
```

---

### Run Frontend

```
cd frontend/angular
npm install
ng serve
```

Frontend runs on:
```
http://localhost:4200
```

The Angular proxy configuration forwards API requests to the backend.

---

## Development Seeding

Controlled via `appsettings.Development.json` flags:

- Role seeding
- Admin user seeding

This ensures clean local database resets without affecting production environments.

---

## API Testing

A Postman collection is available in the `/postman` directory for manual API testing.

---

## Documentation

Additional technical documentation is available in the `/docs` directory, including architecture notes and testing philosophy.

---

## Status

Active development.  
Architecture foundation complete.  
Incrementally expanding business workflows and UI integration.
