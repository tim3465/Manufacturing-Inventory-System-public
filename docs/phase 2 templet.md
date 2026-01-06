(This is for Phase 2A)


## Slice Intent — {EntityPlural}

> **Pluralization note (important):**
> `{EntityPlural}` is the plural form of `{Entity}`.
> Use standard English pluralization conventions when deriving `{EntityPlural}` from `{Entity}`.
> If the plural form is ambiguous or irregular, pause and confirm before proceeding.



This section defines the **explicit behavioral contract** for this slice.
All later phases must conform to this intent.

### Commands

#### Create
- HTTP: POST /api/{entityPlural}
- Exists: Yes
- Returns: {Entity}Dto
- Notes: Standard creation

#### Update
- Exists: Yes
- HTTP Verb: PUT | PATCH   ← choose one explicitly
- Route: /api/{entityPlural}/{id}
- Scope:
  - Metadata-only | Full replace | Partial update
- Returns:
  - {Entity}Dto | bool | void   ← choose one explicitly
- Notes:
  - If metadata-only, fields excluded: {list}
  - Quantity-changing operations explicitly excluded/included

#### Inactivate
- Exists: Yes
- HTTP: POST | PATCH
- Route: /api/{entityPlural}/{id}/inactivate
- Returns: bool
- Notes: Soft-delete semantics

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
- Exists: No | Yes (Admin only)

---

### Explicitly NOT Supported
- Delete
- Hard update of quantity
- {Anything else}

---

### Contract Notes
- If an operation is not listed here, it must NOT appear in:
  - Commands/Queries folders
  - Services
  - Controllers
  - DTOs
