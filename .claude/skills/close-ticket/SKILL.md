# Skill: close-ticket

Clean up a finished ticket: ensure everything is committed, push the branch, open a PR linked to the issue, post a commit summary comment on the issue, then safely delete the worktree from the local machine.

---

## Trigger

User runs `/close-ticket` with no arguments.

---

## Configuration

```
REPO        = tim3465/Manufacturing-Inventory-System
BASE_BRANCH = claude-agent-workflows-20260303
```

> **To target a different base branch:** change `BASE_BRANCH` to the branch name you want the PR to merge into (e.g. `ai-orchestration-setup`). All references to the base branch in this skill use `BASE_BRANCH` — this is the only line you need to edit.

---

## Steps

### Step 1 — Verify location (safety guard)

Check that the current working directory does **not** end in `\main` or `/main`.

If it does, stop immediately:

> This skill cannot be run from the `main` folder. Navigate into the worktree for the ticket you want to close.

Also read the issue number from the environment:

```bash
echo $env:CURRENT_ISSUE
```

If `CURRENT_ISSUE` is not set or empty, stop:

> No active issue found. This skill must be run from a worktree session opened with /new-worktree.

Store: `ISSUE_NUMBER = $env:CURRENT_ISSUE`

Also record the current working directory as `WORKTREE_PATH` and the current branch name:

```bash
git branch --show-current
```

Store as `BRANCH_NAME`.

---

### Step 2 — Check for uncommitted changes

Run:

```bash
git status --porcelain
```

If any output is returned (staged or unstaged changes exist), ask the user:

```
You have uncommitted changes. What would you like to do?

1. Run /git-commit now (recommended)
2. Cancel /close-ticket and handle it manually
```

**If user chooses 1:** Invoke the `/git-commit` skill immediately. Once it completes successfully, continue to Step 3.

**If user chooses 2:** Stop cleanly:
> Cancelled. No changes have been made.

Only continue to Step 3 if the working tree is clean.

---

### Step 3 — Check for unpushed commits

Run:

```bash
git log origin/{BASE_BRANCH}..HEAD --oneline
```

If this returns output, there are local commits not yet pushed. Note this — push will happen in Step 4.

If this returns nothing AND the branch does not exist on origin yet, the push in Step 4 will create it.

---

### Step 4 — Check for an existing PR

```bash
gh pr list --repo tim3465/Manufacturing-Inventory-System --head {BRANCH_NAME} --json number,title,body,state
```

**Scenario A — No PR exists:**
Continue to Step 5 (push + create PR).

**Scenario B — PR already exists:**
- Check whether `Closes #{ISSUE_NUMBER}` appears in the PR body (case-insensitive).
- If it is missing, patch the PR description to append `Closes #{ISSUE_NUMBER}`:

```bash
gh pr edit {PR_NUMBER} --repo tim3465/Manufacturing-Inventory-System --body "{existing_body}

Closes #{ISSUE_NUMBER}"
```

Print:
```
✓ PR #{PR_NUMBER} already exists — added "Closes #{ISSUE_NUMBER}" to description.
```

- If `Closes #{ISSUE_NUMBER}` is already present, print:
```
✓ PR #{PR_NUMBER} already exists and is correctly linked to issue #{ISSUE_NUMBER}.
```

Then skip to Step 6.

---

### Step 5 — Push and create PR

**Push the branch:**

```bash
git push origin {BRANCH_NAME}
```

If the push fails, stop immediately:

> Push failed. The worktree has NOT been deleted. Resolve the push error and run /close-ticket again.

Show the raw error output.

**Collect commit log for PR description:**

```bash
git log origin/{BASE_BRANCH}..{BRANCH_NAME} --oneline
```

**Create the PR:**

```bash
gh pr create \
  --repo tim3465/Manufacturing-Inventory-System \
  --base {BASE_BRANCH} \
  --head {BRANCH_NAME} \
  --title "{issue_title}" \
  --body "## Summary

Closes #{ISSUE_NUMBER}

## Commits

{commit_log_as_bullet_list}"
```

Format each commit log line as a markdown bullet: `- {hash} {message}`

If PR creation fails, stop immediately:

> PR creation failed. The worktree has NOT been deleted. Resolve the error and run /close-ticket again.

Show the raw error output.

Print:
```
✓ Branch pushed successfully.
✓ PR created: {pr_url}
```

---

### Step 6 — Post a comment on the GitHub issue

Collect the full commit log for this branch:

```bash
git log origin/{BASE_BRANCH}..{BRANCH_NAME} --oneline
```

Post a comment on the issue:

```bash
gh issue comment {ISSUE_NUMBER} \
  --repo tim3465/Manufacturing-Inventory-System \
  --body "## Branch closed: \`{BRANCH_NAME}\`

A PR has been opened: {pr_url}

**Commits on this branch:**

{commit_log_as_bullet_list}

This issue will close automatically when the PR is merged into \`{BASE_BRANCH}\`."
```

If the comment fails, warn the user but do **not** stop:

> Warning: Could not post a comment to the issue. You may want to add one manually. Continuing with worktree cleanup.

Print:
```
✓ Comment posted to issue #{ISSUE_NUMBER}.
```

---

### Step 7 — Confirm before deleting the worktree

Print a summary of everything completed so far, then ask:

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  Ready to delete local worktree

  Branch    : {BRANCH_NAME}
  PR        : {pr_url}
  Issue     : #{ISSUE_NUMBER}
  Folder    : {WORKTREE_PATH}
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Everything has been pushed and the PR is open.
Delete the local worktree folder now? (yes / no)
```

If the user says no:
> Skipped. Your worktree folder is still at: {WORKTREE_PATH}
Then stop.

---

### Step 8 — Delete the worktree

From the `main` repo (not from inside the worktree folder), run:

```bash
git -C "C:\dev\projects\Manufacturing-Inventory-System\main" worktree remove "{WORKTREE_PATH}" --force
```

If this fails, stop and show the raw error:

> Worktree removal failed. The folder may still exist at: {WORKTREE_PATH}
> You can remove it manually with:
> git worktree remove "{WORKTREE_PATH}" --force

If it succeeds, print:

```
✓ Worktree deleted: {WORKTREE_PATH}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  Ticket #{ISSUE_NUMBER} is closed out.

  ✓ Changes committed
  ✓ Branch pushed to origin
  ✓ PR open and linked to issue
  ✓ Commit summary posted to issue
  ✓ Worktree deleted

  Next step: review and merge the PR on GitHub.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

## Error Cases

| Situation | Response |
|-----------|----------|
| Run from `main` folder | Stop. Safety guard. |
| `CURRENT_ISSUE` not set | Stop. Tell user to use /new-worktree to open sessions. |
| Uncommitted changes found | Ask: run /git-commit (auto-invoked) or cancel. |
| Push fails | Stop. Do not delete worktree. Show raw error. |
| PR creation fails | Stop. Do not delete worktree. Show raw error. |
| PR already exists | Check for `Closes #` link, patch if missing, skip to comment step. |
| Issue comment fails | Warn but continue to Step 7. |
| Worktree removal fails | Show raw error and manual removal command. |
| User declines deletion | Exit cleanly. Folder is safe. |

---

## Safety Rules

- Never delete the worktree if the push has not succeeded.
- Never delete the worktree if PR creation has not succeeded.
- Never run worktree removal against the `main` folder.
- The local branch is removed as part of `git worktree remove` — no separate branch delete needed.
- This skill never modifies source files.
