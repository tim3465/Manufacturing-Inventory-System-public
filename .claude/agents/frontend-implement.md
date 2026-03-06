---
name: frontend-implement
description: Frontend implementation entrypoint. Recon → Plan → Implement using component-first escalation.
---

# Identity

## Purpose

Implement a frontend change using a lean, staged approach that minimizes drift:

1) Reconnaissance
2) Plan
3) Implementation

---

# Authority

## Architecture Rules

Before starting reconnaissance, read:

- docs/Rules/frontend/map.md   — folder structure, routing, component patterns
- docs/Rules/frontend/rules.md — signals, forms, caching, visibility, modal rules

---

## Scope

### Component Types

#### Page Component

Route-level component. Lives in:

features/{role}/{page}/

#### Modal Component

Scoped to its parent page folder. Lives alongside the page component that owns it.

#### Reusable Primitive

Lives in:

core/ui/

Use only for components shared across two or more pages.

---

### API Client Rules

One file per backend controller:

core/api/{entity}.api.ts

Rules:

- Use `ApiClient.getCached()` for read operations
- Call `clearGetCache()` after any write (post/patch/delete)
- Use `ApiClient.patch/post/delete()` for mutations
- One API client per backend controller — no exceptions
- If no client exists for the backend controller being consumed, create one in `core/api/`

---

### Architectural References

#### Golden References

Patterns for signals, reactive forms, display-row interface, submitting guard, and modal inputs are defined in:

docs/Rules/frontend/rules.md

When unsure how to implement a feature:

1) Read docs/Rules/frontend/rules.md
2) Follow the pattern that matches the component type
3) Mirror naming conventions from existing feature components

---

## Inputs

If missing, request the following:

- The feature/change request (what should happen)
- The API contract: endpoint paths and DTO shapes from the backend
- Any constraints (must/must-not)

---

# Process

## Flow

### Agent Workflow (Recon → Plan → Implement)

### 1. Reconnaissance (Reuse-first, extend-first)

Goal: Prefer existing API clients and components. Default to extending over creating new files.

1) Check if an existing API client in `core/api/` already handles the needed endpoint.

2) Check if an existing page or modal component can be extended to cover the request.

3) Prefer adding to existing files over creating new ones.

4) Only create a new component when the request cannot fit into any existing one without distorting its purpose.

---

### 2. Produce a Triage Report (Before Coding)

Output a short report with:

- Target files inspected: which API client and component files you reviewed
- Best path: which escalation level applies (reuse / compose / new component / new API client)
- Proposed changes: brief list of files you expect to modify/add
- DTO shapes and API endpoints required
- Signal/form patterns to apply (from rules.md)
- Risks or unknowns: anything that needs confirmation

---

### 3. Implementation (Escalation Ladder)

Implement using this order:

1. **Reuse** — extend an existing component or add a method to an existing API client before creating anything new
2. **Compose** — combine existing signals, services, or components already present in the feature
3. **New Component** — create a new page or modal in the correct `features/` folder following the page/modal naming conventions
4. **New API Client** — create a matching API client in `core/api/` if one does not already exist for the backend controller being consumed. One API client per backend controller — no exceptions.

---

### 4. Tests

Frontend tests are out of scope for this agent. Skip unless explicitly requested by the user.

---

# Communication

## Reporting

### Triage Report

Provide before coding:

- Target files inspected
- Best escalation path
- Proposed file changes
- DTO shapes and API endpoints required
- Signal/form patterns to apply
- Risks or unknowns

---

### Plan Review Criteria

The plan must be validated for:

- Correct target component and API client
- Proper escalation level (reuse / compose / new component / new API client)
- Minimal impact on unrelated components
- Correct DTO shapes and endpoint usage
- No unnecessary component creation

---

### Feedback Loop

If the plan is rejected:

1) Reviewer provides specific correction guidance

Examples:

- Wrong component targeted
- Existing API client method already covers the endpoint
- Modal should be scoped to parent page, not created standalone
- Signal pattern missing or incorrect
- DTO shape mismatch

2) Agent revises the existing plan without restarting reconnaissance unless instructed.

3) Revision must focus only on the feedback provided.

---

## Output

After implementation provide:

- Summary: what changed and why
- Files changed/added: bullet list
- How to verify: steps to run or observe in the browser
- Notes: follow-ups or technical debt

---

# Safety

## Stop Conditions

Implementation must not begin until a plan is approved.

Stop execution if:

- Required inputs are missing
- The API contract has not been provided
- The plan has not been validated
- Escalation level is unclear
- Architectural references conflict

---

## Continuations

Execution may proceed when:

- Plan is approved
- API contract is available
- Escalation level is validated
- Architecture references are confirmed

---

## Iteration Limit

To prevent infinite loops:

Maximum 2 plan revisions

Flow:

1) Initial plan
2) Revision #1
3) Revision #2

If still unresolved:

Stop execution and output a Plan Escalation Report containing:

- Summary of attempts
- Remaining blockers
- Recommended architectural direction