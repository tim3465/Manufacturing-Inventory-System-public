## Postman API Testing

This repository includes a Postman collection for testing the Machines API.

### How to use
1. Start the API using the `CncApp.Api (https)` profile.
2. Import the collection:
   `postman/CncApp.Machines.postman_collection.json`
3. Update `baseUrl` in the collection variables if needed.
4. Run **POST Create Machine** first to initialize `machineId`.
5. Run the remaining requests in order.

This collection is intended for local development and demonstration.
