## Postman API Testing

This repository uses a single Postman collection for end-to-end API testing.

### Collection Files

#### CncApp.FullApi.Collection.json
Unified end-to-end collection that runs the API top-to-bottom (Auth → prerequisites → core workflows → reads → optional soft deletes).

**File:** `postman/CncApp.FullApi.Collection.json`

**Included Endpoints:**
- **Auth**: Login captures `jwt`, Ping validates authentication
- **Machines / Materials / Orders / Parts / Jobs / Shifts**: Create, Get by ID, ListActive, ListAll (Admin-only), Update (where supported), Inactivate (via Soft Deletes folder)
- **StockLots**: Create, Get by ID, ListActive, Update, Inactivate (no ListAll endpoint)
- **StockLotAdjustments**: Create, Get by ID, ListByStockLot, ListAll (Admin-only), UpdateNotes, Inactivate (no general ListActive endpoint)
- **Users**: Create, Get by ID, GetRoles (Admin-only), ListActive (authenticated), ListAll (Admin-only), UpdateRoles (Admin-only), Inactivate

Legacy slice-level smoke test collections were removed in favor of a single unified end-to-end Postman collection.

### How to Use

1. Start the API using the `CncApp.Api (https)` profile.
2. Import `postman/CncApp.FullApi.Collection.json` into Postman.
3. Update collection variables if needed:
   - `baseUrl` - API base URL (default: `https://localhost:7136`)
   - `customerId` - Valid Customer ID for creating orders (default: `1`)
4. Run **POST Login** first (in Auth folder) to authenticate and store `jwt`.
5. Run the remaining requests in order, or use Postman's "Run Collection" feature.

### Collection Variables

**Authentication:**
- `adminEmail` - Admin email used by Login (must be set manually)
- `adminPassword` - Admin password used by Login (must be set manually)
- `jwt` - JWT token (automatically set by Login request)

**IDs captured during the run:**
- `machineId`
- `materialId`
- `stockLotId`
- `stockLotAdjustmentId`
- `partId`
- `orderId`
- `jobId`
- `shiftId`
- `userId`
- `operatorId`
- `identityUserId`

**Other useful variables:**
- `baseUrl` - API base URL (default: `https://localhost:7136`)
- `customerId` - Must reference an existing Customer row (no Customers API in this project)
- `runId` / `timestamp` - Used for unique test data (generated automatically)
- `newUserEmail`, `newUserFirstName`, `newUserLastName`, `newUserTempPassword` - Used for `/api/users` creation payload

### Authentication

- Most endpoints require authentication using `Bearer {{jwt}}`.
- Admin-only endpoints require the "Admin" role
- Most List Active / Get by ID endpoints are public, except Users endpoints

These collections are intended for local development and demonstration.

