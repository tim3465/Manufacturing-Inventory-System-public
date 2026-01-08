This section defines the **explicit behavioral contract** for this slice.
All later phases must conform to this intent.

---

### Commands

#### Create
- HTTP: POST /api/parts
- Returns: PartDto
- Notes:
  - Creates a new Part entity
  - Required fields: ApproxPartCycleTime, CheckPerPart

---

#### Update (Metadata Only)
- HTTP: PATCH /api/parts/{id}
- Scope:
  - Metadata-only
  - Allowed fields:
    - ApproxPartCycleTime
    - CheckPerPart
  - Explicitly NOT allowed:
    - Changing Id
    - Changing navigation properties
    - Changing audit fields
- Returns: PartDto | null
- Notes:
  - Partial update semantics
  - Only updates provided fields

---

#### Inactivate
- HTTP: PATCH /api/parts/{id}/inactivate
- Returns: bool
- Notes:
  - Soft-delete semantics (sets inactivation fields)
  - Does not cascade changes

---

### Queries

#### Get
- HTTP: GET /api/parts/{id}
- Returns: PartDto | null
- Notes:
  - Returns single Part by Id
  - Returns null if not found

---

#### List
- HTTP: GET /api/parts
- Returns: List<PartDto>
- Notes:
  - Active records only
  - Default order by CreatedDateTime

---

#### ListAll
- HTTP: GET /api/parts/all
- Returns: List<PartDto>
- Notes:
  - Includes inactive records
  - Intended for admin and auditing

---

### Explicitly NOT Supported
- Hard delete
- Editing audit fields after creation
- Combined / workflow endpoints
- Upsert / find-or-create behavior
- Any endpoint that mutates related aggregates

---

### Contract Notes
- This slice represents a **Part entity**
- Records support soft-delete via inactivation
- If an operation is not listed here, it must NOT appear in:
  - Commands/Queries folders
  - Services
  - Controllers
  - DTOs

