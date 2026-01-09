# CNC Shop Inventory Management – Backend Architecture Project

## Overview

The primary focus of this project is the design and implementation of a **SOLID, scalable backend architecture**. Rather than starting with complex business rules, the project prioritizes **clean separation of concerns**, strong testing boundaries, and a structure that can evolve safely as domain complexity increases.

> SOLID principles in this project are applied primarily to **structure, boundaries, and dependencies**, even while business rules are intentionally minimal in early phases.

The backend is organized into five clearly defined layers:

* **Domain** – Core business rules and invariants
* **Application** – Use cases and orchestration logic
* **Infrastructure** – Database access, persistence, and external concerns
* **API** – HTTP endpoints and request/response handling
* **Testing** – Domain and application tests that validate behavior and guard against regression

The goal is to isolate functionality to its appropriate layer, resulting in code that is easier to reason about, easier to test, and more resilient to change over time.

---

## Problem Domain

The theoretical problem this project models is a **simple shop inventory management system**.

At a high level, the system represents the lifecycle of raw material within a machine shop:

* Raw bar stock enters the system through shipping and receiving
* Inventory is tracked by stock lots
* Inventory is consumed by jobs
* Consumed material results in finished parts (products)

While the domain itself is intentionally straightforward, it provides a realistic foundation for exploring inventory flow, traceability, and backend design patterns commonly found in manufacturing systems.

---

## Current State of the Project

The project is currently operating under **Phase 1: End-to-End Functional Foundations**.

Phase 1 focuses on establishing consistent architectural patterns across all implemented slices of functionality.

At this stage, the focus has been on:

* Establishing the backend architecture and project structure
* Implementing full end-to-end flows (Database → Domain → Application → API)
* Writing tests alongside each slice of functionality
* Defining consistent patterns for controllers, services, repositories, and tests

Business logic at this point is intentionally **rudimentary**. Each API endpoint primarily interacts with its corresponding table or aggregate, without deeper cross-entity coordination.

This approach was chosen deliberately to allow the infrastructure, testing strategy, and architectural patterns to solidify before introducing more complex domain behavior.

> Each “slice” of functionality corresponds to a single aggregate and its supporting API, application, infrastructure, and test components.

---

## Next Phase: Targeted Business Logic

The next phase of the project will focus on **introducing richer, backend-controlled business logic**.

For example:

* The `StockLot` and `StockLotAdjustment` tables are conceptually linked
* Creating a new stock lot should also create a corresponding stock lot adjustment that records how much inventory was added

Rather than handling this logic at the API layer, future iterations will move these responsibilities into the **application and domain layers**, ensuring consistency, traceability, and enforceable invariants.

This phase will involve revisiting and refining existing logic to better reflect real-world workflows and domain rules.

---

## Design Philosophy

This project is intentionally built in stages. Early emphasis is placed on:

* Architectural clarity over feature completeness
* Testability over convenience
* Explicit boundaries between layers

By first establishing a strong foundation, later changes—such as adding cross-entity rules, transactional workflows, or more advanced inventory behavior—can be introduced with confidence and minimal risk.

---

## Backend Project Structure

Below is a quick tour of each backend project and its responsibilities. This section is intended to help orient readers to where different concerns live and how the layers interact.

---

### CncApp.Api

**ASP.NET Core Web API host**. This project is responsible for application startup and HTTP concerns only.

Responsibilities:

* Wires up controllers and routing
* Configures Swagger/OpenAPI
* Configures JWT authentication and ASP.NET Identity
* Registers global exception handling and problem details
* Pulls in the Application and Infrastructure layers
* Seeds default roles and a development admin user *in development only*

Key setup occurs in `Program.cs`:

* Controller and middleware registration
* Authentication and JWT configuration
* Development-only seeding logic

This layer intentionally contains **no business logic**.

---

### CncApp.Application

**Application/service layer** that orchestrates use cases on top of the domain.

Responsibilities:

* Registers per-aggregate services (Machines, Jobs, Materials, Orders, Parts, Shifts, StockLots, StockLotAdjustments, Users)
* Coordinates workflows using repositories
* Applies per-aggregate business rules today, with cross-entity orchestration planned for future phases
* Hosts AutoMapper profiles for DTO ↔ domain mapping

Services are registered centrally via `DependencyInjection.cs`, keeping orchestration logic out of controllers and repositories.

---

### CncApp.Infrastructure

**Data access and cross-cutting support layer**.

Responsibilities:

* Configures EF Core `AppDbContext` against SQL Server
* Implements repositories per aggregate
* Provides current-user resolution via HTTP context
* Syncs ASP.NET Identity users with domain users via an identity provisioning service

This layer contains all persistence concerns and external integrations, allowing higher layers to remain persistence-agnostic.

---

### CncApp.Domain

**Core domain layer** containing the business model.

Responsibilities:

* Domain entities (Machines, Jobs, Materials, Orders, Parts, Shifts, StockLots, StockLotAdjustments, Users)
* Shared base entity types and abstractions
* Domain enums and value concepts

At present, domain entities primarily enforce **invariants, validity, and lifecycle rules**, rather than complex cross-aggregate workflows.

The domain targets .NET 8 and enables nullable reference types and implicit usings. It is designed to be framework-light and focused on business meaning rather than infrastructure concerns.

---

### Testing Projects

* **CncApp.Domain.Tests** – Unit tests that validate domain behavior and invariants
* **CncApp.Application.Tests** – Unit tests that validate application/service logic per aggregate

Tests are structured to ensure:

* Domain tests validate invariants and prevent invalid states without touching persistence
* Application tests validate orchestration and workflows without re-testing domain rules

This testing boundary ensures high confidence while keeping tests fast, focused, and maintainable.

---

### Solution Structure

The `CncApp.sln` solution ties all projects together. Additional folders such as `docs/` and `postman/` provide supporting documentation and API testing artifacts outside the compiled code.

---

## Summary

This backend is structured around clear architectural boundaries, with each project owning a specific responsibility. The separation between API, Application, Infrastructure, and Domain layers is intentional and designed to support long-term maintainability, testability, and future expansion as business logic becomes more sophisticated.
