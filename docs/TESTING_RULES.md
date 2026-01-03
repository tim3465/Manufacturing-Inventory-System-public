# Testing Rules & Conventions (CncApp)

This file defines how tests are organized and written for the CNC App.
The goals are consistency, fast feedback, and a repeatable pattern for every new slice.

---

## Test Project Layout

### Current test projects
- `CncApp.Application.Tests`  
  Unit tests for Application layer (services, commands/queries logic).  
  Uses mocks — no real database, no HTTP pipeline.

- `CncApp.Domain.Tests`  
  Unit tests for Domain entities/value objects when applicable (pure logic only).

> Later (optional):
- `CncApp.Api.Tests`  
  Integration tests using `WebApplicationFactory` to verify controllers, auth, validation, and ProblemDetails responses.
- `CncApp.Infrastructure.Tests`  
  Persistence/mapping tests (EF Core config verification) if needed.

---

## Folder Structure Convention (Mirrors Application)

In `CncApp.Application.Tests`, tests mirror the Application Services folders:

Services/

    Machines/
        Commands/
        Queries/

    Users/
        Commands/
        Queries/


Notes:
- We keep the same “Commands vs Queries” split as the Application layer.
- We do not rename production folders just to satisfy tests.

---

## Naming Conventions

### Test files
- `{UseCaseOrMethodName}Tests.cs`
  - Examples:
    - `GetMachineTests.cs`
    - `InactivateMachineTests.cs`
    - `CreateUserTests.cs`

### Test methods
Use the pattern:
- `MethodName_WhenCondition_ExpectedResult`

Examples:
- `GetAsync_WhenMachineExists_ReturnsDto()`
- `GetAsync_WhenMachineDoesNotExist_ReturnsNull()`
- `InactivateAsync_WhenNotFound_ReturnsFalse()`

---

## Test Style

### Use AAA (Arrange / Act / Assert)
Each test should clearly separate:
- Arrange: create inputs + mocks
- Act: call the method under test
- Assert: verify results and interactions

### One behavior per test
Prefer several small tests over one big test.

### Keep unit tests fast
Unit tests must:
- Avoid real DB access
- Avoid HTTP pipeline
- Avoid network calls
- Use mocks/stubs for repositories and external services

---

## What Belongs Where

### Application unit tests (`CncApp.Application.Tests`)
Test:
- Service logic (commands/queries behavior)
- Validation and business rule decisions
- Repository interaction patterns (mock verification when it matters)

Do NOT test:
- EF Core mappings
- ASP.NET Core ModelState / DTO annotation pipeline
- Authentication middleware

### API integration tests (`CncApp.Api.Tests`) (optional later)
Test:
- Routing, status codes, and endpoint behavior
- `[Authorize]` / role enforcement
- DTO annotation validation responses (400)
- Global exception handling (ProblemDetails)
- End-to-end request/response

---

## Mocking & Dependencies

### Defaults
- Test framework: xUnit
- Mocking: Moq (or project-standard mocking library)
- Assertions: xUnit asserts (optionally FluentAssertions/Shouldly if already present)

### Mock only what you need
- Mock repositories and external services
- Prefer verifying outcomes over verifying every mock call
- Verify key side effects (e.g., “Save was called once” when relevant)

---

## Error/Exception Behavior

### Unit tests
- If a service rejects an operation due to a rule, it should:
  - return a clear result, OR
  - throw an explicit exception type (preferred later), OR
  - throw `InvalidOperationException` (allowed temporarily)

### API layer
- Controllers should not use try/catch for business errors.
- Global exception handling returns `application/problem+json` ProblemDetails with:
  - `traceId`
  - `errorCode`

---

## Test Coverage Targets (Practical)

Start with “pattern builder” tests:
1) Machines Query: `GetAsync(id)` returns DTO/null
2) Machines Command: `InactivateAsync(id)` returns true/false
3) Users Command: `CreateAsync(dto)` success + duplicate rejection

Once patterns are stable:
- Add tests for new slices as they are implemented.
- Add integration tests only when a behavior spans multiple layers.

---

## Cursor Workflow Rules (Prompts)

When using Cursor to generate tests:
- One prompt = one small, reviewable change
- Do not modify production code during “test-only” prompts
- Output only modified/new files
- Keep naming and folder structure consistent with this document

