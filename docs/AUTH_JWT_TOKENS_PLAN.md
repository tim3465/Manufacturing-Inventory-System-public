# Identity + JWT Tokens — Step-by-Step Plan (Cursor-Driven)

**Purpose:** Add **ASP.NET Core Identity + JWT authentication** to the CNC App in small, reviewable steps **without building a full “User slice” yet**.  
**Output:** a working `/api/auth/login` that returns a JWT access token containing user id + roles, and Machine endpoints protected by role rules.

This plan is designed to be used with **Cursor** by creating one small change per prompt and testing after each step.

References:
- Architecture guardrails: `ARCHITECTURE_RULES.md` fileciteturn1file0
- Target database conventions (Id/audit/soft delete, etc.): `CNC_DATABASE_TARGET_BLUEPRINT.md` fileciteturn1file1

---

## Non‑negotiables (scope + style)

- **API-first only** (no UI pages).
- **Minimal changes per step** (small commits).
- **No refactors** to Machine DTOs/repositories/services unless required for auth compilation.
- Identity/EF Core stays in **Infrastructure**; API wires DI; Domain/Application remain clean. fileciteturn1file0
- JWT is used for stateless API calls (Postman + browser).
- Roles are **Admin** and **User**.

---

## High-level milestone checklist

1. Identity tables exist via EF Core migration.
2. Roles `Admin` and `User` are seeded on startup.
3. A dev-only way exists to create at least one Admin user (seeded user is fine).
4. `/api/auth/login` returns a JWT with:
   - `sub` (UserId)
   - `iat`
   - `exp`
   - role claims
5. API validates JWTs (middleware configured).
6. Machines endpoints are protected:
   - `GET /api/machines` and `GET /api/machines/{id}` → **AllowAnonymous**
   - `POST /api/machines`, `DELETE /api/machines/{id}`, `GET /api/machines/all` → **Admin only**

---

## Step-by-step execution plan (do these in order)

### Step 0 — Pre-flight inventory (no code changes)
**Goal:** Confirm what you already have so you don’t fight “mystery config”.

In Cursor, search for:
- Existing authentication packages / `AddAuthentication` / `JwtBearer`
- Existing DbContext base class
- Existing `Program.cs` middleware order
- Existing migrations folder location

**Deliverable:** A short note (in PR/commit) of what exists vs what will be added.

---

### Step 1 — Add Identity model types (Infrastructure)
**Goal:** Add Identity user/role types cleanly (without building a full User domain feature).

Decisions to lock:
- Identity key type (recommend `int` to match the project conventions) fileciteturn1file1
- Use `IdentityUser<int>` and `IdentityRole<int>` for now (simple).

**Deliverable:** Infrastructure compiles with Identity types available.

**Cursor prompt shape (example):**
- “Edit Infrastructure only. Update AppDbContext to support Identity with int keys. Do not touch Domain/Application. No controllers.”

---

### Step 2 — Wire Identity into DI (API composition root)
**Goal:** Register Identity services using the Infrastructure DbContext.

**Deliverable:** App starts successfully after adding Identity registrations.

**Test:** Run API, ensure no runtime service resolution errors.

---

### Step 3 — Create & apply migration (Infrastructure)
**Goal:** Generate a migration that creates Identity tables (AspNetUsers, AspNetRoles, etc.) plus your existing schema.

**Deliverable:** Migration applied successfully to local DB.

**Test:** Verify the DB contains Identity tables.

> Keep this step isolated so you can rollback easily if something drifts.

---

### Step 4 — Add JWT settings (API config)
**Goal:** Add a `Jwt` section in configuration (Issuer, Audience, Key, Minutes).  
Use a **dev-only** secret locally; plan to move to secrets manager later.

**Deliverable:** Config exists and can be read by the API.

---

### Step 5 — Register JWT authentication + authorization (API)
**Goal:** Configure JWT Bearer authentication and add `UseAuthentication()` before `UseAuthorization()`.

**Deliverable:** App starts; `[Authorize]` attributes are honored.

**Test (quick):**
- Add a temporary test endpoint with `[Authorize]` that returns 200 only with a token (then remove it in the next step).

---

### Step 6 — Seed roles (Admin/User) (Infrastructure or API startup helper)
**Goal:** Ensure roles exist on startup:
- `Admin`
- `User`

**Deliverable:** Running the API creates roles if missing.

**Test:** Inspect DB roles table after startup.

---

### Step 7 — Create a dev Admin user (seed) (same seeding area)
**Goal:** Ensure you can obtain an Admin token for testing immediately.

**Deliverable:** A seeded admin user exists (dev only) with known credentials.

**Safety note:** Add a clear comment that it’s dev-only and will be removed/disabled later.

---

### Step 8 — Add minimal Auth endpoints (API)
**Goal:** Add `POST /api/auth/login` (and optionally `POST /api/auth/register` if you want), purely for token issuance.

**Deliverable:** Login returns:
- `accessToken` (JWT string)

**Test:** Use Postman:
- login → get token
- call an `[Authorize]` endpoint with `Authorization: Bearer <token>`

---

### Step 9 — Ensure JWT contains the right claims
**Goal:** Token includes:
- `sub` = user id
- `iat`
- `exp`
- roles (as role claims)

**Deliverable:** Decoding token shows correct claims.

**Test:** Paste token into jwt.io (local verification) or use Postman test script to decode (optional).

---

### Step 10 — Protect Machines endpoints (API controller attributes)
**Goal:** Apply authorization rules:

- AllowAnonymous:
  - `GET /api/machines`
  - `GET /api/machines/{id}`

- Admin only:
  - `POST /api/machines`
  - `DELETE /api/machines/{id}`
  - `GET /api/machines/all`

**Deliverable:** Correct 401/403 behavior.

**Test matrix:**
- Anonymous:
  - GET active → 200
  - POST → 401/403 (depending on config)
- Authenticated User (non-admin):
  - GET active → 200
  - POST/DELETE/ALL → 403
- Admin:
  - Everything works.

---

### Step 11 — Update Postman collection (optional but recommended)
**Goal:** Add a login request + token variable wiring so the collection runs end-to-end.

- Add request: `POST /api/auth/login`
- In Tests: store `accessToken` as a **collection variable**
- Add `Authorization: Bearer {{accessToken}}` to admin-only requests

**Deliverable:** A “one click” Postman run that:
- logs in
- creates a machine
- gets it
- deletes it
- lists all (as admin)

---

## Suggested commit breakdown (clean history)

1. `chore(identity): support Identity types in Infrastructure DbContext`
2. `chore(identity): wire Identity services in API`
3. `db: add Identity tables migration`
4. `chore(jwt): add jwt config + bearer authentication`
5. `chore(identity): seed roles and dev admin user`
6. `feat(auth): add login endpoint returning jwt access token`
7. `feat(authz): restrict Machines write endpoints to Admin`
8. `test(postman): add auth login + bearer token to collection` (optional)

---

## Cursor workflow rules for this plan

For each step, your Cursor prompt should include:
- The step number (ex: “Step 5 — JWT Bearer middleware”)
- Which project(s) can change (API vs Infrastructure only)
- Explicit **do not touch** list (Domain/Application + Machine slice)
- “Keep changes small; no refactors; compile + run after change”

This keeps Cursor from “boiling the ocean” and makes code review easy. fileciteturn1file0

---
