Initiate Phase 2A for the {Entity} slice.

This phase is filesystem-only.
Create empty files as structural placeholders only.
Do NOT define contracts, signatures, or DTO properties.


This section defines the **explicit behavioral contract** for this slice.
All later phases must conform to this intent.

### Commands

#### Create
- HTTP: POST /api/{entityPlural}
- Exists: Yes
- Returns: {Entity}Dto
- Notes: Standard creation.

#### Update
- Exists: Yes
- HTTP Verb: PATCH
- Route: /api/{entityPlural}/{id}
- Scope:
  - Metadata-only
  - Allowed fields: HeatNumber, MaterialName
  - Explicitly NOT allowed: any cross-table operations (no StockLot changes), no workflow orchestration
- Returns:
  - {Entity}Dto
- Notes:
  - This is not a full replace.
  - No quantity semantics exist for this entity.

#### Inactivate
- Exists: Yes
- HTTP: PATCH
- Route: /api/{entityPlural}/{id}/inactivate
- Returns: bool
- Notes: Soft-delete semantics (sets inactivation fields).

---

### Queries

#### Get
- Exists: Yes
- HTTP: GET /api/{entityPlural}/{id}
- Returns: {Entity}Dto | null

#### ListActive
- Exists: Yes
- HTTP: GET /api/{entityPlural}
- Returns: List<{Entity}Dto>

#### ListAll
- Exists: Yes (Admin only)
- HTTP: GET /api/{entityPlural}/all
- Returns: List<{Entity}Dto>
- Notes:
  - Includes inactive records.

---

### Explicitly NOT Supported
- Delete (hard delete)
- Any quantity operations (N/A for this entity)
- Any combined / workflow endpoints (e.g., create material + create stock lot)
- Any upsert / find-or-create behavior (deferred to workflow phase)
- Any endpoint that mutates StockLots through Materials

---

### Contract Notes
- If an operation is not listed here, it must NOT appear in:
  - Commands/Queries folders
  - Services
  - Controllers
  - DTOs
