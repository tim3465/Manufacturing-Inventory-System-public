# ARCHITECTURE_RULES.md

**Purpose:** These are the **non‑negotiable guardrails** Cursor (and we) must follow when generating/refactoring code in this CNC app.  
This file is intentionally short. It defines **boundaries**, not full implementation plans.

---

## 1) Layering (Clean Architecture)

### Domain (`CncApp.Domain`)
**Owns:** business concepts (Entities / Value Objects), domain rules/invariants.  
**Must NOT reference:** EF Core, AutoMapper, ASP.NET, Infrastructure, API.

Rules:
- No persistence attributes required (avoid `[Key]`, `[Required]`, etc.).
- Keep entities focused on business meaning.
- Base entities (`EntityBase`, `AuditableEntityBase`) live here.

### Application (`CncApp.Application`)
**Owns:** use cases, DTOs, interfaces, validation, mapping profiles.  
**May reference:** Domain only.  
**Must NOT reference:** Infrastructure, EF Core, DbContext.

Rules:
- Define repository/service interfaces here (contracts).
- Use DTOs for inputs/outputs at the app boundary.
- Mapping lives here (AutoMapper profiles or manual mapping).

### Infrastructure (`CncApp.Infrastructure`)
**Owns:** EF Core implementation details and external integrations.  
**May reference:** Domain and Application (to implement Application interfaces).  
**Must NOT reference:** API.

Rules:
- `DbContext`, migrations, and EF configurations live here.
- Implement repositories/interfaces defined by Application.
- One EF configuration class per entity (preferred), applied via assembly scanning.
- Soft delete query filters belong here (EF config), not Domain.

### API (`CncApp.Api`)
**Owns:** HTTP endpoints and composition root (DI wiring).  
**May reference:** Application + Infrastructure (for registration), Domain only if needed for shared primitives.  
**Must NOT contain:** EF Core usage in controllers, business logic in controllers.

Rules:
- Controllers are thin: validate request → call Application service/use case → return response.
- DI setup belongs here (`Program.cs`), using registration extension methods when available.

---

## 2) Data + persistence rules

- Domain entities are **not** tables; Infrastructure maps them to tables.
- **Primary keys:** use `Id` on entities. Foreign keys are `<EntityName>Id`.
- Constraints belong in EF Fluent Config (Infrastructure), not as attributes in Domain.
- Soft delete uses `InactivatedAt`/`InactivatedByUserId` (or the project’s chosen fields) and is enforced via EF query filters.

---

## 3) Mapping rules (Entity ⇄ DTO)

- Mapping happens in **Application**.
- Prefer AutoMapper for consistency once introduced:
  - `Entity -> Dto`
  - `Create/UpdateDto -> Entity`
- Controllers should not manually map complex objects unless trivial.

---

## 4) Package placement

- EF Core packages: **Infrastructure only**.
- AutoMapper packages: **Application (and API only for DI registration if needed)**.
- Testing packages: **test projects only**.

---

## 5) Cursor instructions (how we use it)

When asking Cursor to generate/refactor code, every prompt should:
- Reference this file: `ARCHITECTURE_RULES.md`
- Specify the **layer** being edited (Domain / Application / Infrastructure / API)
- Restrict scope to a small, reviewable change set
- Explicitly say what **not** to touch (especially migrations/DbContext/controllers)

---

## 6) Definition of “clean” (reviewer-friendly)

A change is “clean” when:
- Dependency direction is correct (Domain ← Application ← Infrastructure; API composes)
- EF Core details do not leak into Domain/Application
- Use cases are testable without the database
- Controllers stay thin and boring

