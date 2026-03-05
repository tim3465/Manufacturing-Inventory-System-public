---
name: backend-implement
description: Backend implementation entrypoint. Recon → Plan → Implement using service-first escalation.
context: fork
---

## Purpose

Implement a backend change using a lean, staged approach that minimizes drift:
1) Reconnaissance
2) Plan
3) Implementation

## Inputs to Request (if missing)

- The feature/change request (what should happen)
- Any constraints (must/must-not)
- Any relevant spec/acceptance criteria (if available)

### Plan Review and Iteration Limits

After the **Triage Report / Plan** is produced, the managing agent (or reviewer) must validate the plan **before implementation begins**.

### Recon Reference (Project Structure)

Use the following conventions to locate the correct backend slice before expanding search.

#### Service Types

There are two types of services in this project:

1. **Single-Entity Services**

Location:

```
CncApp.Application/Services/{EntityPlural}/
```

Structure:

```
Commands/
Queries/
```

Rules:

- One **command or query per file**
- Each file contains **one public method**
- Commands mutate state
- Queries read state

Example:

```
CncApp.Application/Services/Machines/Commands/CreateMachine.cs
CncApp.Application/Services/Machines/Queries/GetMachineById.cs
```

---

2. **Workflow Services**

Used when a request involves **multiple tables/entities** in a single transaction.

Location:

```
CncApp.Application/Services/Workflows/{WorkflowName}/
```

Example:

```
CreateMaterialStockLotWorkflow/
```

Workflow services coordinate multiple entity services.

---

#### API Location

Once a service is identified, confirm the API surface.

Entity controllers:

```
CncApp.Api/Controllers/{EntityPlural}Controller.cs
```

Workflow controllers:

```
CncApp.Api/Controllers/Workflow/{WorkflowName}Controller.cs
```

Controllers should call the corresponding **Application service**.

---

#### Golden Reference

The Machines slice is the canonical architectural reference.

When unsure how to implement a feature:
1) Inspect Machines services
2) Follow the same structure
3) Mirror naming conventions

If unsure how something should be implemented, inspect the Machines slice first.

#### Multi-Table Transaction Reference (Workflow Services in the workflow folder)

The **ShippingReceiving** workflow is the canonical reference when a request involves **multiple tables/entities**.

When implementing a multi-entity operation:

1) Inspect the `ShippingReceivingService` workflow
2) Follow the same transaction structure
3) Use the `TransactionManager` (Begin → Commit)
4) Rollback inside `catch` and `throw` the exception
5) Compose existing entity services rather than writing database logic directly

## Agent Workflow (Recon → Plan → Implement)


### 1) Reconnaissance (Add-first, compose-first)

Goal: Prefer existing services/workflows. Default to adding new workflows/methods rather than editing existing ones.

1) Check `CncApp.Application/Services/Workflows/` first for a workflow service matching the request.
2) If not found, check `CncApp.Application/Services/<Table>/Commands and Queries` for an existing service method matching the request.
3) If the request touches multiple tables:
   - Plan a workflow that composes existing table services (one per table).
   - Verify each needed operation exists; if missing, add a new method to the correct table service.
   - Ensure the workflow is executed within a transaction so all changes commit/rollback together.
4) Avoid modifying existing service methods unless:
   - there is a clear bug, or
   - extending the existing method is cleaner than creating a parallel method.
   - do not modify service methods unrelated to the requested functionality.

### 2) Produce a Triage Report (before coding)

Output a short report with:

- **Target area(s):** which service/workflow files you inspected first
- **Best path:** which escalation level you believe applies (reuse / compose / workflow / new table)
- **Proposed changes:** brief list of files you expect to modify/add
- **Risks/unknowns:** anything that needs confirmation
- **Next step:** proceed to implementation
- **Shape of the in/output dto:** if you expect to need new or modified DTOs, describe their shape here so we can confirm before you implement

### 3) Implementation (follow the escalation ladder)

Implement using this escalation order:

1. **Reuse:** If an existing service method can accomplish the task, use it.
2. **Compose:** If not, combine multiple existing methods/services to accomplish the task.
3. **Workflow:** If composition isn’t sufficient or becomes unclear, create or extend a workflow service.
4. **New table:** If the requirements cannot be met without new persistence, propose and implement a new table, then:
   - Add a new service folder/file for that table
   - Wire it into the service layer as needed

### 4) Tests

After implementation, ensure tests exist/are updated.

- If you have a dedicated test-writing skill, invoke it:
  - Run: `/backend-write-tests`
- Otherwise, add/update tests according to existing project conventions.

### 5) Output Format

At the end, provide:

- **Summary:** what changed and why
- **Files changed/added:** bullet list
- **How to verify:** commands or steps to run tests/build
- **Notes:** any follow-ups or tech debt identified

#### Plan Review Criteria

The plan should be checked for:

- Correct **target services or workflows**
- Proper **escalation level** (reuse / compose / workflow / new table)
- Minimal impact on unrelated services
- Correct **DTO shapes** and API expectations
- No unnecessary service modification when composition or workflow is sufficient

#### Feedback Loop

If the plan is rejected:

1. The reviewer must provide **specific correction guidance**, such as:
   - Incorrect service chosen
   - Workflow required instead of service
   - Existing method already solves the problem
   - Missing DTO or API mapping
   - Incorrect table ownership

2. The implementing agent must **revise the existing plan**, not restart reconnaissance unless explicitly instructed.

3. The revised plan should focus **only on the feedback provided**.

#### Iteration Limit

To prevent infinite loops:

- Maximum **2 plan revisions** are allowed.

Flow:

1. Initial plan
2. Revision #1 (if rejected)
3. Revision #2 (if rejected)

If the plan is still not acceptable after **2 revisions**:

- Stop execution
- Output a **Plan Escalation Report** containing:
  - Summary of attempts
  - Remaining blockers
  - Recommended architectural direction

Implementation must **not proceed** until a plan is approved.
