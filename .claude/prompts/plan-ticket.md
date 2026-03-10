# plan-and-update-ticket.md
# Full planning workflow — run this before starting any implementation session.

---

## Step 1 — Start plan mode

In Claude Code, type:

```
/plan
```

Then paste this prompt with the issue number filled in:

```
Fetch GitHub issue #[NUMBER] from repo timjohnmontague/Manufacturing-Inventory-System. Read the following reference files before planning: backend-map.md, backend-rules.md, frontend-map.md, frontend-rules.md, and test-rules.md. Produce a full implementation plan — backend first, then frontend. Include file paths, method signatures, and any new DTOs or entities needed. Do not write any code yet.
```

---

## Step 2 — Review and adjust

Read the plan. Ask for adjustments as needed. Repeat until the plan looks right.

---

## Step 3 — Write the plan back to the ticket

Once the plan is finalized, paste this prompt with the issue number filled in:

```
Update GitHub issue #[NUMBER] on repo timjohnmontague/Manufacturing-Inventory-System with the finalized implementation plan from this session. Replace the existing issue body with the full plan as we have agreed on it. Do not summarize or shorten it — write the complete plan. Do not implement any code.
```

---

## Step 4 — Stop

Exit (ESC) plan mode without implementing anything. The ticket now holds the plan. Implementation happens later via /new-worktree → /start-ticket.