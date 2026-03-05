# CNC Shop Inventory Management System

Full-stack manufacturing inventory system. .NET 8 backend + Angular 21 frontend. Clean Architecture with strict layer boundaries.

---

## Project Structure

```
Manufacturing-Inventory-System/
├── backend/          # .NET 8 Clean Architecture solution
├── frontend/         # Angular 21 standalone components
├── docs/Rules/       # Architecture rules and maps (source of truth)
├── .claude/skills/   # Claude Code skills (slash commands)
└── CLAUDE.md         # This file
```

### Backend layers

```
backend/CncApp/
├── CncApp.Api/               # Controllers
├── CncApp.Application/       # Services, DTOs, AutoMapper
├── CncApp.Infrastructure/    # EF Core, Repositories
├── CncApp.Domain/            # Entities, Guards, DomainException
├── CncApp.Domain.Tests/      # Domain invariant tests
└── CncApp.Application.Tests/ # Service workflow tests
```

### Frontend structure

```
frontend/angular/src/app/
├── core/       # App-wide singletons (api, auth, layout, ui, dtos, theme)
└── features/   # Pages organized by role (machinist, shipping, supervisor, admin)
```

No `shared/` folder exists. Reusable non-feature code lives in `core/`.

---

## Rules and Architecture Maps (always read before making changes)

These are the source of truth. Read the relevant file before starting any task.

| What | File |
|------|------|
| Backend folder structure, namespaces, hard rules | `docs/Rules/backend/map.md` |
| Backend controller rules, domain rules, AutoMapper, DI | `docs/Rules/backend/rules.md` |
| Domain and application test rules | `docs/Rules/backend/test-rules.md` |
| Frontend folder structure, routing, component patterns | `docs/Rules/frontend/map.md` |
| Frontend member visibility, signals, caching, forms | `docs/Rules/frontend/rules.md` |

**Golden references:**
- Backend single-entity: Machines slice
- Backend workflow: ShippingReceiving slice
- Frontend: Users page + modals (admin/users)

---

## Hard Rules (memorize these)

- One method per file in Commands/Queries folders
- No `.Commands` / `.Queries` in namespaces
- Repositories never call `SaveChangesAsync` — the service does
- Repository mutations call domain methods, never set properties directly
- Services never catch domain exceptions — they bubble to GlobalExceptionHandler
- Controllers inject concrete services, not interfaces
- Workflow services own all transaction boundaries
- `StockLot.AmountOfBars` only changes through a `StockLotAdjustment` in the same transaction
- No `shared/` folder in frontend — `core/` handles all app-wide singletons
- All Angular components are standalone — no NgModules

---

## Available Skills

Skills are invoked with `/skill-name`. Always read the relevant rule files before a skill runs.

| Slash Command | What It Does |
|---------------|--------------|
| `/backend-implement` | Recon → Plan → Implement a backend change |
| `/backend-compose-services` | Compose existing services into a workflow |
| `/backend-add-table` | Add a new entity, repository, service, and migration |
| `/backend-write-tests` | Write or update backend tests |
| `/frontend-implement` | Implement an Angular feature or component |
| `/frontend-ui-contract-check` | Verify frontend matches backend API contract |
| `/frontend-write-tests` | Write or update frontend tests |
| `/integration-review` | Review integration between frontend and backend |
| `/shared-spec-to-plan` | Convert a spec or ticket into an implementation plan |
| `/shared-summarize-changes` | Summarize what changed and why |
| `/shared-create-pr-description` | Generate a PR description from recent changes |

---

## Tech Stack

**Backend:** .NET 8, ASP.NET Core, EF Core, SQL Server, ASP.NET Identity, JWT, AutoMapper, xUnit, Moq

**Frontend:** Angular 21 (standalone), Angular Signals, Reactive Forms, Tailwind CSS

**API contracts:** NSwag (backend → frontend type generation)

**Tickets:** GitHub Issues

---

## Key Patterns

- Services registered and injected as **concrete types** (not interfaces)
- Repositories use interfaces (`I{Entity}Repository`)
- Partial classes — one method per file in Commands/Queries
- Domain tests: entity invariants only, no mocks, no DB
- Application tests: service workflows only, mock repos, never re-test domain rules
- Angular signals for all component state
- Reactive Forms with `FormBuilder.nonNullable.group()`
- API cache TTL 5 minutes, write operations clear related cache keys
