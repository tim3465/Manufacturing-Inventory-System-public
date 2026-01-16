# Auth (API) + Login (UI) — Current State & Wiring Plan

This document captures **what exists today** for authentication/roles in the backend API and the Angular frontend, and proposes a **minimal, best-practice wiring plan** to connect them (no implementation in this doc).

---

## Backend (API) — what exists today

### JWT + ASP.NET Core Identity
- **Identity store**: `AppDbContext` derives from `IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>` (Identity users + roles live in the same DB as the domain tables).
- **Auth middleware**: JWT Bearer auth is configured in `backend/CncApp/CncApp.Api/Program.cs` via `AddAuthentication().AddJwtBearer(...)` and `app.UseAuthentication(); app.UseAuthorization();`.
- **JWT settings**: configured under `Jwt` in `backend/CncApp/CncApp.Api/appsettings*.json` (`Issuer`, `Audience`, `Key`, `Minutes`).

### Login endpoint
- **Endpoint**: `POST /api/auth/login`
- **Controller**: `backend/CncApp/CncApp.Api/Controllers/AuthController.cs`
- **Request DTO**: `backend/CncApp/CncApp.Api/ApiDtos/LoginRequestDto.cs`
  - Fields: `Email`, `Password`
- **Response DTO**: `backend/CncApp/CncApp.Api/ApiDtos/LoginResponseDto.cs`
  - Fields: `AccessToken` (JWT)
- **Process** (high-level):
  - Find Identity user by email
  - Check password
  - Read Identity roles via `_userManager.GetRolesAsync(user)`
  - Generate JWT including:
    - `sub` = `IdentityUser<int>.Id` (stringified int)
    - `role` claims (added using `ClaimTypes.Role`)

### “Am I authenticated?” endpoint
- **Endpoint**: `GET /api/auth/ping`
- **Auth**: `[Authorize]`
- **Use**: returns 200 if the JWT is valid, otherwise 401.

### Current role enforcement pattern (already partially present)
Many controllers already use:
- `[Authorize]` at the controller level (e.g., `UsersController`)
- `[Authorize(Roles = "Admin")]` for admin-only operations (seen across several controllers)

### User provisioning + role assignment
There is an explicit “admin provisions users” path:
- **Create user**: `POST /api/users` (Admin-only)
  - Creates an Identity user + assigns Identity roles + creates a linked Domain `User`.
- **Update roles**: `PATCH /api/users/{id}` (Admin-only)
  - Replaces Identity roles via `IdentityProvisioningService.AssignRolesAsync`.

### Domain vs Identity (important)
- Identity user id (int) is the **source of truth** for authentication and role claims.
- Domain `User` is linked by `IdentityUserId` and is used for **audit fields**.
- `ICurrentUserService` resolves current Identity user id from `sub` claim and `AppDbContext.SaveChangesAsync` translates IdentityUserId → DomainUserId for auditing.

---

## Frontend (Angular) — what exists today

### Public vs app-shell route separation (UI-only)
- `/login` is **public** and renders *without* `AppShellLayoutComponent`.
- `/dashboard` (and all feature routes) render *with* `AppShellLayoutComponent`.
- Default route and unknown routes redirect to `/login`.

### Current login is mock / dummy
- Login page: `frontend/angular/src/app/features/auth/login.page.ts|html|css`
- “Auth service”: `frontend/angular/src/app/core/auth/mock-auth.service.ts`
  - Stores `cncapp.mockAuth=true` in `localStorage`
  - No backend call, no JWT, no server-verified identity
- Logout: implemented in header; clears mock flag and navigates to `/login`

### Existing role UI plumbing (currently UI-only)
- Role model: `frontend/angular/src/app/core/layout/role.model.ts`
  - UI roles: `Machinist | ShippingReceiving | Supervisor | Admin`
- Role service: `frontend/angular/src/app/core/layout/role.service.ts`
  - Stores selected role in `localStorage` (`cncapp.role`)
  - Currently acts like a UI toggle, not server-driven
- Navigation groups: `frontend/angular/src/app/core/layout/nav.config.ts`
  - Sidebar renders groupings for each UI role
  - Current sidebar does not appear to filter by server roles (it’s a “show all groups” UX right now)

---

## Key gaps / mismatches to resolve when wiring up real auth

### 1) Credential field mismatch
- API expects `Email` + `Password`
- UI login currently uses `username` + `password`

### 2) Role name mismatch
- API currently seeds Identity roles `"Admin"` and `"User"` (dev seeding in `Program.cs`)
- UI expects `Machinist`, `ShippingReceiving`, `Supervisor`, `Admin`
- Domain enum `RoleType` contains: `Operator, Admin, Shipping, Receiving, Supervisor, Quality, Setup`

**Recommendation:** pick a single canonical set of role names for Identity (and therefore JWT claims) that match the UI + authorization rules. Then seed those roles in dev, and assign them via the existing admin provisioning endpoints.

### 3) “Logged in” verification
- UI currently “trusts itself” via `localStorage`
- API already supports:
  - JWT validation on every protected request
  - `GET /api/auth/ping` for an explicit auth check

---

## Proposed wiring plan (efficient + best practices)

### Phase 1 (minimal): real login → store token → extract roles

#### A) Add a real Auth API client (frontend)
- Create an `AuthApi` wrapper (can use existing `ApiClient`) that calls:
  - `POST /api/auth/login` with `{ email, password }`
  - receives `{ accessToken }`

#### B) Add a real AuthState/AuthService (frontend)
Single source of truth for:
- `accessToken` (string | null)
- decoded claims: `identityUserId` (from `sub`), `roles` (from role claims)
- helpers:
  - `isLoggedIn()` = token present and not expired
  - `logout()` clears token + derived state

**Token storage recommendation (pragmatic):**
- For MVP: store `accessToken` in `localStorage` (easy, survives refresh)
- Prefer next step: store in memory and use refresh token via **HttpOnly** cookie (reduces XSS exposure)

#### C) HTTP interceptor to attach the JWT
- Add an Angular HTTP interceptor that adds:
  - `Authorization: Bearer <token>`
  - to requests to `environment.apiBaseUrl`
- On `401`:
  - clear auth state
  - navigate to `/login`

#### D) Use the JWT as the “role transport”
Backend already places role claims in the JWT. The frontend can decode them.

Implementation detail to verify during wiring:
- Role claims are added using `ClaimTypes.Role`. In JWTs these commonly show up as `"role"` (often as an array if multiple roles).
- Confirm actual claim name by decoding a token from `/api/auth/login` once wired.

#### E) Bridge into existing `RoleService` and navigation
Two workable options:
- **Option 1 (recommended):** evolve `RoleService` to be **server-driven**:
  - expose `userRoles: Role[]` (from JWT)
  - expose `activeRole` (either the single role, or a user-selected role when multiple)
  - persist `activeRole` only if multiple roles are present
- **Option 2:** keep `RoleService` as-is (UI demo) and add a separate `AuthzService` later; this is simpler short-term but duplicates “role” concepts.

#### F) Login UX flow
1. User enters email/password on `/login`
2. UI calls `POST /api/auth/login`
3. On success:
   - store token
   - decode roles + `sub`
   - navigate to `/dashboard`
4. On failure (401):
   - show “Invalid credentials” (separate from required-field validation)

### Phase 2 (future): tighter authorization + role restrictions on endpoints

#### A) Expand the role set consistently
Decide and implement one canonical role naming scheme across:
- Identity roles (source of truth)
- JWT role claims
- frontend `Role` union and nav grouping
- controller restrictions

Examples (aligning with UI):
- `Admin`, `Supervisor`, `Machinist`, `ShippingReceiving`

Or (aligning with domain enum):
- `Admin`, `Supervisor`, `Operator`, `Shipping`, `Receiving`, `Quality`, `Setup`

#### B) Use policies for complex rules
When “roles aren’t enough” (multi-role or attribute-based rules), define policies in `Program.cs`:
- `options.AddPolicy("CanManageUsers", p => p.RequireRole("Admin"));`
- Later: custom requirements (e.g., user can only access their own resources)

#### C) Apply restrictions in controllers incrementally
You already have a pattern of:
- `[Authorize]` at controller level
- `[Authorize(Roles="Admin")]` at action level

Future work can extend this to non-admin roles per endpoint while keeping the surface area explicit and readable.

---

## “What to implement first” checklist (when you’re ready)

1. Frontend: replace mock login submit with `POST /api/auth/login` (email/password).
2. Frontend: store access token + decode JWT to extract roles + sub.
3. Frontend: add HTTP interceptor to attach `Authorization` header.
4. Backend: ensure Identity role names match the UI roles you want to enforce.
5. Backend: seed those roles in dev and ensure user provisioning assigns them.
6. Frontend: drive nav visibility from decoded roles (optional at first; can keep current UI).



