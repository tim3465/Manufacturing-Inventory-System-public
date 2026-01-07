Initiate Phase 2A for the StockLotAdjustments slice.

This phase is filesystem-only.
Create empty files as structural placeholders only.
Do NOT define contracts, signatures, or DTO properties.

This section defines the **explicit behavioral contract** for this slice.
All later phases must conform to this intent.

---

### Commands

#### Create
- HTTP: POST /api/stocklotadjustments
- Exists: Yes
- Returns: StockLotAdjustmentDto
- Notes:
  - Creates a ledger event
  - Required fields include identifiers and delta values
  - No inventory computation or validation occurs here
  - No cross-table operations or workflow orchestration

---

#### Update (Notes Only)
- Exists: Yes
- HTTP Verb: PATCH
- Route: /api/stocklotadjustments/{id}/notes
- Scope:
  - Metadata-only
  - Allowed fields: Notes
  - Explicitly NOT allowed:
    - Changing delta values
    - Changing reason
    - Changing foreign keys
    - Any quantity or inventory semantics
- Returns:
  - StockLotAdjustmentDto
- Notes:
  - Annotation only
  - Does not alter historical truth
  - Not a general update endpoint

---

#### Inactivate
- Exists: Yes
- HTTP: PATCH
- Route: /api/stocklotadjustments/{id}/inactivate
- Returns: bool
- Notes:
  - Soft-delete semantics (sets inactivation fields)
  - Does not recompute or cascade changes

---

### Queries

#### Get
- Exists: Yes
- HTTP: GET /api/stocklotadjustments/{id}
- Returns: StockLotAdjustmentDto | null
- Notes:
  - Intended for admin or debugging scenarios

---

#### ListByParent
- Exists: Yes
- HTTP: GET /api/stocklotadjustments/by-parent/{parentId}
- Returns: List<StockLotAdjustmentDto>
- Notes:
  - Active records only by default
  - Parent may represent a related aggregate (e.g., stock lot or job)
  - Ordering by creation time is expected

---

#### ListAll
- Exists: Yes (Admin only)
- HTTP: GET /api/stocklotadjustments/all
- Returns: List<StockLotAdjustmentDto>
- Notes:
  - Includes inactive records
  - Intended for auditing and diagnostics

---

### Explicitly NOT Supported
- Hard delete
- Editing core ledger values after creation
- Inventory recomputation or validation
- Combined / workflow endpoints
- Upsert / find-or-create behavior
- Any endpoint that mutates related aggregates

---

### Contract Notes
- This slice represents a **ledger table**
- Records are append-only by intent
- Immutability is enforced except for annotations
- If an operation is not listed here, it must NOT appear in:
  - Commands/Queries folders
  - Services
  - Controllers
  - DTOs

