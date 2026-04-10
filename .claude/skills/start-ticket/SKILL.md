---

name: start-ticket
description: Fully autonomous ticket execution orchestrator. Fetches the active GitHub issue, then runs backend and frontend agents end-to-end with internal validation and minimal user interruption.
disable-model-invocation: true
allowed-tools: Bash(echo *), Bash(gh issue view *)
--------------------------------------------------

## Purpose

This skill is the orchestrator for ticket execution.

It runs inline and:

* retrieves the active GitHub issue
* delegates work to backend and frontend agents
* validates outputs internally against the approved plan
* returns a final packaged result to the user

The user is only involved:

* at the start (confirming the correct ticket)
* at the end (reviewing final output)

---

## Step 1 — Read active issue

```powershell
echo $env:CURRENT_ISSUE
```

If missing → STOP

---

## Step 2 — Fetch issue

```powershell
gh issue view $env:CURRENT_ISSUE --json number,title,body,state
```

Display:

* number
* title
* state
* full body

Ask user to confirm.

If rejected → STOP

---

## Step 3 — Parse approved plan

Treat issue body as the source of truth.

Extract:

* backend scope
* frontend scope
* approved tables
* constraints

---

## Step 4 — Backend execution (autonomous)

Call backend-implement with:

* issue number
* title
* body

Instruction:

* perform full Recon → Plan → Implement
* return Backend Complete Summary

---

## Step 5 — Backend validation (internal)

Compare backend output to approved plan:

Rules:

* no unapproved tables
* no schema drift
* endpoints match intent

If violation:
→ STOP and emit Orchestration Escalation Report

Otherwise:
→ continue automatically

---

## Step 6 — Build frontend contract

Prepare:

* endpoints
* DTOs
* backend decisions

---

## Step 7 — Frontend execution (autonomous)

Call frontend-implement with:

* issue body
* backend contract

Instruction:

* perform full implementation
* return Frontend Complete Summary

---

## Step 8 — Frontend validation (internal)

Check:

* aligns with backend endpoints
* matches ticket intent

If major mismatch:
→ STOP and emit Orchestration Escalation Report

---

## Step 9 — Final output

Return:

* backend summary
* frontend summary
* files changed
* endpoints
* DTOs
* verification steps

Do NOT commit
Do NOT push

---

## Behavior Principles

* Fully autonomous execution
* No mid-process user approvals
* Orchestrator acts as managing agent
* Only escalate on blocking conflicts

---

## Safety

Stop if:

* missing issue
* cannot fetch issue
* backend introduces unapproved table
* frontend/backend mismatch cannot be resolved

---

## Architecture Model

/start-ticket = orchestrator
backend-implement = backend worker
frontend-implement = frontend worker

User = final reviewer
