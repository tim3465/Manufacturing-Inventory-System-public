---
name: start-dev
description: Launch the full dev environment — runs npm install + ng serve for the frontend and dotnet run for the backend, each in its own terminal window.
---

## Purpose

Start the full development environment for this project with a single command.

## Process

### 1. Locate the project root

The project root is the directory that contains the `.claude/` folder. This is NOT necessarily named `main` — the skill must work in any worktree or folder name.

Determine the project root by resolving the path of this skill file:
- This file lives at `.claude/skills/start-dev/SKILL.md`
- Therefore the project root is three levels up from this file

### 2. Run the startup script

Run the following command from anywhere (the script uses its own location to resolve paths):

```powershell
powershell -ExecutionPolicy Bypass -File ".claude/scripts/start-dev.ps1"
```

Run this from the project root directory.

### 3. Report to the user

Once the script launches, confirm:

```
Dev environment is starting.

  Backend:  https://localhost:7136  (dotnet run — CncApp.Api)
  Frontend: http://localhost:4200   (ng serve)

Two terminal windows have opened. The frontend will be ready after npm install completes.
```

## Notes

- The script opens two new PowerShell windows so both processes remain visible with their logs
- The frontend window runs `npm install` before `npm start` every time — this is safe and keeps dependencies in sync
- The backend listens on `https://localhost:7136`; the frontend proxy forwards all `/api/*` requests there automatically
- This skill does not wait for the processes to finish — it simply launches them
