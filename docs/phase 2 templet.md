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
    - OrderId
    - MachineId
    - StockLotId
    - PartAmountPlanned
    - BarAmountPlanned
    - BarCycleTime
    - BarsInJob
    - EstimatedPartsPerBar
  - Represents a planned execution unit
  - No shift creation occurs here
  - No inventory adjustment occurs here

---

#### Update
- Exists: Yes
- HTTP Verb: PATCH
- Route: /api/{entityPlural}/{id}
- Scope:
  - Metadata-only (planning fields only)
  - Allowed fields:
    - MachineId
    - StockLotId
    - PartAmountPlanned
    - BarAmountPlanned
    - BarCycleTime
    - BarsInJob
    - EstimatedPartsPerBar
  - Explicitly NOT allowed:
    - Creating or modifying Shifts
    - Adjusting inventory
    - Triggering workflow chains
- Returns:
  - {Entity}Dto
- Notes:
  - Planning adjustments only
  - Does not imply execution or completion

---

#### Inactivate
- Exists: Yes
- HTTP: PATCH
- Route: /api/{entityPlural}/{id}/inactivate
- Returns: bool
- Notes:
  - Soft-delete semantics (sets inactivation fields)
  - Does not cascade to Shifts or Orders
  - Preserves historical planning intent

---

### Queries

#### Get
- Exists: Yes
- HTTP: GET /api/{entityPlural}/{id}
- Returns: {Entity}Dto | null
- Notes:
  - Intended for admin, debugging, or planning review

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
- Shift creation or modification
- Inventory computation or adjustment
- Combined planning + execution endpoints
- Automatic workflow chaining
- Upsert / find-or-create behavior
- Any endpoint that mutates related aggregates

---

### Contract Notes
- This slice represents a **planned execution table**
- Records define intent, not outcome
- Execution and results belong to Shifts
- Inventory movement belongs to StockLotAdjustments
- If an operation is not listed here, it must NOT appear in:
  - Commands folders
  - Queries folders
  - Services
  - Controllers
  - DTOs
