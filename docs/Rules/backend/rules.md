---
category: backend-rules
area: rules
layer: backend
activation: passive
summary: Defines backend behavioral rules for controllers, services, domain entities, mapping, DTO validation, and workflows.
keywords:
  - controller rules
  - domain rules
  - automapper
  - dto validation
  - workflow pattern
  - transaction rules
  - api conventions
use-when:
  - writing backend code
  - implementing endpoints
  - enforcing domain invariants
  - applying architectural constraints
---

# Backend Rules

Rules verified against the current codebase. These supplement `map.md` (structure) with behavioral rules.

---

## Controller Rules

### HTTP Verb + Response Code Mapping

| Operation | Verb | Route | Success | Not Found |
|-----------|------|-------|---------|-----------|
| Create | `POST` | `/api/{entity}` | `201 Created` via `CreatedAtRoute` | — |
| Get | `GET` | `/api/{entity}/{id:int}` | `200 OK` | `404 NotFound` |
| List Active | `GET` | `/api/{entity}` | `200 OK` | — |
| List All | `GET` | `/api/{entity}/all` | `200 OK` | — |
| Update | `PATCH` | `/api/{entity}/{id:int}` | `200 OK` | `404 NotFound` |
| Inactivate | `PATCH` | `/api/{entity}/{id:int}/inactivate` | `204 NoContent` | `404 NotFound` |

Every action must have `[ProducesResponseType]` attributes for each possible status code.

### Not-Found Convention

- Service queries (`GetAsync`) return `null` when the entity doesn't exist.
- Service commands (`InactivateAsync`) return `false` when the entity doesn't exist.
- Controllers check the return value and respond with `NotFound()`.
- Controllers never throw exceptions for not-found — they return HTTP 404.

### CancellationToken

Every async method across all layers accepts `CancellationToken ct = default` as its last parameter:
- Controller actions
- Service methods
- Repository methods

### No Try/Catch in Controllers

Controllers never catch exceptions. All unhandled exceptions bubble to `GlobalExceptionHandler`.

---

## GlobalExceptionHandler

Maps exceptions to RFC 7807 `ProblemDetails` responses with `traceId` and `errorCode`.

| Exception Type | HTTP Status | Error Code |
|---------------|-------------|------------|
| `InvalidOperationException` | `400 Bad Request` | `INVALID_OPERATION` |
| Everything else (including `DomainException`) | `500 Internal Server Error` | `INTERNAL_SERVER_ERROR` |

`DomainException` extends `Exception` directly (not `InvalidOperationException`), so it falls through to the 500 default. `InvalidOperationException` is used by infrastructure services like `IdentityProvisioningService` for user-caused failures.

---

## Domain Entity Rules

### Construction Pattern

- **Private parameterless constructor** for EF Core materialization — sets backing fields directly to avoid validation.
- **Public constructor with parameters** for domain creation — validates via `Guard` and throws `DomainException` on violation.

### Property Setters

Property setters enforce invariants using the `Guard` class:
- `Guard.AgainstNullOrWhiteSpace(value, nameof(Property))`
- `Guard.AgainstMaxLength(value, MaxLength, nameof(Property))`

### Domain Methods

- `Inactivate(int? inactivatedByUserId = null)` sets `InactivatedDateTime` and `InactivatedByUserId`.
- Double-inactivation throws `DomainException`.
- Domain methods are the only way to mutate protected state — repositories must call domain methods, not set properties directly.

---

## AutoMapper Rules

- `CreateMap<Entity, Dto>()` — direct property mapping, includes `Id`.
- `CreateMap<CreateRequestDto, Entity>()` — map only client-provided fields.
- Navigation properties must be ignored: `.ForMember(dest => dest.Nav, opt => opt.Ignore())`.
- Never map audit fields (`CreatedDateTime`, `CreatedByUserId`, etc.) from DTOs to entities.
- Never map `Id` from request DTOs to entities (server-assigned).
- No business logic inside mapping profiles.

---

## DTO Validation Rules

- `[Required(ErrorMessage = "FieldName is required.")]`
- `[MaxLength(n, ErrorMessage = "FieldName cannot exceed n characters.")]`
- Max length values must match the domain entity constant and EF configuration exactly.
- Request DTOs: no `Id`, no audit fields.
- Response DTOs: include `Id`.

---

## Foreign Key Query Naming

When a query filters by a foreign key, name the route and parameter explicitly after the parent:

- ✅ Route: `/by-stocklot/{stockLotId:int}`
- ✅ Method: `ListByStockLotAsync(int stockLotId, CancellationToken ct = default)`
- ❌ Never use generic names like `/by-parent/{parentId}`

---

## Active Record Filter

"Active" means `InactivatedDateTime` is null. The LINQ pattern across all repositories:

```csharp
.Where(e => !e.InactivatedDateTime.HasValue)
```

---

## Multi-Table Write Rule

If a single user action creates or modifies records in more than one aggregate, it must be implemented as a **Workflow service** — not as sequential calls to individual entity services from the frontend.

- The frontend makes **one API call** to a workflow controller endpoint
- The workflow service owns the transaction boundary via `ITransactionManager`
- Single-entity services never manage transactions
- Workflow services live in `Application/Services/Workflows/{WorkflowName}/`
- Workflow controllers live in `Api/Controllers/Workflow/{WorkflowName}Controller.cs`

**Golden reference:** `ShippingReceiving` slice — see `ShippingReceivingService` and `ShippingReceivingController`.

**Examples of when this applies:**
- Creating an Order + one or more Jobs in a single user action
- Any write that spans more than one aggregate in one transaction

---

## Test Rules

### Test Boundary

- **Domain tests** verify entity invariants only. No database, no mocks, no application services.
- **Application tests** verify service workflows. Mock repository + mapper. Do not re-test domain invariants.

### Test Naming

- Domain: `{Scenario}_When{Condition}_ThrowsDomainException()` or `{Scenario}_When{Condition}_Creates{Entity}()`
- Application: `{MethodName}_When{Condition}_Returns{Result}()`

### Domain Test Structure

- Single file per entity: `{Entity}Tests.cs`
- Not partial — uses `#region` to organize: Constructor Tests, Property Setter Tests, Method Tests
- Class comment: "These tests do NOT access the database or test application workflows."

### Application Test Structure

- Partial class per method file
- Base file contains shared mocks (`MockRepository`, `MockMapper`) and service initialization
- Each test verifies mock interactions using `.Verify(..., Times.Once)` / `Times.Never`