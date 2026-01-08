# Shifts Slice Intent (Phase 2 Template)

> Derived from `/docs/phase 2 templet.md` (master remains unchanged).
> Purpose: declare which operations exist for Shifts. No behavior/logic implied.

---

### Commands

#### Create
- HTTP: POST /api/shifts
- Exists: Yes
- Returns: `ShiftResultDto`
- Notes:
  - Creates a Shift execution record
  - Requires Job/Operator/StartTime plus production metrics as applicable
  - No inventory movement here; no Job state changes

#### Inactivate
- HTTP: PATCH /api/shifts/{id}/inactivate
- Exists: Yes
- Returns: bool
- Notes:
  - Soft-delete semantics (audit fields only)
  - Does not cascade to related aggregates

---

### Queries

#### Get
- HTTP: GET /api/shifts/{id}
- Exists: Yes
- Returns: `ShiftResultDto` | null
- Notes: For admin/diagnostics

#### ListActive
- HTTP: GET /api/shifts
- Exists: Yes
- Returns: `List<ShiftResultDto>`
- Notes: Active-only list

#### ListAll
- HTTP: GET /api/shifts/all
- Exists: Yes (admin)
- Returns: `List<ShiftResultDto>`
- Notes: Includes inactive records

---

### Explicitly NOT Supported
- Hard delete
- Update/patch of production metrics after creation (defer)
- Inventory adjustments (belongs to StockLotAdjustments)
- Job state changes
- Workflow chaining or combined endpoints
- Upsert / find-or-create

---

### Contract Notes
- Shifts represent execution/production records tied to Jobs and Operators.
- Audit fields enforced via `AuditableEntityBase`; invariants enforced in domain.
- If an operation is not listed here, it must NOT appear in services, repositories, controllers, or DTOs.


