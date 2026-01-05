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

### Phase 2 — Structural Cleanup (Minimal, Mechanical)
Goal: remove blockers that would cause drift or compile issues later.

Typical cleanup actions:
- Delete placeholder files and replace with correctly named method files (empty skeletons are OK)
- Fix namespaces to match SliceMap rules
- Ensure Application.Tests base file has shared setup consistent with Machines test base
- Ensure DI registrations exist (Application + Infrastructure)

Deliverable:
- Slice compiles after cleanup (even if methods are TODO).

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
