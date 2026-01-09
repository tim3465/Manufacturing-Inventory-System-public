CNC Shop Inventory Management – Backend Architecture Project

Overview

The primary focus of this project is the design and implementation of a SOLID, scalable backend architecture. Rather than starting with complex business rules, the project prioritizes clean separation of concerns, strong testing boundaries, and a structure that can evolve safely as domain complexity increases.
SOLID principles in this project are applied primarily to structure, boundaries, and dependencies, even while business rules are intentionally minimal in early phases.
The backend is organized into five clearly defined layers:
•	Domain – Core business rules and invariants
•	Application – Use cases and orchestration logic
•	Infrastructure – Database access, persistence, and external concerns
•	API – HTTP endpoints and request/response handling
•	Testing – Domain and application tests that validate behavior and guard against regression
The goal is to isolate functionality to its appropriate layer, resulting in code that is easier to reason about, easier to test, and more resilient to change over time.
________________________________________
Problem Domain
The theoretical problem this project models is a simple shop inventory management system.
At a high level, the system represents the lifecycle of raw material within a machine shop:
•	Raw bar stock enters the system through shipping and receiving
•	Inventory is tracked by stock lots
•	Inventory is consumed by jobs
•	Consumed material results in finished parts (products)
While the domain itself is intentionally straightforward, it provides a realistic foundation for exploring inventory flow, traceability, and backend design patterns commonly found in manufacturing systems.
________________________________________
Current State of the Project
The project is currently operating under Phase 1: End-to-End Functional Foundations.
Phase 1 focuses on establishing consistent architectural patterns across all implemented slices of functionality.
At this stage, the focus has been on:
•	Establishing the backend architecture and project structure
•	Implementing full end-to-end flows (Database → Domain → Application → API)
•	Writing tests alongside each slice of functionality
•	Defining consistent patterns for controllers, services, repositories, and tests
Business logic at this point is intentionally rudimentary. Each API endpoint primarily interacts with its corresponding table or aggregate, without deeper cross-entity coordination.
This approach was chosen deliberately to allow the infrastructure, testing strategy, and architectural patterns to solidify before introducing more complex domain behavior.
Each slice of functionality corresponds to a single aggregate and its supporting API, application, infrastructure, and test components.
________________________________________
Next Phase: Targeted Business Logic
The next phase of the project will focus on introducing richer, backend-controlled business logic.
Examples include:
•	The StockLot and StockLotAdjustment tables are conceptually linked
•	Creating a new stock lot should also create a corresponding stock lot adjustment that records how much inventory was added
Rather than handling this logic at the API layer, future iterations will move these responsibilities into the application and domain layers, ensuring consistency, traceability, and enforceable invariants.
This phase will involve revisiting and refining existing logic to better reflect real-world workflows and domain rules.
________________________________________
Design Philosophy
This project is intentionally built in stages. Early emphasis is placed on:
•	Architectural clarity over feature completeness
•	Testability over convenience
•	Explicit boundaries between layers
By first establishing a strong foundation, later changes—such as cross-entity rules, transactional workflows, or more advanced inventory behavior—can be introduced with confidence and minimal risk.
________________________________________
Backend Project Structure
Below is a high-level tour of each backend project and its responsibilities.
________________________________________
CncApp.Api
ASP.NET Core Web API host responsible for application startup and HTTP concerns.
Responsibilities:
•	Wire up controllers and routing
•	Configure Swagger/OpenAPI
•	Configure JWT authentication and ASP.NET Identity
•	Register global exception handling and problem details
•	Pull in the Application and Infrastructure layers
•	Seed default roles and a development admin user (development only)
This layer intentionally contains no business logic.
________________________________________
CncApp.Application
Application/service layer that orchestrates use cases on top of the domain.
Responsibilities:
•	Register per-aggregate services (Machines, Jobs, Materials, Orders, Parts, Shifts, StockLots, StockLotAdjustments, Users)
•	Coordinate workflows using repositories
•	Apply per-aggregate business rules today, with cross-entity orchestration planned for future phases
•	Host AutoMapper profiles for DTO ↔ domain mapping
All service registrations are centralized to keep orchestration logic out of controllers and repositories.
________________________________________
CncApp.Infrastructure
Data access and cross-cutting support layer.
Responsibilities:
•	Configure EF Core AppDbContext against SQL Server
•	Implement repositories per aggregate
•	Provide current-user resolution via HTTP context
•	Sync ASP.NET Identity users with domain users via an identity provisioning service
This layer contains all persistence concerns and external integrations, allowing higher layers to remain persistence-agnostic.
________________________________________
CncApp.Domain
Core domain layer containing the business model.
Responsibilities:
•	Domain entities (Machines, Jobs, Materials, Orders, Parts, Shifts, StockLots, StockLotAdjustments, Users)
•	Shared base entity types and abstractions
•	Domain enums and value concepts
At present, domain entities primarily enforce invariants, validity, and lifecycle rules, rather than complex cross-aggregate workflows.
The domain targets .NET 8, enables nullable reference types and implicit usings, and is designed to remain framework-light and business-focused.
________________________________________
Testing Projects
•	CncApp.Domain.Tests – Unit tests validating domain behavior and invariants
•	CncApp.Application.Tests – Unit tests validating application/service orchestration per aggregate
Testing boundaries:
•	Domain tests enforce invariants and prevent invalid states without touching persistence
•	Application tests validate workflows and orchestration without re-testing domain rules
This separation keeps tests fast, focused, and maintainable while providing high confidence.
________________________________________
Solution Structure
The CncApp.sln solution ties all projects together. Supporting folders such as docs/ and postman/ contain documentation and API testing assets outside the compiled code.
________________________________________
Summary
This backend is structured around clear architectural boundaries, with each project owning a specific responsibility. The separation between API, Application, Infrastructure, and Domain layers is intentional and designed to support long-term maintainability, testability, and future expansion as business logic becomes more sophisticated
