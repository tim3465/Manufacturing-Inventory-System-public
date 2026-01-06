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

### How to Use

1. Start the API using the `CncApp.Api (https)` profile.
2. Import the collection file into Postman.
3. Update collection variables if needed:
   - `baseUrl` - API base URL (default: `https://localhost:7136`)
   - `materialId` - Valid Material ID for creating/updating stock lots (default: `1`)
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

These collections are intended for local development and demonstration.

