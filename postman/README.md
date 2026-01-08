## Postman API Testing

This repository includes Postman collections for testing all API endpoints in the CNC App.

### Collection Files

#### CncApp.AllEndpoints.postman_collection.json
Complete collection with all endpoints organized by controller.

**Included Endpoints:**
- **Auth**: Login and Ping endpoints
- **Machines**: Create, List, Get by ID, Inactivate, and List All
- **Users**: Create user (admin-only)

#### StockLots.SmokeTests.{timestamp}.postman_collection.json
End-to-end smoke tests for StockLots slice. Versioned with timestamp suffix.

**Latest:** `StockLots.SmokeTests.20260105-1702.postman_collection.json`

**Included Endpoints:**
- **Auth**: Login (Admin)
- **StockLots - Create**: POST Create StockLot
- **StockLots - Get**: GET by ID, GET non-existent (404)
- **StockLots - ListActive**: GET List Active StockLots
- **StockLots - Update**: PUT Update (metadata only), PUT non-existent (404)
- **StockLots - Inactivate**: DELETE Inactivate, DELETE non-existent (404)

#### Materials.SmokeTests.{timestamp}.postman_collection.json
End-to-end smoke tests for Materials slice. Versioned with timestamp suffix.

**Latest:** `Materials.SmokeTests.20260106-1051.postman_collection.json`

**Included Endpoints:**
- **Auth**: Login (Admin)
- **Materials - Create**: POST Create Material
- **Materials - Get**: GET by ID, GET non-existent (404)
- **Materials - ListActive**: GET List Active Materials
- **Materials - ListAll**: GET List All Materials (Admin)
- **Materials - Update**: PATCH Update (metadata only), PATCH non-existent (404)
- **Materials - Inactivate**: PATCH Inactivate, PATCH non-existent (404)

#### StockLotAdjustments.SmokeTests.{timestamp}.postman_collection.json
End-to-end smoke tests for StockLotAdjustments slice. Versioned with timestamp suffix.

**Latest:** `StockLotAdjustments.SmokeTests.20260107-1600.postman_collection.json`

**Included Endpoints:**
- **Auth**: Login (Admin)
- **StockLotAdjustments - Create**: POST Create StockLotAdjustment
- **StockLotAdjustments - Get**: GET by ID, GET non-existent (404)
- **StockLotAdjustments - ListByStockLot**: GET List Adjustments by StockLot
- **StockLotAdjustments - ListAll**: GET List All StockLotAdjustments (Admin)
- **StockLotAdjustments - Update Notes**: PATCH Update Notes (metadata only), PATCH non-existent (404)
- **StockLotAdjustments - Inactivate**: PATCH Inactivate, PATCH non-existent (404)

#### Parts.SmokeTests.{timestamp}.postman_collection.json
End-to-end smoke tests for Parts slice. Versioned with timestamp suffix.

**Latest:** `Parts.SmokeTests.20260108-0659.postman_collection.json`

**Included Endpoints:**
- **Auth**: Login (Admin)
- **Parts - Create**: POST Create Part
- **Parts - Get**: GET by ID, GET non-existent (404)
- **Parts - ListActive**: GET List Active Parts
- **Parts - ListAll**: GET List All Parts (Admin)
- **Parts - Update**: PATCH Update (metadata only, partial updates supported), PATCH non-existent (404)
- **Parts - Inactivate**: PATCH Inactivate, PATCH non-existent (404), verification that inactivated part is excluded from active list but included in all list

#### Orders.SmokeTests.{timestamp}.postman_collection.json
End-to-end smoke tests for Orders slice. Versioned with timestamp suffix.

**Latest:** `Orders.SmokeTests.20260108-0852.postman_collection.json`

**Included Endpoints:**
- **Auth**: Login (Admin)
- **Orders - Create**: POST Create Order
- **Orders - Get**: GET by ID, GET non-existent (404)
- **Orders - ListActive**: GET List Active Orders
- **Orders - ListAll**: GET List All Orders (Admin)
- **Orders - Update**: PATCH Update (metadata only), PATCH non-existent (404)
- **Orders - Inactivate**: PATCH Inactivate, PATCH non-existent (404)

### How to Use

1. Start the API using the `CncApp.Api (https)` profile.
2. Import the collection file into Postman.
3. Update collection variables if needed:
   - `baseUrl` - API base URL (default: `https://localhost:7136`)
   - `materialId` - Valid Material ID for creating/updating stock lots (default: `1`)
   - `stockLotId` - Valid StockLot ID for creating/listing stock lot adjustments (default: `1`)
   - `partId` - Valid Part ID for creating orders (default: `1`)
   - `customerId` - Valid Customer ID for creating orders (default: `1`)
4. Run **POST Login** first (in Auth folder) to authenticate and store the access token.
5. Run the remaining requests in order, or use Postman's "Run Collection" feature.

### Collection Variables

**Common Variables:**
- `baseUrl` - API base URL (default: https://localhost:7136)
- `accessToken` - JWT token (automatically set by Login request)

**StockLots Collection Variables:**
- `stockLotId` - StockLot ID (automatically set by Create StockLot request)
- `materialId` - Material ID for creating/updating stock lots (default: 1, update to valid ID)

**Materials Collection Variables:**
- `materialId` - Material ID (automatically set by Create Material request)

**StockLotAdjustments Collection Variables:**
- `stockLotAdjustmentId` - StockLotAdjustment ID (automatically set by Create StockLotAdjustment request)
- `stockLotId` - StockLot ID for creating/listing adjustments (default: 1, update to valid ID)

**Parts Collection Variables:**
- `partId` - Part ID (automatically set by Create Part request)

**Orders Collection Variables:**
- `orderId` - Order ID (automatically set by Create Order request)
- `partId` - Part ID for creating/updating orders (default: 1, update to valid ID)
- `customerId` - Customer ID for creating/updating orders (default: 1, update to valid ID)

**Machines Collection Variables:**
- `machineId` - Machine ID (automatically set by Create Machine request)
- `userId` - Domain User ID (automatically set by Create User request)

### Authentication

- Most endpoints require authentication using `Bearer {{accessToken}}`
- Admin-only endpoints require the "Admin" role
- Public endpoints (like List Active) don't require authentication

### StockLots Slice Notes

- **Update** is metadata-only (excludes `AmountOfBars` - quantity changes must use StockLotAdjustments)
- **ListAll** is not supported (only ListActive)
- **Hard Delete** is not supported (only soft delete via Inactivate)

### Materials Slice Notes

- **Update** is metadata-only (HeatNumber, MaterialName only)
- **ListAll** is supported (Admin only, includes inactive records)
- **Hard Delete** is not supported (only soft delete via Inactivate)

### StockLotAdjustments Slice Notes

- **Update Notes** is metadata-only (Notes field only - does not alter historical ledger values)
- **ListAll** is supported (Admin only, includes inactive records)
- **ListByStockLot** returns active records only, ordered by creation time
- **Hard Delete** is not supported (only soft delete via Inactivate)
- This slice represents a **ledger table** - records are append-only by intent
- Core ledger values (deltaBars, reason, stockLotId) cannot be changed after creation

### Parts Slice Notes

- **Update** is metadata-only (ApproxPartCycleTime, CheckPerPart only)
- **Update** supports partial updates (can update only ApproxPartCycleTime or only CheckPerPart)
- **ListAll** is supported (Admin only, includes inactive records)
- **ListActive** returns active records only, ordered by CreatedDateTime
- **Hard Delete** is not supported (only soft delete via Inactivate)
- ApproxPartCycleTime is a TimeSpan (formatted as "HH:mm:ss" in JSON, e.g., "00:05:00" for 5 minutes)
- CheckPerPart must be non-negative integer

### Orders Slice Notes

- **Update** is metadata-only (PartId, CustomerId, PartAmountRequested, PartsPerBar only)
- **ListAll** is supported (Admin only, includes inactive records)
- **ListActive** returns active records only, ordered by CreatedDateTime
- **Hard Delete** is not supported (only soft delete via Inactivate)
- This slice represents a **planning / request table** - records are mutable within defined bounds
- No workflow orchestration occurs (no automatic Job creation, no inventory changes)
- PartId must be positive (must reference an existing Part)
- CustomerId must be positive
- PartAmountRequested must be positive
- PartsPerBar must be non-negative (default: 0)

These collections are intended for local development and demonstration.

