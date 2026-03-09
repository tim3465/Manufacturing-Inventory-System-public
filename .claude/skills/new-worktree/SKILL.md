# Skill: new-worktree

Create a Git worktree for a GitHub issue. Fetches the issue title, builds a clean folder/branch name, sets up the worktree as a sibling of `main`, and opens a new terminal tab with Claude Code ready to go.

---

## Trigger

User runs `/new-worktree` with a GitHub issue number.

Example:
```
/new-worktree 42
```

---

## Configuration

```
REPO = tim3465/Manufacturing-Inventory-System
SCRIPT = C:\dev\projects\Manufacturing-Inventory-System\start-worktree.ps1
```

---

## Steps

### Step 1 — Get the issue title

```bash
gh issue view {number} --repo tim3465/Manufacturing-Inventory-System --json title,state
```

If the issue is not found, stop and tell the user:
> Issue #{number} not found in tim3465/Manufacturing-Inventory-System. Check the number and try again.

If the issue state is `closed`, warn the user:
> Issue #{number} is already closed. Do you still want to create a worktree for it? (yes/no)

Only continue if user confirms yes.

---

### Step 2 — Build the name

From the issue title:
1. Lowercase the entire title
2. Remove all characters that are not letters, numbers, or spaces
3. Split into words, take the first 5
4. Join with hyphens
5. Prepend the issue number
6. Append today's date as `yyyymmdd`

Format: `{number}-{slug}-{yyyymmdd}`

Examples:
- Issue 42: "Add Material Catalog Page" → `42-add-material-catalog-page-20260309`
- Issue 87: "Fix: Stock Lot Adjustment not saving notes" → `87-fix-stock-lot-adjustment-not-saving-20260309`
- Issue 103: "Inventory page — migrate to Angular Signals" → `103-inventory-page-migrate-to-angular-20260309`

---

### Step 3 — Verify location

Check that the current working directory ends in `\main` or `/main`.

If not, stop and tell the user:
> This skill must be run from inside the `main` folder.
> Current location: {cwd}
> Please run: cd C:\dev\projects\Manufacturing-Inventory-System\main

---

### Step 4 — Resolve the sibling path

The worktree folder goes one level up from `main`, as a sibling.

```
parent = parent directory of current working directory
worktree_path = {parent}\{name}
```

Example:
- cwd: `C:\dev\projects\Manufacturing-Inventory-System\main`
- parent: `C:\dev\projects\Manufacturing-Inventory-System`
- worktree_path: `C:\dev\projects\Manufacturing-Inventory-System\42-add-material-catalog-page-20260309`

---

### Step 5 — Create the worktree

```bash
git worktree add "{worktree_path}" -b "{name}"
```

This creates the folder and a new branch with the same name.

If this fails, show the raw error and stop. Do not attempt to recover automatically.

---

### Step 6 — Open new terminal tab and launch Claude Code

Print:

```
✓ Worktree created successfully

  Folder : {worktree_path}
  Branch : {name}
  Issue  : #{number} — {title}

Opening new terminal tab...
```

Then run:

```bash

powershell -Command "Start-Process -FilePath 'wt' -ArgumentList @('-w', '0', 'new-tab', '--title', '{name}', '-d', '{worktree_path}', 'pwsh', '-NoExit', '-ExecutionPolicy', 'Bypass', '-File', 'C:\dev\projects\Manufacturing-Inventory-System\start-worktree.ps1')"
```

If this fails, print:

```
Could not open a new tab automatically. Open a new terminal tab manually and run:

  cd {worktree_path}
  claude
```

---

## Error Cases

| Situation | Response |
|-----------|----------|
| Issue number not provided | Ask: "Which issue number do you want to create a worktree for?" |
| Issue not found on GitHub | Stop. Tell user to check the number. |
| Issue is closed | Warn and ask for confirmation before continuing. |
| Not running from `main` | Stop. Print correct path to cd into. |
| Branch already exists | Stop. Tell user: "Branch {name} already exists. Has this worktree already been created?" |
| Worktree folder already exists | Stop. Tell user the folder already exists and show its path. |
| `git worktree add` fails for any other reason | Show raw error. Stop. |
