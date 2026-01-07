# CNC App Slice Primer (Reusable)

## Purpose

This primer is the **repeatable playbook** for taking *one database table* (“one slice”) from **scaffold → working backend**:

- Domain invariants (Domain layer)
- Repository persistence (Infrastructure layer)
- Service workflows (Application layer)
- API endpoints (API layer)
- Tests (Domain.Tests + Application.Tests)
- Verification (build + tests)

Use this file for **every slice** (Jobs, Materials, Orders, Parts, Shifts, StockLots, StockLotAdjustments, etc.).

---

## Source of Truth

When anything conflicts:

1) **Machines slice** is the golden reference.
2) **`/docs/SliceMap.md`** is the canonical structure + naming contract.
3) Slice-specific audits (ex: `/docs/JobsSliceAudit.md`) describe current scaffold state and required cleanup.

**Non‑negotiables:**
- Mirror Machines patterns; do not “improve” existing code as you go.
- Commands/Queries are **folders** only — namespaces **must NOT** include `.Commands` or `.Queries`.
- Services are injected as **concrete types**; repositories use interfaces.
- Repositories do **not** call `SaveChanges` inside `Add`/`Inactivate`; `SaveChanges` is separate and called by the service.

---

## Test Boundary Contract (Locked)

- **Domain tests** verify *domain invariants* and prevent invalid states.
  - No database access.
  - No “orphan state” possibility.
  - Do not inspect EF/Core persistence.

- **Application tests** verify *workflows/use‑cases* via **Application service methods** (not controllers).
  - Assume the domain already enforces invariants.
  - Mock repository + mapper as needed.
  - Do not re-test invariants.

(Keep this boundary strict to avoid overlap and brittle tests.)

---

## Slice Vocabulary

- `{Entity}` = singular class name (e.g., `Job`)
- `{EntityPlural}` = plural folder/controller name (e.g., `Jobs`)
- **Commands** = writes/mutations (Create/Inactivate/Update/etc.)
- **Queries** = reads (Get/ListActive/ListAll/etc.)
- **Placeholder files** = seeded files that exist only to “hold a spot” and must be **deleted/replaced** with real method files (one method per file).

---

## Standard Execution Sequence (Do This Every Slice)

### Phase 0 — Load Context (No Changes)
Goal: ensure Cursor “knows the rules” before it touches code.

- Read: `SliceMap.md`
- Skim Machines implementation across layers (paths referenced in SliceMap)
- Read any slice audit that exists for the target entity (or generate one)

Deliverable:
- A short “facts only” summary: what exists now + what is missing.

---

### Phase 1 — Slice Audit (No Changes)
Goal: inventory current slice scaffold and identify required cleanup.

If you don’t have an audit MD yet, generate it:
- Output: `/docs/slices/{EntityPlural}.Audit.md`

Audit must include:
- Full path listing per layer
- Placeholder inventory (what must be replaced)
- Conformance check vs Machines + SliceMap
- “Ready-to-Implement?” verdict + blockers

---

### Phase 2A — Structural Cleanup (Filesystem Only, No Contracts)

Goal:
Prepare the slice’s folder and file layout so later phases can proceed without drift, without committing to behavior, workflows, or endpoints.

This phase is intentionally non-semantic.
You are organizing where things will go, not what they do.

Allowed actions (ONLY these)

Delete placeholder / example files
(e.g. files with Placeholder, PlaceFolder, Example, Template in the name)

Create empty files with correct final names

Files may contain:

namespace

partial class declaration

TODO comments

Files must NOT contain:

method bodies

logic

signatures that imply behavior

Fix namespaces to match SliceMap.md

No .Commands or .Queries in namespaces

All partials share the slice root namespace

Ensure folder structure mirrors Machines:

Commands / Queries folders exist where required

Test folders mirror Application.Services structure

Ensure base test file exists (shared setup only)

Mocks may be declared

No test methods

Explicitly NOT allowed in Phase 2

❌ No DTO properties

❌ No repository interfaces or method signatures

❌ No service method signatures

❌ No EF / DbContext usage

❌ No mapping profiles or CreateMap calls

❌ No DI registrations

❌ No controller endpoints

❌ No tests with assertions

❌ No logic of any kind

If a change implies “this operation exists”, it does not belong in Phase 2.

Deliverable

The slice:

Has correct folders

Has correctly named empty files

Has correct namespaces

The solution may compile or may not compile
(compilation is NOT a requirement at this phase)

Completion criteria:

“There is now a correct place for everything — but nothing meaningful exists yet.”

Guardrail (very important)

If you are unsure whether an action is structural or behavioral:

Assume it is behavioral and defer it to a later phase.


---

### Phase 2B — Contract Definition (No Behavior, No Persistence)

Goal
Define the explicit contracts for this slice based on intent, without implementing behavior, workflows, or persistence.

This phase answers:

“What operations exist for this slice?”
not
“How do they work?”

Preconditions

Before starting Phase 2B:

Phase 2A is complete

Placeholder / example files have been removed

Correct empty files exist with correct final names

Slice intent has been defined (which Commands / Queries exist)

If slice intent is unclear, stop and define it before proceeding.

Allowed actions (ONLY these)
1) DTO contracts

Create or complete DTO classes:

{Entity}Dto

Create{Entity}RequestDto (only if Create exists)

Update{Entity}RequestDto (only if Update exists)

DTOs may include:

Properties

DataAnnotations validation

DTO rules:

DTOs must reflect domain constraints

Do not speculate about UI behavior

Do not add fields “just in case”

2) Repository interface (signatures only)

Define method signatures in:

I{Entity}Repository

Allowed:

Method names

Parameters

Return types

NOT allowed:

EF Core usage

DbContext access

Any implementation logic

Important:
Repository interfaces define what persistence is required, not how it is done.

3) Service method signatures (optional, shape only)

Service partial files may contain:

Method signatures

Empty bodies

throw new NotImplementedException();

Purpose:

Define workflow entry points

Reserve names and parameters

Enable later phases to plug in behavior

Rules:

One method per file

No logic

No repository calls

No mapping usage

4) Mapping contracts

Create or complete mapping profiles.

Allowed:

CreateMap<,>() declarations only

NOT allowed:

Custom mapping logic

Value transformations

Conditional mapping

Ignoring fields for behavioral reasons

Mappings exist here only to define type relationships, not behavior.

5) Test scaffolding (structure only)

Ensure test files exist for each intended operation.

Files may contain:

Class declaration

Constructor

TODO comments

NOT allowed:

Assertions

Test logic

Mock setup beyond empty fields

Purpose:

Reserve the test surface

Mirror the Application.Services structure

Explicitly NOT allowed in Phase 2B

❌ Repository implementations

❌ Editing repository .cs method files

❌ EF Core usage

❌ DbContext access

❌ SaveChangesAsync calls

❌ Controller endpoints

❌ Business logic

❌ Domain invariants or domain behavior

❌ Test assertions

❌ DI registrations

❌ “Fixing” compilation errors caused by unimplemented repositories

If code does something, it does not belong in Phase 2B.

Deliverable

At the end of Phase 2B:

DTOs define input/output shape

Repository interfaces define persistence needs

Service methods define workflow entry points

Mapping profiles define type relationships

No behavior exists

No persistence exists

Repository implementations are untouched

Compilation is optional at this phase and must not be forced.

Guardrail (Hard Stop)

Phase 2B ends when you can say:

“We know exactly what this slice is capable of —
but nothing actually works yet.”

If anything works, Phase 2B has gone too far.

---

### Phase 3 — Implement Backend (Small Steps, In Order)

Recommended order (mirrors real dependencies):

1) **Domain**: entity invariants + domain methods (e.g., `Inactivate`)
2) **Application contracts**: DTOs + repository interface signatures
3) **Infrastructure**: repository method implementations + EF config if needed
4) **Application service**: command/query service methods + mapping usage
5) **API controller**: endpoints that call service methods
6) **Tests**:
   - Domain.Tests: invariants + domain methods
   - Application.Tests: service workflows (mock repo/mapper)

Deliverable:
- Endpoints compile and function; tests pass.

---

### Phase 4 — Verification & Wrap
Verification checklist:
- `dotnet build` succeeds
- Domain tests pass
- Application tests pass
- API project builds
- If schema changed: migration created + applied in dev
- Quick manual smoke test (create, get, list active/all, inactivate)

Wrap:
- Commit with a narrative message
- PR description references slice name, what’s implemented, what’s deferred (UI, etc.)

---

## Prompt Templates (Reusable)

> These are templates. Replace `{Entity}` / `{EntityPlural}` and paste into Cursor.

### Prompt A — Slice Audit (No Changes)
- Create `/docs/slices/{EntityPlural}.Audit.md`
- Scan repo for all `{Entity}`/`{EntityPlural}` files across layers
- Compare to Machines + SliceMap
- List placeholders and required replacements
- No file modifications

### Prompt B — Structural Cleanup (Minimal Changes)
- Delete placeholder files for `{EntityPlural}`
- Create real method files with correct names (empty skeletons OK)
- Fix namespaces to match SliceMap
- Add missing DI registrations (service + repository)
- Ensure Application.Tests base file has shared setup
- No refactors outside `{EntityPlural}` slice

### Prompt C1 — Domain Implementation
- Implement invariants, constructors, and domain methods in `{Entity}.cs`
- Add/adjust Domain.Tests for invariants and domain methods

### Prompt C2 — Repository Implementation
- Implement repository query + command methods (one method per file)
- Ensure repository calls domain methods for mutations
- Ensure SaveChanges is separate

### Prompt C3 — Service Implementation
- Implement service methods (one method per file)
- Call repository + mapper appropriately
- Ensure workflow logic lives in service, not controller

### Prompt C4 — API Implementation
- Implement controller endpoints mirroring Machines
- Ensure correct routing and authorization pattern

### Prompt C5 — Application Tests
- Add tests per method file mirroring Machines test structure
- Mock repository + mapper
- Verify workflows and repository interactions (not domain invariants)

### Prompt D — Verification
- Build + run tests
- Summarize failures with exact file/line and propose smallest fix
- No refactors

---

## Placeholder File Rules

If a file name contains terms like:
- `Placeholder`
- `PlaceFolder`
- `Example`
- `Template`

Treat it as a **temporary scaffold**.

Rules:
- **Delete it** when you start real implementation for that method group.
- Replace with correctly named files (e.g., `JobService.Get.cs`, not `JobService.PlaceholderQuery.cs`).
- One method per file (Commands/Queries folders).

---

## Quick Reference: What “Done” Means for a Slice

A slice is “backend done” when:
- Domain entity enforces invariants and supports required domain methods
- Repository interface + implementation support:
  - `GetById`, `ListActive`, `ListAll`, `Add`, `Inactivate`, `SaveChanges`
- Service supports:
  - `Create`, `Inactivate`, `Get`, `ListActive`, `ListAll`
- Controller exposes endpoints mirroring Machines
- Tests exist:
  - Domain.Tests: invariants + domain methods
  - Application.Tests: workflows per service method
- Build + tests green

---

## Commit + PR Hygiene (Recommended)

Commit message pattern:
- `Slice: Implement {EntityPlural} backend`
or split by layer if you prefer small commits:
- `Domain: Enforce {Entity} invariants`
- `Infrastructure: Implement {Entity} repository methods`
- `Application: Implement {Entity} service workflows`
- `Api: Add {EntityPlural} endpoints`
- `Tests: Add {Entity} domain + application tests`

PR description should include:
- What’s implemented (commands/queries)
- What’s intentionally deferred (UI, extra methods, etc.)
- Any migrations added
- Verification steps run (build/tests)

---
### Phase 5 — Postman Smoke-Test Collection (Regenerate + Version)

**Goal**  
Create (or update) a **reusable Postman collection** for this slice so a developer can run an end-to-end smoke test quickly (auth → create → get → list → update/inactivate, as applicable).

This phase exists to keep manual verification **repeatable** and **low-friction**, without forcing the AI to “patch” an older collection in risky ways.

---

## Strategy (Versioned, Safe-by-Default)

**Default behavior (recommended):**
- **Keep the existing Postman collection file**
- Generate a **new** collection file with a **timestamp** suffix
- Do not attempt to “merge” or “repair” the old one

**Why:** avoids drift and avoids the AI trying to reinterpret old structure.

---

## File Location and Naming

- Folder: `/postman/` at repo root (or your existing Postman folder name)
- Output filename pattern (reusable):
  - `{EntityPlural}.SmokeTests.{YYYYMMDD-HHmm}.postman_collection.json`

**Examples:**
- `StockLots.SmokeTests.20260105-1432.postman_collection.json`
- `Jobs.SmokeTests.20260105-1432.postman_collection.json`

---

## Inputs Required Before Generating

Phase 5 generation must reference:
- The slice **intent** document (which operations exist)
- The actual API routes implemented (controller)
- The app’s auth workflow requirements

If intent says an operation does **not** exist, it must **not** appear in the collection.

---

## Required Collection Contents (Reusable Template)

### 1) Environment Variables (collection-level or environment file)

Must define variables so this works across machines:
- `baseUrl`
- `adminEmail` (or username)
- `adminPassword`
- `token` (or cookie/session value depending on your auth)
- `entityId` (captured from Create)
- Any required foreign key IDs (e.g., `materialId`) if needed for Create

---

### 2) Auth / Admin Login (First)

- One request that logs in as admin
- A test script that captures auth output into variables (token/cookie)
- Every write request must use that auth variable

---

### 3) Slice Smoke Tests (Only What Exists)

Include folders (or request groups) like:
- **Create** (if supported)
- **Get** (if supported)
- **ListActive** (if supported)
- **ListAll** (if supported)
- **Update** (PUT or PATCH, if supported)
- **Inactivate** (if supported)

Each request should:
- Assert expected HTTP status code
- Capture any IDs needed for later steps (Create → `entityId`)
- Avoid excessive assertions (smoke tests are minimal)

---

### 4) Edge Cases (Optional, Lightweight)

Only include a few if they’re stable and cheap:
- Get non-existent ID → 404
- Update non-existent ID → 404
- Inactivate non-existent ID → 404

---

## “Keep Old vs Regenerate From Old” Rule

- **Preferred:** Generate a **new** collection from scratch every time (versioned file)
- **Allowed (optional):** If you have a known-good login workflow already, you may:
  - Copy the **Auth/Login** request as-is
  - Regenerate the slice requests fresh

**Explicitly forbidden:** Editing the existing collection in place unless explicitly requested.

---

## Deliverable

- A new Postman collection JSON file saved under the Postman folder with a timestamped filename
- Collection includes:
  - Admin login step
  - Only the endpoints defined by slice intent
  - Minimal tests (status codes + variable capture)

---

## Completion Criteria

Phase 5 is complete when:
- A developer can import the new collection
- Set environment variables once
- Click **Run Collection** (or run requests in order)
- And confirm the slice works end-to-end in minutes

### Phase 6 — Postman README Sync (Documentation Only)

**Goal**  
Update `/postman/README.md` so humans can quickly find and run the latest smoke-test collection for the slice.

**Inputs**
- The newly generated file from Phase 5:
  - `{EntityPlural}.SmokeTests.{timestamp}.postman_collection.json`

**Allowed actions**
- Edit `/postman/README.md` only
- Add or update the `{EntityPlural}.SmokeTests.{timestamp}.postman_collection.json` section
- Update the **Latest** line to point to the newest timestamped file
- Update the slice’s collection variables list (if applicable)

**Explicitly NOT allowed**
- Do not modify any Postman collection JSON files
- Do not change API code, tests, or slice contracts
- Do not reorganize unrelated README sections

**Deliverable**
- `/postman/README.md` references the newest collection file and lists endpoints/variables.
