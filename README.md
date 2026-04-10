# CNC Shop Inventory Management System

Full-stack CNC shop inventory system built with ASP.NET Core (.NET 8) and Angular 21, designed with clean architecture, strict layer boundaries, and production-style patterns.

---

## Tech Stack

### Backend

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core (SQL Server)
* ASP.NET Identity
* JWT Authentication (role claims)
* AutoMapper
* xUnit + Moq (unit testing)
* Swagger / OpenAPI

### Frontend

* Angular 21 (standalone components)
* Angular Signals
* Reactive Forms
* Tailwind CSS (custom styling, no component library)
* Vitest (unit testing)
* HTTP Interceptors (JWT attachment + 401 handling)
* Custom API client with caching layer (TTL + invalidation)
* Dark mode via ThemeService

---

## Key Features

* Role-based authentication and authorization (JWT with role claims)
* Dual-user system (Identity user + Domain user)
* Soft delete pattern across all entities
* Full audit trail (Created/Updated/Inactivated with user tracking)
* Global exception handler returning RFC 7807 ProblemDetails
* Development-only database seeding (roles + admin user)
* API request caching layer (frontend)
* 60+ application-layer unit tests
* Strict Domain / Application / Infrastructure / API separation

---

## AI-Assisted Development Workflow

This project includes a structured AI-assisted development system built on top of Claude Code.

Key capabilities:

* Ticket-driven development using GitHub issues as the source of truth
* Automated backend and frontend orchestration via specialized agents
* Strict output contracts for predictable implementation results
* Worktree-based isolation for parallel feature development
* Automated PR creation, issue linking, and cleanup workflows

Core commands:

* `/plan` — generate full implementation plan and write to issue
* `/new-worktree` — create isolated work environment per ticket
* `/start-ticket` — execute backend and frontend implementation
* `/git-commit` — guided commit message generation
* `/close-ticket` — push, open PR, and clean up worktree
* `/amend-ticket` — apply scoped refinements to existing tickets

This system enforces consistency, reduces manual overhead, and ensures alignment between planning and implementation.

---

## Developer Workflow System

The `.claude` directory contains a structured development system including:

* custom agents (backend/frontend implementation)
* reusable skills for ticket execution
* orchestration rules and validation layers

This system enables consistent, repeatable development workflows and enforces architectural boundaries during implementation.

---

## Roles & Access Control

Supported roles:

* Machinist
* Shipping & Receiving
* Supervisor
* Administrator
* User (baseline authenticated access)

Pages are restricted by role both:

* Backend (JWT role enforcement)
* Frontend (route guards + conditional navigation)

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

## UI Patterns

### Smart Table System

The frontend uses a reusable smart table pattern for data-heavy pages, featuring:

* Server-driven filtering and pagination
* Column-level filtering
* Consistent layout and interaction model
* Shared implementation across inventory, orders, and reporting views

The Shipping/Receiving Inventory table serves as the reference implementation for all future tables.

---

## Issue Log System

A messaging-style issue log system is used for tracking:

* downtime
* scrap
* production issues

Features include:

* user-specific highlighting
* timestamped entries
* structured logging for reporting
* reusable UI between shift logs and reports

---

## Design Highlights

### Dual User Model

Authentication uses `IdentityUser<int>`, while business logic uses a separate `Domain.User` entity linked by `IdentityUserId`.

### Soft Delete + Audit Trail

All entities inherit from `AuditableEntityBase` and include:

* CreatedDateTime / CreatedByUserId
* UpdatedDateTime / UpdatedByUserId
* InactivatedDateTime / InactivatedByUserId

Audit fields are automatically populated in `SaveChangesAsync`.

### Global Exception Handling

Unhandled exceptions are mapped to structured RFC 7807 `ProblemDetails` responses with trace IDs.

### Service-per-Aggregate Pattern

Application layer uses a service-per-aggregate structure organized into `Commands/` and `Queries/`.

---

## Performance Considerations

* Read-only database context used for query operations
* Entity Framework query optimizations (Include ordering)
* Frontend caching with TTL and invalidation
* Lazy loading of large datasets (e.g., shift history)

---

## API Authorization

All API endpoints are explicitly secured using role-based authorization.

* No anonymous access unless explicitly required
* Endpoint access aligned with frontend role restrictions
* Authorization audited and standardized across all controllers

---

## Testing

* xUnit for backend tests
* Moq for mocking dependencies
* 60+ application-layer test files
* Domain tests validate invariants without persistence
* Application tests validate orchestration without re-testing domain rules
* Vitest for frontend unit tests

The Domain layer has zero external dependencies.

---

## Getting Started

### Prerequisites

* .NET 8 SDK
* Node.js (v18+)
* SQL Server or LocalDB
* Angular CLI

---

### Run Backend

```
cd backend/CncApp
dotnet restore
dotnet ef database update
dotnet run --project CncApp.Api
```

Backend:

```
https://localhost:7136
```

Swagger:

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

Frontend:

```
http://localhost:4200
```

---

## Development Seeding

Controlled via `appsettings.Development.json`:

* Role seeding
* Admin user seeding

---

## API Testing

A Postman collection is available in `/postman`.

---

## Documentation

Additional technical documentation is available in `/docs`, including architecture notes and testing philosophy.

---

## Status

Active development.
Architecture foundation complete.
Incrementally expanding business workflows and UI integration.
