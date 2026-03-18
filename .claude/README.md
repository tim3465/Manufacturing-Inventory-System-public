# Ticket Workflow — End to End

This document covers the full lifecycle of a ticket: planning through close. Follow the steps in order.

---

## Overview

```
Plan → Worktree → Implement → Commit → Close → Cleanup
```

All skills and agents are invoked inside Claude Code. The terminal context determines which skills are available.

---

## Phase 1 — Plan the Ticket

**Where:** `main\` folder, inside Claude Code.

**Purpose:** Read the issue, build a full implementation plan, and write it back to the ticket body. No code is written yet.

### Step 1 — Enter plan mode

```
/plan
```

### Step 2 — Fetch and plan the issue

Paste this prompt with the issue number filled in:

```
Fetch GitHub issue #[NUMBER] from repo timjohnmontague/Manufacturing-Inventory-System. Read the following reference files before planning: backend-map.md, backend-rules.md, frontend-map.md, frontend-rules.md, and test-rules.md. Produce a full implementation plan — backend first, then frontend. Include file paths, method signatures, and any new DTOs or entities needed. Do not write any code yet.
```

### Step 3 — Review and adjust

Read the plan. Ask for adjustments as needed. Repeat until the plan looks right.

### Step 4 — Write the plan back to the ticket

Once the plan is finalized, paste this prompt with the issue number filled in:

```
Update GitHub issue #[NUMBER] on repo timjohnmontague/Manufacturing-Inventory-System with the finalized implementation plan from this session. Replace the existing issue body with the full plan as we have agreed on it. Do not summarize or shorten it — write the complete plan. Do not implement any code.
```

### Step 5 — Exit plan mode

Press `ESC` to exit plan mode without implementing anything. The ticket now holds the approved plan. Implementation happens in Phase 2.

---

## Phase 2 — Create the Worktree

**Where:** `main\` folder, inside Claude Code.

**Purpose:** Create an isolated Git worktree for the ticket and open a new terminal tab ready for implementation.

### Step 6 — Create the worktree

```
/new-worktree [ISSUE_NUMBER]
```

Example:
```
/new-worktree 42
```

What this does:
- Fetches the issue title from GitHub
- Builds a branch and folder name in the format `{number}-{slug}-{yyyymmdd}`
- Creates a Git worktree as a sibling of `main\`
- Opens a new terminal tab in Windows Terminal, `cd`'d into the worktree folder, with `CURRENT_ISSUE` set and Claude Code launched

If the new tab does not open automatically, the skill prints the manual commands to run.

---

## Phase 3 — Implement the Ticket

**Where:** The worktree terminal tab (not `main\`), inside Claude Code.

**Purpose:** Hand the approved plan to the managing agent, which orchestrates backend then frontend implementation.

### Step 7 — Start the ticket

```
/start-ticket
```

What this does:
- Reads `CURRENT_ISSUE` from the environment
- Fetches the issue (number, title, body) from GitHub
- Displays it for your review
- On confirmation, hands off to the `managing` agent

### What the managing agent does

The managing agent runs automatically after you confirm. You do not need to type anything to drive it — it reports back to you at key gates:

1. **Backend phase** — Invokes `backend-implement`. After completion, reports files changed, new tables, and API endpoints.
2. **Table gating** — If a new table is proposed that is not in the approved plan, the agent stops and asks for your explicit approval before continuing.
3. **Frontend phase** — Invokes `frontend-implement` with the feature request and the API contract from the backend phase.
4. **Frontend review gate** — After frontend is complete, the agent presents a summary and asks:
   > Does this look correct? Reply yes to close the feature, or provide feedback and I will send it back to the frontend agent for another pass.
5. **Final summary** — Lists all files changed, new tables and migrations, and end-to-end verification steps.

---

## Phase 4 — Commit Changes

**Where:** The worktree terminal tab, inside Claude Code.

**Purpose:** Stage changes and create a well-formed commit message locally. Does not push.

### Step 8 — Commit

```
/git-commit
```

What this does:
- Runs `git diff`, `git diff --staged`, and `git status`
- Generates 3 commit message options (title + bullet description each)
- You select one, then choose to submit as-is, edit directly, or edit with grammar/spell check
- Commits locally — never pushes

You can run `/git-commit` multiple times during implementation to make incremental commits.

---

## Phase 5 — Close the Ticket

**Where:** The worktree terminal tab, inside Claude Code.

**Purpose:** Push the branch, open a PR linked to the issue, post a commit summary on the issue, then clean up the worktree.

### Step 9 — Close the ticket

```
/close-ticket
```

What this does, in order:

1. **Safety check** — Confirms you are not in `main\` and that `CURRENT_ISSUE` is set.
2. **Uncommitted changes** — If any exist, offers to run `/git-commit` now or cancel.
3. **Existing PR check** — If a PR already exists for this branch, it checks for the `Closes #` link and patches it if missing.
4. **Push** — Pushes the branch to `origin`.
5. **Create PR** — Opens a PR against `claude-agent-workflows-20260303` with a commit log in the body and `Closes #[ISSUE_NUMBER]` in the description.
6. **Issue comment** — Posts a comment on the GitHub issue summarising the branch name, PR link, and commit list.
7. **Worktree cleanup** — Prints the final status and the exact command to run after closing the terminal:

```
git -C "C:\dev\projects\Manufacturing-Inventory-System\main" worktree remove "{WORKTREE_PATH}" --force
```

Close the terminal tab, then run that command from any terminal.

---

## Phase 6 — Periodic Worktree Cleanup

**Where:** Project parent folder (`C:\dev\projects\Manufacturing-Inventory-System`), inside Claude Code.

**Purpose:** Scan all worktree folders, check GitHub to confirm their issues are closed or PRs merged, and delete the safe ones.

### Step 10 — Run cleanup

```
/cleanup-worktrees
```

What this does:
- Lists all sibling folders matching the `{number}-{slug}-{date}` pattern (skips `main`)
- For each folder, checks the linked issue state and PR merge state on GitHub
- Prints a summary table: safe to delete / skipped (issue open) / unknown (API error)
- Asks for confirmation before deleting anything
- Uses `git worktree remove --force` — never `rm -rf`
- Runs `git worktree prune` after deletions to clean up stale git references

**This skill must be run from the parent folder, not from inside `main`.**

---

## Quick Reference

| Step | Where | Command |
|------|-------|---------|
| Plan the ticket | `main\` | `/plan` then paste the planning prompt |
| Write plan back | `main\` | Paste the update prompt (still in plan mode) |
| Create worktree | `main\` | `/new-worktree [NUMBER]` |
| Start implementation | worktree tab | `/start-ticket` |
| Commit locally | worktree tab | `/git-commit` |
| Push, PR, and close | worktree tab | `/close-ticket` |
| Clean up old worktrees | parent folder | `/cleanup-worktrees` |

---

## Folder Structure

```
Manufacturing-Inventory-System\
├── main\                                    ← primary repo, never delete
│   └── .claude\
│       ├── CLAUDE.md
│       ├── agents\
│       │   ├── managing.md
│       │   ├── backend-implement.md
│       │   └── frontend-implement.md
│       └── skills\
│           ├── start-ticket\SKILL.md
│           ├── new-worktree\SKILL.md
│           ├── close-ticket\SKILL.md
│           └── git-commit\SKILL.md
├── {number}-{slug}-{yyyymmdd}\             ← worktree folders (one per ticket)
├── start-worktree.ps1                       ← launched automatically by /new-worktree
└── .claude\
    ├── README.md                            ← this file
    ├── CLAUDE.md
    └── skills\
        └── cleanup-worktrees\SKILL.md
```
