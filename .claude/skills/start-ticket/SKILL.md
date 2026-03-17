---
name: start-ticket
description: Fetch the active GitHub issue, display it, and hand off to the managing agent.
---

Fetch the active GitHub issue from the environment, display it to the user, and hand it off to the managing agent on confirmation.

---

## Trigger

User runs `/start-ticket` with no arguments.

---

## Configuration

```
REPO = tim3465/Manufacturing-Inventory-System
```

---

## Steps

### Step 1 — Read the issue number

Read the issue number from the environment variable:

```bash
echo $env:CURRENT_ISSUE
```

If `CURRENT_ISSUE` is not set or is empty, stop and tell the user:

> No active issue found. Make sure you opened this session using /new-worktree, which sets the issue number automatically.

---

### Step 2 — Fetch the issue from GitHub

```bash
gh issue view {CURRENT_ISSUE} --repo tim3465/Manufacturing-Inventory-System --json number,title,body,state
```

If the issue is not found, stop and tell the user:
> Issue #{CURRENT_ISSUE} not found in tim3465/Manufacturing-Inventory-System.

---

### Step 3 — Display the issue to the user

Print the issue clearly:

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  Issue #{number} — {title}
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

{body}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

Then ask the user:

> Ready to hand this off to the managing agent and begin implementation? (yes / no)

---

### Step 4 — Hand off to managing agent or exit

**If the user says yes:**

Invoke the `managing` agent with:

- Issue number: `{number}`
- Issue title: `{title}`
- Issue body: `{body}` (this is the approved plan)

**If the user says no:**

Print:
> Cancelled. No changes have been made. Run /start-ticket again when you are ready.

Then stop.

---

## Error Cases

| Situation | Response |
|-----------|----------|
| `CURRENT_ISSUE` not set | Stop. Tell user to use /new-worktree to open sessions. |
| Issue not found on GitHub | Stop. Tell user to check the issue number. |
| User cancels | Exit cleanly. No changes made. |
