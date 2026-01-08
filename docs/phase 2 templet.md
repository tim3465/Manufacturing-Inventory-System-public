Initiate Phase 2A for the {Entity} slice.

This phase is filesystem-only.
Create empty files as structural placeholders only.
Do NOT define contracts, signatures, or DTO properties.

This section defines the **explicit behavioral contract** for this slice.
All later phases must conform to this intent.

---

### Commands

#### Create
- HTTP: POST /api/{entityPlural}
- Exists: Yes
- Returns: {Entity}Dto
- Notes:
  - Creates a new {Entity} record
  - Required fields include:
    - PartId
    - CustomerId
    - PartAmountRequested
    - PartsPerBar
  - No workflow orchestration occurs here
  - No cross-table operations (no automatic job creation, no inventory changes)

---

#### Update
- Exists: Yes
- HTTP Verb: PATCH
- Route: /api/{entityPlural}/{id}
- Scope:
  - Metadata-only (planning fields only)
  - Allowed fields:
    - PartId
    - CustomerId
    - PartAmountRequested
    - PartsPerBar
  - Explicitly NOT allowed:
    - Creating or modifying related entities (no Jobs, no Shifts)
    - Triggering workflows
    - Inventory or execution semantics
- Returns:
  - {Entity}Dto
- Notes:
  - General update endpoint
  - Does not imply scheduling or execution

---

#### Inactivate
- Exists: Yes
- HTTP: PATCH
- Route: /api/{entityPlural}/{id}/inactivate
- Returns: bool
- Notes:
  - Soft-delete semantics (sets inactivation fields)
  - No cascade behavior
  - Record remains queryable via admin routes

---

### Queries

#### Get
- Exists: Yes
- HTTP: GET /api/{entityPlural}/{id}
- Returns: {Entity}Dto | null
- Notes:
  - Intended for admin, debugging, or reference scenarios

---

#### List
- Exists: Yes
- HTTP: GET /api/{entityPlural}
- Returns: List<{Entity}Dto>
- Notes:
  - Returns active records only
  - Default ordering by creation time

---

#### ListAll
- Exists: Yes (Admin only)
- HTTP: GET /api/{entityPlural}/all
- Returns: List<{Entity}Dto>
- Notes:
  - Includes inactive records
  - Intended for auditing and diagnostics

---

### Explicitly NOT Supported
- Hard delete
- Workflow orchestration (no automatic Job creation)
- Inventory computation or validation
- Combined / multi-aggregate endpoints
- Upsert / find-or-create behavior
- Any endpoint that mutates related aggregates

---

### Contract Notes
- This slice represents a **planning / request table**
- Records are mutable within defined bounds
- No execution or inventory meaning is implied
- If an operation is not listed here, it must NOT appear in:
  - Commands folders
  - Queries folders
  - Services
  - Controllers
  - DTOs
