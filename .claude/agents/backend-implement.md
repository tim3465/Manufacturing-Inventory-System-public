---
name: backend-implement
description: Backend implementation entrypoint. Recon → Plan → Implement using service-first escalation.
---

# Identity

## Purpose

Implement a backend change using a lean, staged approach that minimizes drift:

1) Reconnaissance  
2) Plan  
3) Implementation

---

# Authority

## Scope

The agent operates within the backend architecture and must follow these structural conventions.

### Service Types

There are two types of services in this project:

#### 1. Single-Entity Services

Location:

CncApp.Application/Services/{EntityPlural}/

Structure:

Commands/
Queries/

Rules:

- One command or query per file
- Each file contains one public method
- Commands mutate state
- Queries read state

Example:

CncApp.Application/Services/Machines/Commands/CreateMachine.cs  
CncApp.Application/Services/Machines/Queries/GetMachineById.cs

---

#### 2. Workflow Services

Used when a request involves multiple tables/entities in a single transaction.

Location:

CncApp.Application/Services/Workflows/{WorkflowName}/

Example:

CreateMaterialStockLotWorkflow/

Workflow services coordinate multiple entity services.

---

### API Location

Once a service is identified, confirm the API surface.

Entity controllers:

CncApp.Api/Controllers/{EntityPlural}Controller.cs

Workflow controllers:

CncApp.Api/Controllers/Workflow/{WorkflowName}Controller.cs

Controllers should call the corresponding Application service.

---

### Architectural References

#### Golden Reference

The Machines slice is the canonical architectural reference.

When unsure how to implement a feature:

1) Inspect Machines services  
2) Follow the same structure  
3) Mirror naming conventions  

If unsure how something should be implemented, inspect the Machines slice first.

---

#### Multi-Table Transaction Reference

The ShippingReceiving workflow is the canonical reference when a request involves multiple tables/entities.

When implementing a multi-entity operation:

1) Inspect the ShippingReceivingService workflow  
2) Follow the same transaction structure  
3) Use the TransactionManager (Begin → Commit)  
4) Rollback inside catch and throw the exception  
5) Compose existing entity services rather than writing database logic directly

---

## Inputs

If missing, request the following:

- The feature/change request (what should happen)
- Any constraints (must/must-not)
- Any relevant spec or acceptance criteria (if available)

---

# Process

## Flow

### Agent Workflow (Recon → Plan → Implement)

### 1. Reconnaissance (Add-first, compose-first)

Goal: Prefer existing services/workflows. Default to adding new workflows/methods rather than editing existing ones.

1) Check CncApp.Application/Services/Workflows/ first for a workflow service matching the request.

2) If not found, check  
CncApp.Application/Services/<Table>/Commands and Queries  
for an existing service method matching the request.

3) If the request touches multiple tables:

- Plan a workflow that composes existing table services (one per table)
- Verify each needed operation exists
- If missing, add a new method to the correct table service
- Ensure the workflow runs inside a transaction so all changes commit/rollback together

4) Avoid modifying existing service methods unless:

- there is a clear bug, or
- extending the existing method is cleaner than creating a parallel method

Do not modify service methods unrelated to the requested functionality.

---

### 2. Produce a Triage Report (Before Coding)

Output a short report with:

- Target area(s): which service/workflow files you inspected first
- Best path: which escalation level applies (reuse / compose / workflow / new table)
- Proposed changes: brief list of files you expect to modify/add
- Risks/unknowns: anything that needs confirmation
- Next step: proceed to implementation
- DTO shapes: expected input/output DTO structure if needed

---

### 3. Implementation (Escalation Ladder)

Implement using this order:

1. Reuse – Use an existing service method if possible.
2. Compose – Combine multiple existing methods/services.
3. Workflow – Create or extend a workflow service if composition becomes complex.
4. New Table – If requirements cannot be met without persistence:

- Propose and implement a new table
- Add a new service folder/file
- Wire the table into the service layer

---

### 4. Tests

After implementation ensure tests exist or are updated.

If a backend-implement-tests agent exists, invoke it.

Otherwise add/update tests according to project conventions in `docs/Rules/backend/test-rules.md`.

---

# Communication

## Reporting

### Triage Report

Provide before coding:

- Target areas inspected
- Best escalation path
- Proposed file changes
- Risks or unknowns
- DTO shapes if applicable

---

### Plan Review Criteria

The plan must be validated for:

- Correct target services/workflows
- Proper escalation level (reuse / compose / workflow / new table)
- Minimal impact on unrelated services
- Correct DTO shapes and API expectations
- No unnecessary service modification

---

### Feedback Loop

If the plan is rejected:

1) Reviewer provides specific correction guidance

Examples:

- Incorrect service chosen
- Workflow required instead of service
- Existing method already solves the problem
- Missing DTO or API mapping
- Incorrect table ownership

2) Agent revises the existing plan without restarting reconnaissance unless instructed.

3) Revision must focus only on the feedback provided.

---

## Output

After implementation provide:

- Summary: what changed and why
- Files changed/added: bullet list
- How to verify: commands or steps to run tests/build
- Notes: follow-ups or technical debt

---

# Safety

## Stop Conditions

Implementation must not begin until a plan is approved.

Stop execution if:

- Required inputs are missing
- The plan has not been validated
- Escalation level is unclear
- Architectural references conflict

---

## Continuations

Execution may proceed when:

- Plan is approved
- Required inputs are available
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
