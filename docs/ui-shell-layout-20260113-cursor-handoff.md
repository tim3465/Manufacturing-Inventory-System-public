# UI Shell Layout (Header + Sidebar + Main) — Cursor Handoff
**Branch:** `ui-shell-layout-20260113`  
**App:** Angular 21 (standalone-first) + Tailwind v3 + ThemeService + ApiClient (scaffold already complete)

> This branch is ONLY for the **UI shell** (layout infrastructure).  
> Do **not** build backend wiring, auth, or real feature pages yet.

---

## 1) Goal (what “done” means)

When this branch is complete, the Angular app should have:

- A persistent **top header** (app title, theme toggle, mock role selector, user menu placeholder)
- A persistent **left sidebar nav** (nav items filtered by selected role)
- A **main content area** with `<router-outlet>` that renders placeholder pages
- Clean, consistent Tailwind layout and spacing (desktop-first, reasonable mobile behavior)
- No API calls required; everything can run offline with mock data

---

## 2) Non-negotiable constraints (do not go off the rails)

### Keep the scaffold stable
- Do NOT change Tailwind setup, theme tokens, ThemeService wiring, or ApiClient base behavior.
- No UI frameworks (Angular Material, Bootstrap, PrimeNG, etc.). Headless approach only.
- Do NOT redesign the folder boundaries (`core/`, `shared/`, `features/`).

### Scope boundaries for this branch
✅ Allowed:
- New layout components
- Nav configuration objects
- Mock “role selection” behavior (UI-only)
- Placeholder pages with dummy content
- Tailwind styling for layout and nav

❌ Not allowed (explicitly out of scope):
- Auth UX / login screens
- Permission enforcement / guards tied to backend
- State management libraries (NgRx, etc.)
- Real feature workflows (inventory, planning, shift logging logic)
- Calling the API / wiring to backend services
- Building full CRUD for any entity

---

## 3) Information architecture (workflows, not tables)

The UI is organized by **what people do**, not by what tables exist.

Initial roles (mock for now):
- `Machinist`
- `ShippingReceiving`
- `Supervisor`
- `Admin`

Nav groups and starter pages (placeholders only):

### Machinist
- My Jobs
- Log Shift

### Shipping / Receiving
- Receive Material
- Inventory

### Supervisor
- Orders
- Job Planning

### Admin
- Machines
- Users
- Settings

> Note: “StockLotAdjustments” is not a main nav page. It will be embedded later under Inventory/Receiving workflows.

---

## 4) Target file layout (create these)

Create the UI shell under `src/app/core/layout/` and keep pages under `src/app/features/`.

### Layout
- `src/app/core/layout/app-shell-layout.component.ts`
- `src/app/core/layout/app-header.component.ts`
- `src/app/core/layout/app-sidebar.component.ts`
- `src/app/core/layout/nav.config.ts`  (role-based nav definitions)
- `src/app/core/layout/role.model.ts` (role type + labels)
- `src/app/core/layout/role.service.ts` (UI-only role selection w/ localStorage)

### Shared UI bits (optional but nice)
- `src/app/shared/ui/page-shell.component.ts` (standard page header + toolbar slot)
- `src/app/shared/ui/card.component.ts` (simple wrapper)

### Placeholder feature pages (standalone)
- `src/app/features/dashboard/dashboard.page.ts`
- `src/app/features/machinist/my-jobs.page.ts`
- `src/app/features/machinist/log-shift.page.ts`
- `src/app/features/shipping/receive-material.page.ts`
- `src/app/features/shipping/inventory.page.ts`
- `src/app/features/supervisor/orders.page.ts`
- `src/app/features/supervisor/job-planning.page.ts`
- `src/app/features/admin/machines.page.ts`
- `src/app/features/admin/users.page.ts`
- `src/app/features/admin/settings.page.ts`

> Pages can be minimal: title + a few dummy cards + a simple table/list mock.

---

## 5) Routing plan (must be simple and stable)

Use a shell route so header/sidebar persist:

- `app.routes.ts` should have a layout route:
  - path: `""`
  - component: `AppShellLayoutComponent`
  - children: all feature routes

Add a default route:
- `"" -> /dashboard`

Each page route should be a direct component route (no lazy-loading needed yet unless you want it).

---

## 6) UI behavior requirements

### Header
Include:
- App name (left)
- Theme toggle button (calls existing ThemeService)
- Role selector (dropdown or segmented buttons) — **UI-only**
- User menu placeholder (icon/button; no behavior yet)

### Sidebar
- Shows nav items filtered by selected role
- Highlights active route
- Collapsible on small screens is optional (nice-to-have, not required)
- Use Tailwind for spacing/hover/active states

### Main area
- Standard content width container (e.g., `max-w-6xl` or similar)
- Consistent page padding
- Page title header

---

## 7) Acceptance checklist (you must verify)

From project root `frontend/angular`:

- `npm start` runs successfully
- Navigate between pages via sidebar without errors
- Theme toggle works (light/dark class on body changes)
- Role selection changes visible nav items and persists on reload
- No console errors



## 8) Notes for Cursor (prevent common mistakes)

- Do not introduce Angular Material or any component library.
- Do not rework ThemeService; just call it from the header toggle.
- Keep all behavior simple and local; no API calls.
- Keep naming consistent and boring; avoid overengineering.
