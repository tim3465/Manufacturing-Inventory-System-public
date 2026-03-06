---
name: managing
description: Orchestrates backend-implement → frontend-implement for a full-stack feature. Gates new-table creation against an approved plan file.
---

# Identity

## Purpose

Orchestrate a full-stack feature implementation by sequencing:

1) Backend phase (backend-implement)
2) Frontend phase (frontend-implement)

Gate any new-table creation against an approved plan file before allowing backend to proceed.

---

# Inputs

The following are required before the process begins:

- **Feature/change request** — what the feature should do
- **Approved plan file path** — path to a markdown file containing approved tables and their expected schemas

If either is missing, stop and request them before proceeding.

---

# Process

## Step 0 — Read the Approved Plan File

Parse the approved plan file. Extract:

- Any new tables listed as approved
- The expected column/schema for each approved table

Store this as the approval baseline for Step 1.

---

## Step 1 — Backend Phase

Invoke `backend-implement` with the feature request.

### Table Gating

After the backend triage report is produced, inspect it for any proposed new tables.

Apply the following rules:

| Scenario | Action |
|----------|--------|
| No new tables proposed | Auto-approve. Let backend proceed. |
| New table is in the approved plan AND structure matches | Auto-approve. Let backend proceed. |
| New table is in the approved plan BUT structure does NOT match | **Stop.** Show the discrepancy to the user. Wait for explicit approval or correction before continuing. |
| New table is NOT in the approved plan | **Stop.** Report the proposed table to the user. Do not proceed until the user explicitly approves or rejects it. |

Never guess on schema approval. Always surface ambiguities to the user.

### Backend Complete

After backend implementation is complete, report a **Backend Complete** summary to the user:

- Files changed/added
- New tables created (if any)
- API endpoints exposed (paths + DTO shapes)

Do not activate the frontend phase until this summary is reported and any table concerns are resolved.

---

## Step 2 — Frontend Phase

Invoke `frontend-implement` with:

- The original feature request
- The API contract produced by the backend phase (endpoints + DTO shapes from the Backend Complete summary)

After frontend implementation is complete, report a **Frontend Complete** summary:

- Files changed/added
- Components created or modified
- How to verify in the browser

---

## Step 3 — Final Output

Summarize both phases:

- All files changed (backend and frontend)
- New tables and migrations (if any)
- End-to-end verification steps: how to run the backend, generate the API client, and verify the feature in the browser

---

# Safety

## Sequencing Rules

- Never invoke `frontend-implement` before `backend-implement` is done and all table concerns are resolved.
- Always surface ambiguities to the user; never guess on schema approval.
- If backend exceeds its 2-revision iteration limit, stop the orchestration and escalate to the user with a **Orchestration Escalation Report**:
  - Summary of backend attempts
  - Remaining blockers
  - Recommended next step

## Stop Conditions

Stop orchestration if:

- Required inputs (feature request or approved plan file) are missing
- A proposed new table is not in the approved plan and the user has not explicitly approved it
- A proposed new table's structure does not match the approved plan and the user has not explicitly approved the discrepancy
- Backend exceeds 2 plan revisions without resolution
