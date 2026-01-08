This section defines the **explicit behavioral contract** for this slice.
All later phases must conform to this intent.

---

### Commands

#### Create
- HTTP: POST /api/jobs
- Exists: Yes
- Returns: JobDto
- Notes:
  - Creates a new Job record
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
- Route: /api/jobs/{id}
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
  - JobDto
- Notes:
  - Planning adjustments only
  - Does not imply execution or completion

---

#### Inactivate
- Exists: Yes
- HTTP: PATCH
- Route: /api/jobs/{id}/inactivate
- Returns: bool
- Notes:
  - Soft-delete semantics (sets inactivation fields)
  - Does not cascade to Shifts or Orders
  - Preserves historical planning intent

---

### Queries

#### Get
- Exists: Yes
- HTTP: GET /api/jobs/{id}
- Returns: JobDto | null
- Notes:
  - Intended for admin, debugging, or planning review

---

#### List
- Exists: Yes
- HTTP: GET /api/jobs
- Returns: List<JobDto>
- Notes:
  - Returns active records only
  - Default ordering by creation time

---

#### ListAll
- Exists: Yes (Admin only)
- HTTP: GET /api/jobs/all
- Returns: List<JobDto>
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

