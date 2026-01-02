## Postman API Testing

This repository includes a Postman collection for testing all API endpoints in the CNC App.

### Collection File
- **CncApp.AllEndpoints.postman_collection.json** - Complete collection with all endpoints organized by controller

### Included Endpoints
- **Auth**: Login and Ping endpoints
- **Machines**: Create, List, Get by ID, Inactivate, and List All
- **Users**: Create user (admin-only)
- **WeatherForecast**: Get weather forecast (template endpoint)

### How to use
1. Start the API using the `CncApp.Api (https)` profile.
2. Import the collection: `CncApp.AllEndpoints.postman_collection.json`
3. Update `baseUrl` in the collection variables if needed (default: `https://localhost:7136`).
4. Run **POST Login** first (in Auth folder) to authenticate and store the access token.
5. Run the remaining requests in order. Requests are organized by controller in folders.

### Collection Variables
- `baseUrl` - API base URL (default: https://localhost:7136)
- `accessToken` - JWT token (automatically set by Login request)
- `machineId` - Machine ID (automatically set by Create Machine request)
- `userId` - Domain User ID (automatically set by Create User request)

### Authentication
- Most endpoints require authentication using `Bearer {{accessToken}}`
- Admin-only endpoints require the "Admin" role
- Public endpoints (like List Active Machines) don't require authentication

This collection is intended for local development and demonstration.

