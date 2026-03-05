---
name: git-commit
description: Generate commit message options, let user select and refine, then commit locally. Never pushes.
---

## Purpose

Help the user craft a good commit message by generating options, allowing selection and refinement, then committing locally.

## Process

### 1. Gather Changes

Run:

```
git diff --staged
git diff
git status
```

If anything is already staged, generate options only for the staged changes and commit only those. Do not run `git add -A`.

If nothing is staged, proceed normally and run `git add -A` before committing.

Review all modified, added, and deleted files.

---

### 2. Generate 3 Options

Based on the changes, generate exactly 3 commit message options. Each option includes:

- A short title (imperative, under 72 characters)
- A 2-5 bullet description of what changed and why

Present them clearly:

```
Option 1:
  Title: ...
  - ...
  - ...

Option 2:
  Title: ...
  - ...
  - ...

Option 3:
  Title: ...
  - ...
  - ...

(or type "cancel" to exit at any time)
Type a number to select, or type "more" for 3 new options.
(Up to 9 options shown at a time — new options replace the oldest)
```

---

### 3. Handle Selection or More Options

**If user types "cancel":** stop immediately, do not commit anything.

**If user types "more":**
- Generate 3 new options
- Add them to the list, replacing the oldest if total exceeds 9
- Show the updated list and ask again

**If user types a number:**
- Confirm the selected option and show it clearly
- Ask:

```
What would you like to do?

1. Submit as-is
2. Edit directly (type your changes, then press Enter to commit)
3. Edit with grammar and spell check (type your changes, I will clean them up before committing)

(or type "cancel" to exit at any time)
```

---

### 4. Handle the Choice

**If user types "cancel":** stop immediately, do not commit anything.

**Option 1 — Submit as-is:**
- Commit immediately with the selected title and description
- Done

**Option 2 — Edit directly:**
- Ask the user to type their revised title and/or description
- Commit exactly what they typed, no changes
- Done

**Option 3 — Edit with grammar and spell check:**
- Ask the user to type their revised title and/or description
- Clean up spelling, grammar, and phrasing while preserving the meaning and the user's intent
- Show the cleaned version and loop back to:

```
1. Submit as-is
2. Edit directly
3. Edit with grammar and spell check

(or type "cancel" to exit at any time)
```

Repeat until the user submits or cancels.

---

### 5. Commit

If changes were already staged, commit as-is:

```
git commit -m "<title>" -m "<description>"
```

If nothing was staged, stage everything first:

```
git add -A
git commit -m "<title>" -m "<description>"
```

Confirm the commit was successful and show the commit hash.

---

## Safety

- Never push
- Never modify source files
- Never assume approval — always wait for explicit selection
- If anything is unclear, ask before committing
- If the user types "cancel" at any point, stop immediately and do not commit anything