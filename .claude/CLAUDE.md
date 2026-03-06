# CNC Shop Inventory Management System

Full-stack manufacturing inventory system. .NET 8 backend + Angular 21 frontend. Clean Architecture with strict layer boundaries.

---

## Project Structure

```
Manufacturing-Inventory-System/
├── backend/CncApp/       # .NET 8 Clean Architecture solution
├── frontend/angular/     # Angular 21 standalone components
├── docs/Rules/           # Architecture rules and maps (source of truth)
├── .claude/agents/       # Claude Code agents
└── CLAUDE.md             # This file
```

---

## Rules and Maps (read before making changes)

| What | File |
|------|------|
| Backend structure, namespaces, layer rules | `docs/Rules/backend/map.md` |
| Backend patterns, DI, AutoMapper, controller rules | `docs/Rules/backend/rules.md` |
| Domain and application test rules | `docs/Rules/backend/test-rules.md` |
| Frontend structure, routing, component patterns | `docs/Rules/frontend/map.md` |
| Frontend signals, forms, caching, visibility rules | `docs/Rules/frontend/rules.md` |

**Golden references:**
- Backend single-entity: Machines slice
- Backend workflow: ShippingReceiving slice

---

## Available Agents

| Agent | What It Does |
|-------|--------------|
| `backend-implement` | Recon → Plan → Implement a backend feature or change |
| `frontend-implement` | Recon → Plan → Implement a frontend feature or change |
| `managing` | Orchestrate backend-implement → frontend-implement; gates new-table creation against an approved plan file |

## Available Skills

| Slash Command | What It Does |
|---------------|--------------|
| `/git-commit` | Summarize changes, propose commit message, confirm, then commit locally |

---

## Tech Stack

**Backend:** .NET 8, ASP.NET Core, EF Core, SQL Server, ASP.NET Identity, JWT, AutoMapper, xUnit, Moq

**Frontend:** Angular 21 (standalone), Angular Signals, Reactive Forms, Tailwind CSS

**API contracts:** NSwag

**Tickets:** GitHub Issues
