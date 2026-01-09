# API Recon

## 1. Endpoint Inventory (by Controller)

### AuthController
| HTTP Method | Route | Auth Requirement | Request DTO | Response Type / Status |
|-------------|-------|------------------|-------------|------------------------|
| POST | `/api/auth/login` | AllowAnonymous | `LoginRequestDto` | `LoginResponseDto` / 200, 401 |
| GET | `/api/auth/ping` | Authenticated | none | anonymous object / 200 |

### JobsController
| HTTP Method | Route | Auth Requirement | Request DTO | Response Type / Status |
|-------------|-------|------------------|-------------|------------------------|
| GET | `/api/jobs` | AllowAnonymous | none | `List<JobDto>` / 200 |
| GET | `/api/jobs/{id}` | AllowAnonymous | none | `JobDto` / 200, 404 |
| GET | `/api/jobs/all` | Admin | none | `List<JobDto>` / 200 |
| POST | `/api/jobs` | Admin | `CreateJobRequestDto` | `JobDto` / 201 |
| PATCH | `/api/jobs/{id}` | Admin | `UpdateJobRequestDto` | `JobDto` / 200, 404 |
| PATCH | `/api/jobs/{id}/inactivate` | Admin | none | NoContent / 204, 404 |

### MachinesController
| HTTP Method | Route | Auth Requirement | Request DTO | Response Type / Status |
|-------------|-------|------------------|-------------|------------------------|
| GET | `/api/machines` | AllowAnonymous | none | `List<MachineDto>` / 200 |
| GET | `/api/machines/{id}` | AllowAnonymous | none | `MachineDto` / 200, 404 |
| GET | `/api/machines/all` | Admin | none | `List<MachineDto>` / 200 |
| POST | `/api/machines` | Admin | `CreateMachineRequestDto` | object(Id) / 201 |
| PATCH | `/api/machines/{id}/inactivate` | Admin | none | NoContent / 204, 404 |

### MaterialsController
| HTTP Method | Route | Auth Requirement | Request DTO | Response Type / Status |
|-------------|-------|------------------|-------------|------------------------|
| GET | `/api/materials` | AllowAnonymous | none | `List<MaterialDto>` / 200 |
| GET | `/api/materials/{id}` | AllowAnonymous | none | `MaterialDto` / 200, 404 |
| GET | `/api/materials/all` | Admin | none | `List<MaterialDto>` / 200 |
| POST | `/api/materials` | Admin | `CreateMaterialRequestDto` | object(Id) / 201 |
| PATCH | `/api/materials/{id}` | Admin | `UpdateMaterialRequestDto` | `MaterialDto` / 200, 404 |
| PATCH | `/api/materials/{id}/inactivate` | Admin | none | NoContent / 204, 404 |

### OrdersController
| HTTP Method | Route | Auth Requirement | Request DTO | Response Type / Status |
|-------------|-------|------------------|-------------|------------------------|
| GET | `/api/orders` | AllowAnonymous | none | `List<OrderDto>` / 200 |
| GET | `/api/orders/{id}` | AllowAnonymous | none | `OrderDto` / 200, 404 |
| GET | `/api/orders/all` | Admin | none | `List<OrderDto>` / 200 |
| POST | `/api/orders` | Admin | `CreateOrderRequestDto` | object(Id) / 201 |
| PATCH | `/api/orders/{id}` | Admin | `UpdateOrderRequestDto` | `OrderDto` / 200, 404 |
| PATCH | `/api/orders/{id}/inactivate` | Admin | none | NoContent / 204, 404 |

### PartsController
| HTTP Method | Route | Auth Requirement | Request DTO | Response Type / Status |
|-------------|-------|------------------|-------------|------------------------|
| GET | `/api/parts` | AllowAnonymous | none | `List<PartDto>` / 200 |
| GET | `/api/parts/{id}` | AllowAnonymous | none | `PartDto` / 200, 404 |
| GET | `/api/parts/all` | Admin | none | `List<PartDto>` / 200 |
| POST | `/api/parts` | Admin | `CreatePartRequestDto` | `PartDto` / 201 |
| PATCH | `/api/parts/{id}` | Admin | `UpdatePartRequestDto` | `PartDto` / 200, 404 |
| PATCH | `/api/parts/{id}/inactivate` | Admin | none | NoContent / 204, 404 |

### ShiftsController
| HTTP Method | Route | Auth Requirement | Request DTO | Response Type / Status |
|-------------|-------|------------------|-------------|------------------------|
| GET | `/api/shifts` | AllowAnonymous | none | `List<ShiftDto>` / 200 |
| GET | `/api/shifts/{id}` | AllowAnonymous | none | `ShiftDto` / 200, 404 |
| GET | `/api/shifts/all` | Admin | none | `List<ShiftDto>` / 200 |
| POST | `/api/shifts` | Admin | `CreateShiftRequestDto` | object(Id) / 201 |
| PATCH | `/api/shifts/{id}/inactivate` | Admin | none | NoContent / 204, 404 |

### StockLotsController
| HTTP Method | Route | Auth Requirement | Request DTO | Response Type / Status |
|-------------|-------|------------------|-------------|------------------------|
| GET | `/api/stocklots` | AllowAnonymous | none | `List<StockLotDto>` / 200 |
| GET | `/api/stocklots/{id}` | AllowAnonymous | none | `StockLotDto` / 200, 404 |
| POST | `/api/stocklots` | Admin | `CreateStockLotRequestDto` | object(Id) / 201 |
| PATCH | `/api/stocklots/{id}` | Admin | `UpdateStockLotRequestDto` | NoContent / 204, 404 |
| PATCH | `/api/stocklots/{id}/inactivate` | Admin | none | NoContent / 204, 404 |

### StockLotAdjustmentsController
| HTTP Method | Route | Auth Requirement | Request DTO | Response Type / Status |
|-------------|-------|------------------|-------------|------------------------|
| GET | `/api/stocklotadjustments/{id}` | AllowAnonymous | none | `StockLotAdjustmentDto` / 200, 404 |
| GET | `/api/stocklotadjustments/by-stocklot/{stockLotId}` | AllowAnonymous | none | `List<StockLotAdjustmentDto>` / 200 |
| GET | `/api/stocklotadjustments/all` | Admin | none | `List<StockLotAdjustmentDto>` / 200 |
| POST | `/api/stocklotadjustments` | Admin | `CreateStockLotAdjustmentRequestDto` | object(Id) / 201 |
| PATCH | `/api/stocklotadjustments/{id}/notes` | Admin | `UpdateStockLotAdjustmentNotesRequestDto` | `StockLotAdjustmentDto` / 200, 404 |
| PATCH | `/api/stocklotadjustments/{id}/inactivate` | Admin | none | NoContent / 204, 404 |

### UsersController
| HTTP Method | Route | Auth Requirement | Request DTO | Response Type / Status |
|-------------|-------|------------------|-------------|------------------------|
| GET | `/api/users` | Authenticated | none | `List<UserDto>` / 200 |
| GET | `/api/users/{id}` | Authenticated | none | `UserDto` / 200, 404 |
| GET | `/api/users/all` | Admin | none | `List<UserDto>` / 200 |
| POST | `/api/users` | Admin | `CreateUserRequestDto` | `CreateUserResponseDto` / 201 |
| PATCH | `/api/users/{id}` | Admin | `UpdateUserRolesRequestDto` | `bool` / 200 |
| PATCH | `/api/users/{id}/inactivate` | Admin | none | `bool` / 200 |

## 2. Dependency & Ordering Analysis
- **Foreign-key requirements**
  - `CreateJobRequestDto` requires `OrderId`, `MachineId`, `StockLotId`, `PartAmountPlanned`, `BarAmountPlanned`, etc. Jobs therefore require existing Orders, Machines, StockLots, Parts.
  - `CreateOrderRequestDto` references `PartId` and customer-related data, so Parts must exist before Orders.
  - `CreateStockLotRequestDto` requires `MaterialId`, `Diameter`, `BarLength`, `Condition`; Materials must exist first.
  - `CreateStockLotAdjustmentRequestDto` needs `StockLotId`, so StockLots must exist.
  - `CreateStockLotRequestDto` indirectly depends on Materials.
  - `CreateJobRequestDto` depends on Machines, Orders, Parts, StockLots.

- **Ordering constraints**
  1. Create Materials (no pre-reqs).
  2. Create Machines.
  3. Create Parts (independent).
  4. Create Orders (requires Parts).
  5. Create StockLots (requires Materials), then StockLotAdjustments (requires StockLots).
  6. Create Jobs (requires Orders, Machines, StockLots, Parts).
  7. Create Shifts (can optionally depend on Jobs/users but not enforced).
  8. Seed Users before any admin-only actions (user creation requires identity provisioning).

- **Circular / optional dependencies**
  - No true circular dependencies; the chain flows from Materials/Machines/Parts → Orders/StockLots → Jobs.
  - Shifts and Users can be created independently; Jobs do not depend on Shifts.

## 3. Postman Variable Plan
- `{{authToken}}` – JWT obtained from `/api/auth/login`.
- `{{machineId}}` – created via `/api/machines`.
- `{{materialId}}` – from `/api/materials`.
- `{{partId}}` – from `/api/parts`.
- `{{orderId}}` – from `/api/orders`.
- `{{stockLotId}}` – from `/api/stocklots`.
- `{{stockLotAdjustmentId}}` – from `/api/stocklotadjustments`.
- `{{jobId}}` – from `/api/jobs`.
- `{{shiftId}}` – from `/api/shifts`.
- `{{userId}}` – from `/api/users` (optionally active user for cleanup).
- Additional DTO vars: `{{materialName}}`, `{{jobReference}}`, etc., as needed for JSON payload templates.

## 4. Proposed Execution Order
1. **Auth** – `POST /api/auth/login` to get `{{authToken}}`.
2. **Seed prerequisites**
   - Create Materials, Machines, Parts.
   - Create Users (Admin) if not already seeded.
3. **Core flows**
   - Create Orders (requires Part).
   - Create StockLots (requires Material) → optionally create StockLotAdjustments.
   - Create Jobs (requires Order, Machine, StockLot, Part).
   - Create Shifts (optional; may reference Jobs or Users).
4. **Reads**
   - Hit `/api/<entity>`, `/api/<entity>/{id}`, `/api/<entity>/all` for each resource to validate responses.
5. **Cleanup (optional)**
   - Inactivate Jobs, Orders, Parts, Machines, Materials, StockLots, StockLotAdjustments, Users via their PATCH `/inactivate` endpoints to avoid persistent data.

