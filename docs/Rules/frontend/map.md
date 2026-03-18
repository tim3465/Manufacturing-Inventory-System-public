---
category: frontend-rules
area: map
layer: frontend
activation: passive
summary: Defines frontend file structure, folder layout, routing organization, API client placement, and component/page/modal patterns for the Angular application.
keywords:
  - angular structure
  - frontend map
  - file layout
  - folder layout
  - standalone components
  - api clients
  - routing
  - pages and modals
use-when:
  - creating new frontend features
  - deciding where files belong
  - organizing angular code
  - following frontend project structure
---


# Frontend File & Folder Map

Angular 21 (standalone components) + Tailwind CSS + Signals. No component library.

---

## Top-Level Layout

```
frontend/angular/src/app/
├── app.ts                    (root component)
├── app.config.ts             (providers: HttpClient + authInterceptor, Router)
├── app.routes.ts             (all route definitions)
├── app.html / app.css
├── core/                     (app-wide singletons — never feature-specific)
└── features/                 (pages and page-scoped components by role/workflow)
```

No `shared/` folder exists. Reusable non-feature code lives in `core/`.

---

## `core/` — What Goes Here

Everything in `core/` is **singleton, app-wide infrastructure**. It never contains feature logic or page-specific UI.

```
core/
├── api/                      (one API client per backend controller)
├── auth/                     (AuthService, guards, interceptor, role definitions)
├── dtos/                     (TypeScript interfaces mirroring backend DTOs)
├── layout/                   (shell layout, header, sidebar, nav config)
├── theme/                    (ThemeService for dark/light mode)
└── ui/                       (reusable UI primitives — toast system)
```

### `core/api/` — API Clients

One file per backend controller. Each is an `@Injectable({ providedIn: 'root' })` service.

```
api/
├── api-client.service.ts     (base HTTP wrapper — get, getCached, post, patch, delete)
├── api-cache.service.ts      (TTL + inflight dedup cache)
├── index.ts                  (barrel export for all API clients)
├── auth.api.ts               → AuthController
├── users.api.ts              → UsersController
├── machines.api.ts           → MachinesController
├── materials.api.ts          → MaterialsController
├── orders.api.ts             → OrdersController
├── parts.api.ts              → PartsController
├── jobs.api.ts               → JobsController
├── shifts.api.ts             → ShiftsController
├── stock-lots.api.ts         → StockLotsController
├── stock-lot-adjustments.api.ts → StockLotAdjustmentsController
└── shipping-receiving.api.ts → ShippingReceivingController (workflow)
```

**Naming:** `{entity-kebab}.api.ts` for CRUD controllers, `{workflow-kebab}.api.ts` for workflow controllers.

**Pattern for every API client:**

```typescript
const _PATH = '/users';

@Injectable({ providedIn: 'root' })
export class UsersApi {
  constructor(private readonly api: ApiClient) {}

  listActive(): Observable<UserDto[]> {
    return this.api.getCached<UserDto[]>(`${_PATH}`);
  }

  create(dto: CreateUserRequestDto): Observable<CreateUserResponseDto> {
    return this.api.post<CreateUserResponseDto>(`${_PATH}`, dto).pipe(
      tap(() => {
        this.api.clearGetCache(`${_PATH}`);
        this.api.clearGetCache(`${_PATH}/all`);
      })
    );
  }
}
```

**Rules:**
- Read methods use `getCached()` (5-minute TTL + inflight dedup)
- Write methods (`post`, `patch`) clear related cache entries via `clearGetCache()` in a `tap()`
- Workflow API clients may clear cache for related entity paths (e.g., `ShippingReceivingApi.receive()` clears stock-lots cache)
- All paths are relative — `ApiClient` prepends `environment.apiBaseUrl`
- `ApiClient` does NOT attach auth tokens — that's the interceptor's job

### `core/auth/` — Authentication & Authorization

```
auth/
├── auth.service.ts           (JWT storage, decode, role parsing, login/logout)
├── auth.interceptor.ts       (attaches Bearer token, handles 401 → logout)
├── auth.guard.ts             (redirects unauthenticated users to /login)
├── role.guard.ts             (redirects users without required role to /dashboard)
└── roles.ts                  (Roles constant, Role type, ALL_ROLES, ROLE_LABELS)
```

**AuthService** is the single source of truth for auth state:
- Stores JWT in `localStorage` under key `cncapp.accessToken`
- Decodes JWT payload client-side to extract `role` claims and display name
- Provides: `isLoggedIn()`, `getRoles()`, `isAdmin()`, `hasAnyRole()`, `login()`, `logout()`
- On `login()`: stores token → clears API cache → navigates to dashboard (or returnUrl)
- On `logout()`: removes token → clears API cache → navigates to `/login`

**authInterceptor** (functional interceptor):
- Attaches `Authorization: Bearer <token>` to all `/api/` requests except `/api/auth/login`
- On 401 response → calls `auth.logout()`

**Guards** (functional `CanMatchFn`):
- `authGuard` — checks `isLoggedIn()`, redirects to `/login` with `returnUrl`
- `roleGuard` — checks `hasAnyRole()` against `route.data.roles`, redirects to `/dashboard`

### `core/dtos/` — TypeScript DTO Interfaces

```
dtos/
├── auth/
│   ├── login-request.dto.ts
│   └── login-response.dto.ts
├── users/
│   ├── index.ts                              (barrel)
│   ├── user.dto.ts
│   ├── create-user-request.dto.ts
│   ├── create-user-response.dto.ts
│   ├── user-roles.dto.ts
│   └── update-user-roles-request.dto.ts
├── shipping-receiving/
│   ├── index.ts                              (barrel)
│   ├── receive-shipment-request.dto.ts
│   └── receive-shipment-response.dto.ts
├── stock-lots/
│   └── stock-lot.dto.ts
├── stock-lot-adjustments/
│   └── create-stock-lot-adjustment-request.dto.ts
├── jobs/
│   └── dto-placeholder.ts                    (stub)
├── machines/
│   └── dto-placeholder.ts                    (stub)
├── materials/
│   └── dto-placeholder.ts                    (stub)
├── orders/
│   └── dto-placeholder.ts                    (stub)
├── parts/
│   └── dto-placeholder.ts                    (stub)
└── shifts/
    └── dto-placeholder.ts                    (stub)
```

**Naming:** `{name-kebab}.dto.ts`. Folder per backend entity using kebab-case of the plural name.

**Rules:**
- DTOs are plain TypeScript `interface` or `type` declarations — no classes
- Enum-like values (e.g., `StockLotCondition`) live in the DTO file that uses them, with label maps
- Barrel `index.ts` files are used when a folder has 3+ DTOs for cleaner imports
- Placeholder files exist for entities whose frontend DTOs haven't been built yet

### `core/layout/` — App Shell

```
layout/
├── app-shell-layout/         (wraps sidebar + header + <router-outlet>)
├── app-header/               (app name, theme toggle, user menu)
├── app-sidebar/              (role-filtered nav links, active route highlight)
├── nav.config.ts             (TOP_NAV + ROLE_NAV_GROUPS arrays)
├── role.model.ts             (Role type re-export for layout)
└── role.service.ts           (UI-only role selection for sidebar filtering)
```

### `core/ui/` — Reusable UI Primitives

```
ui/
└── toast/
    ├── toast.service.ts      (signal-based: success/error/info/warning + auto-dismiss)
    ├── toast.model.ts        (Toast type, ToastPhase, ToastType)
    ├── toast-host.component.ts/html/css  (renders toast stack)
```

**ToastService** handles backend error extraction — parses RFC7807 ProblemDetails, validation error shapes, and plain string errors.

---

## `features/` — What Goes Here

Pages and page-scoped UI organized by **role/workflow**, not by database table.

```
features/
├── auth/                     (public, no shell)
│   └── login.page.ts/html/css
├── dashboard/                (all authenticated users)
│   └── dashboard.page.ts/html/css
├── machinist/                (Machinist role)
│   ├── my-jobs.page.ts/html/css
│   └── log-shift.page.ts/html/css
├── shipping/                 (Shipping role)
│   ├── receive-material/
│   │   └── receive-material.page.ts/html/css
│   └── inventory/
│       ├── inventory.page.ts/html/css
│       ├── receive-shipment-modal/
│       │   └── receive-shipment-modal.component.ts/html/css
│       └── adjust-bars-modal/
│           └── adjust-bars-modal.component.ts/html/css
├── supervisor/               (Supervisor role)
│   ├── orders.page.ts/html/css
│   └── job-planning.page.ts/html/css
└── admin/                    (Admin role)
    ├── machines/
    │   └── machines.page.ts/html/css
    ├── settings/
    │   └── settings.page.ts/html/css
    └── users/
        ├── users.page.ts/html/css
        ├── add-user-modal/
        │   └── add-user-modal.component.ts/html/css
        ├── manage-user-roles-modal/
        │   └── manage-user-roles-modal.component.ts/html/css
        └── inactivate-user-modal/
            └── inactivate-user-modal.component.ts/html/css
```

---

## Page vs. Modal Naming Convention

| Type | Naming | Location |
|------|--------|----------|
| Page | `{name}.page.ts/html/css` | Directly under role folder, or in its own subfolder if it has child modals |
| Modal | `{name}.component.ts/html/css` | Own subfolder under the page that opens it |

Pages are the routable components. Modals are non-routable child components owned by one specific page.

**Simple pages** (no modals): files sit directly in the role folder.
```
machinist/
├── my-jobs.page.ts
├── my-jobs.page.html
└── my-jobs.page.css
```

**Pages with modals**: page gets its own subfolder; modals nest inside it.
```
admin/users/
├── users.page.ts/html/css
├── add-user-modal/
│   └── add-user-modal.component.ts/html/css
└── inactivate-user-modal/
    └── inactivate-user-modal.component.ts/html/css
```

---

## End-to-End Wiring (Add User Example)

This traces the flow from button click to backend and back:

```
1. users.page.html            → "Add user" button → openAddUser()
2. users.page.ts              → sets isAddUserOpen signal to true
3. users.page.html            → @if renders <app-add-user-modal>
4. add-user-modal.component   → form with Reactive Forms + Validators
5. add-user-modal.component   → onSubmit() builds CreateUserRequestDto
6. add-user-modal.component   → calls usersApi.create(dto)
7. core/api/users.api.ts      → api.post('/users', dto) + clears list caches
8. core/api/api-client.ts     → HttpClient.post(baseUrl + '/users', dto)
9. core/auth/auth.interceptor → attaches Bearer token to request
10. proxy.conf.json            → proxies /api → https://localhost:7136
11. Backend UsersController    → UserService.CreateAsync → Identity + Domain user
12. Response flows back
13. add-user-modal.component   → toast.success() + emits (created) + (closed)
14. users.page.ts              → (created) handler calls loadUsers()
15. users.page.ts              → usersApi.listActive() refreshes user list
```

---

## Routing Structure

```
/login                          → AuthLoginPageComponent (public, no shell)
/                               → redirects to /login
/dashboard                      → DashboardPageComponent
/machinist/my-jobs              → MyJobsPageComponent        [Machinist, Admin]
/machinist/log-shift            → LogShiftPageComponent       [Machinist, Admin]
/shipping/receive-material      → ReceiveMaterialPageComponent [Shipping, Admin]
/shipping/inventory             → InventoryPageComponent       [Shipping, Admin]
/supervisor/orders              → OrdersPageComponent          [Supervisor, Admin]
/supervisor/job-planning        → JobPlanningPageComponent     [Supervisor, Admin]
/admin/machines                 → MachinesPageComponent        [Admin]
/admin/users                    → UsersPageComponent           [Admin]
/admin/settings                 → SettingsPageComponent        [Admin]
/**                             → redirects to /login
```

All routes except `/login` are children of `AppShellLayoutComponent` and protected by `authGuard`. Role-specific route groups use `roleGuard` with `data: { roles: [...] }`. Admin role has access to everything via `hasAnyRole()` which returns `true` for admins.

---

## Component Patterns

### State Management
- **Angular Signals** for all component state (`signal()`, `computed()`)
- No NgRx, no external state library
- `AuthService` caches decoded JWT roles/display-name in memory

### Forms
- **Reactive Forms** with `FormBuilder.nonNullable.group()`
- Validation via Angular `Validators`
- `form.markAllAsTouched()` on invalid submit to show all errors
- `form.getRawValue()` to build the DTO

### Data Loading
- Pages call API in `ngOnInit()` via `.subscribe()`
- Loading/error state tracked via `signal<boolean>` and `signal<string | null>`
- Lists use `computed()` to transform raw DTOs into display rows

### Modal Pattern
- Parent page owns `isModalOpen` signal
- Parent template uses `@if (isModalOpen())` to render/destroy modal
- Modal communicates back via `@Output()` EventEmitters: `(closed)`, `(created)`, `(updated)`, `(inactivated)`, etc.
- After a successful mutation, modal emits both the action event and `(closed)`
- Parent handler refreshes data by re-calling the list API

### Error Handling
- `ToastService.errorMessage(err)` extracts messages from `HttpErrorResponse` (RFC7807 ProblemDetails, validation errors, plain strings)
- Modals show toast on error and re-enable the submit button via `submitting.set(false)`

### Styling
- Tailwind utility classes only — no component library
- CSS custom properties for theme: `var(--bg)`, `var(--fg)`, `var(--surface)`, `var(--border)`
- Dark mode via `ThemeService` toggling a class on `<body>`

---

## Hard Rules

1. **`core/` is for app-wide singletons only** — never feature-specific logic
2. **`features/` is organized by role/workflow**, not by database table
3. **One API client file per backend controller** — named `{entity-kebab}.api.ts`
4. **DTOs are plain interfaces**, not classes — one file per DTO or closely related group
5. **Pages use `.page.ts` suffix**, modals and other components use `.component.ts`
6. **Modals live under the page that owns them** in their own subfolder
7. **All components are standalone** — no NgModules
8. **Signals for state**, Reactive Forms for forms, no external state management
9. **Write operations must clear related caches** via `api.clearGetCache()` in a `tap()`
10. **No component libraries** — Tailwind + CSS custom properties only
