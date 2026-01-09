## Postman API Testing

This repository uses a single Postman collection for end-to-end API testing.

### Collection Files

#### CncApp.FullApi.Collection.json
Unified end-to-end collection that runs the API top-to-bottom (Auth → prerequisites → core workflows → reads → optional soft deletes).

**File:** `postman/CncApp.FullApi.Collection.json`

**Included Endpoints:**
- **Auth**: Login captures `jwt`, Ping validates authentication
- **Machines / Materials / StockLots / StockLotAdjustments / Orders / Parts / Jobs / Shifts / Users**: Create, Get by ID, ListActive, ListAll (Admin-only where supported), Update (where supported), Inactivate (via Soft Deletes folder)

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
- Public endpoints (like List Active) don't require authentication

These collections are intended for local development and demonstration.

