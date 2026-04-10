---
name: amend-ticket
description: Applies an approved amendment (delta) to the active GitHub ticket on the current branch. Reads the issue for context, accepts a structured amendment block, triages scope (backend, frontend, or both), executes only the delta, and returns a controlled summary.
disable-model-invocation: true
allowed-tools: Bash(echo *), Bash(gh issue view *), Bash(gh issue comment *)
---

## Purpose

This skill handles **post-implementation refinements** to an existing ticket.

It runs inline and:

- retrieves the active GitHub issue for context
- accepts an approved amendment block from the user
- determines whether the change is backend-only, frontend-only, or both
- delegates execution to backend and/or frontend agents
- validates outputs against the amendment scope
- returns a final amendment summary

The user is involved:

- at the start (confirming the correct ticket)
- when providing the amendment block
- at the end (reviewing the result before posting to GitHub)

---

## Step 1 — Read active issue

```powershell
echo $env:CURRENT_ISSUE
```

If present:

- Use `$env:CURRENT_ISSUE` as the issue number

If missing:

- Prompt user: "Enter GitHub issue number"
- Store value temporarily as `ISSUE_NUMBER`

---

## Step 2 — Fetch issue

If `$env:CURRENT_ISSUE` is present, run:

```powershell
gh issue view $env:CURRENT_ISSUE --json number,title,body,state
```

Otherwise run:

```powershell
gh issue view ISSUE_NUMBER --json number,title,body,state
```

Display:

- number
- title
- state
- full body

Ask user to confirm.

If rejected → STOP

---

## Step 3 — Accept amendment block

Prompt user:

"Paste the approved amendment block. Be explicit about what must change."

Requirements:

- Must describe **specific changes**
- Must be treated as the **approved delta**
- Must NOT redefine the entire ticket

Store as: `AMENDMENT_BLOCK`

---

## Step 4 — Parse context + delta

Treat:

- Issue body = original baseline
- Amendment block = approved delta

Extract:

- backend impact
- frontend impact
- constraints

---

## Step 5 — Scope triage

Determine:

- Backend only
- Frontend only
- Backend + Frontend

Rules:

- If unclear → ask user to clarify
- Do NOT assume both unless necessary

---

## Step 6 — Backend execution (if needed)

Call `backend-implement` with:

- issue number
- title
- original issue body
- amendment block (explicitly marked as delta)

Instruction:

- implement ONLY what is required for the amendment
- do NOT reimplement full ticket
- return Backend Complete Summary

---

## Step 7 — Backend validation (internal)

Validate against amendment block:

- no unrelated changes
- no schema drift unless explicitly required
- matches requested delta

If violation:

→ STOP and emit Orchestration Escalation Report

---

## Step 8 — Build frontend contract (if needed)

Prepare:

- updated endpoints
- updated DTOs
- backend changes relevant to amendment

---

## Step 9 — Frontend execution (if needed)

Call `frontend-implement` with:

- issue body
- backend contract (if applicable)
- amendment block

Instruction:

- implement ONLY amendment scope
- return Frontend Complete Summary

---

## Step 10 — Frontend validation (internal)

Check:

- aligns with backend changes
- matches amendment intent

If mismatch:

→ STOP and emit Orchestration Escalation Report

---

## Step 11 — Final output

Return:

- amendment summary
- backend changes (if any)
- frontend changes (if any)
- files changed
- verification steps

Do NOT commit  
Do NOT push  
Do NOT post to GitHub yet

---

## Step 12 — User approval

Ask user:

"Do you approve this amendment?"

If NO → STOP

If YES → continue

---

## Step 13 — GitHub amendment comment

Post a comment to the issue summarizing:

### Amendment Summary

- What was requested (from amendment block)
- What was implemented
- Key files changed
- Notes or decisions

Use the confirmed issue number from Step 2.

---

## Behavior Principles

- Amendment block is the source of truth for this run
- Original issue remains unchanged
- Only implement the delta
- Do not expand scope
- Keep execution lightweight

---

## Safety

Stop if:

- issue cannot be retrieved
- amendment block is missing or unclear
- amendment conflicts with original constraints
- backend/frontend mismatch cannot be resolved

---

## Architecture Model

/amend-ticket = delta orchestrator  
backend-implement = backend worker  
frontend-implement = frontend worker  

User = reviewer and approver
