Initiate Phase 2A for the {Entity} slice.

This phase is filesystem-only.
Create empty files as structural placeholders only.
Do NOT define contracts, signatures, or DTO properties.

This section defines the **explicit behavioral intent** for this slice.
All later phases must conform to this intent.

---

### Commands

#### Create
- HTTP: POST /api/{entityPlural}
- Exists: Yes
- Returns: {Entity}Dto
- Notes:
  - Creates a new {Entity} record
  - Provisions Identity + Domain user when applicable
  - Required fields include:
    - UserName
    - FirstName
    - LastName
    - Identity provisioning inputs (as defined by the slice)
  - Admin-only
  - No cross-slice workflow orchestration beyond Identity provisioning
  - No related aggregate mutation

---

#### Update
- Exists: Yes
- HTTP: PATCH
- Route: /api/{entityPlural}/{id}
- Returns: {Entity}Dto 
- Notes:
  - Admin-only
  - Updates are limited strictly to **role assignment changes**
  - No updates to domain profile fields (UserName, FirstName, LastName)
  - No updates to Identity credentials (email, password)
  - Role updates are applied via Identity only
  - No cross-slice workflow orchestration

---

#### Inactivate
- Exists: Yes
- HTTP: PATCH
- Route: /api/{entityPlural}/{id}/inactivate
- Returns: bool
- Notes:
  - Admin-only
  - Soft-delete semantics (sets inactivation fields)
  - Intended to disable an operator for future Shift selection
  - Does not cascade to Shifts
  - Preserves historical records

---

### Queries

#### Get
- Exists: Yes
- HTTP: GET /api/{entityPlural}/{id}
- Returns: {Entity}Dto | null
- Notes:
  - Intended for admin, debugging, or operator reference
  - Domain-only representation (no Identity-sensitive data)
  - Auth should align with Shifts create/edit access (not necessarily Admin-only)

---

#### List
- Exists: Yes
- HTTP: GET /api/{entityPlural}
- Returns: List<{Entity}Dto>
- Notes:
  - Returns active records only
  - Intended for operator selection (e.g., Shifts.OperatorId)
  - Auth should align with Shifts create/edit access (not necessarily Admin-only)
  - Default ordering defined in later phases

---

#### ListAll
- Exists: Yes (Admin only)
- HTTP: GET /api/{entityPlural}/all
- Returns: List<{Entity}Dto>
- Notes:
  - Includes inactive records
  - Intended for auditing and diagnostics
  - Admin-only

---

### Explicitly NOT Supported
- Hard delete
- Editing Identity-sensitive data via this API (passwords, emails)
- Updating domain profile fields (name, username)
- Combined / workflow endpoints beyond Identity provisioning on Create
- Upsert / find-or-create behavior
- Any endpoint that mutates related aggregates

---

### Contract Notes
- This slice represents a **domain operator / user reference table**
- Identity is managed separately; this slice provides domain linkage and operator lookup
- Role management is the **only supported update operation**
- Inactivation controls whether a user is selectable for future Shifts
- If an operation is not listed here, it must NOT appear in:
  - Commands folders
  - Queries folders
  - Services
  - Controllers
  - DTOs
