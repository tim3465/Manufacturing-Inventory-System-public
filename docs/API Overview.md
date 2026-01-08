# API Overview

Summary of the HTTP endpoints exposed by `CncApp.Api`. Routes are rooted at `api/<controller>` unless otherwise noted.

## Auth
- `POST /api/auth/login` — no auth required; returns JWT on valid credentials.
- `GET /api/auth/ping` — requires any authenticated user; health check for JWT.

## Jobs
- `POST /api/jobs` — Admin only; create job.
- `PATCH /api/jobs/{id}` — Admin only; update job.
- `PATCH /api/jobs/{id}/inactivate` — Admin only; soft delete job.
- `GET /api/jobs` — Allow anonymous; list active jobs.
- `GET /api/jobs/all` — Admin only; list all jobs.
- `GET /api/jobs/{id}` — Allow anonymous; get job by id.

## Machines
- `POST /api/machines` — Admin only; create machine.
- `GET /api/machines` — Allow anonymous; list active machines.
- `GET /api/machines/all` — Admin only; list all machines.
- `GET /api/machines/{id}` — Allow anonymous; get machine by id.
- `PATCH /api/machines/{id}/inactivate` — Admin only; soft delete machine.

## Materials
- `POST /api/materials` — Admin only; create material.
- `PATCH /api/materials/{id}` — Admin only; update material metadata.
- `GET /api/materials` — Allow anonymous; list active materials.
- `GET /api/materials/all` — Admin only; list all materials.
- `GET /api/materials/{id}` — Allow anonymous; get material by id.
- `PATCH /api/materials/{id}/inactivate` — Admin only; soft delete material.

## Orders
- `POST /api/orders` — Admin only; create order.
- `PATCH /api/orders/{id}` — Admin only; update order metadata.
- `PATCH /api/orders/{id}/inactivate` — Admin only; soft delete order.
- `GET /api/orders` — Allow anonymous; list active orders.
- `GET /api/orders/all` — Admin only; list all orders.
- `GET /api/orders/{id}` — Allow anonymous; get order by id.

## Parts
- `POST /api/parts` — Admin only; create part.
- `PATCH /api/parts/{id}` — Admin only; update part metadata.
- `PATCH /api/parts/{id}/inactivate` — Admin only; soft delete part.
- `GET /api/parts` — Allow anonymous; list active parts.
- `GET /api/parts/all` — Admin only; list all parts.
- `GET /api/parts/{id}` — Allow anonymous; get part by id.

## Shifts
- `POST /api/shifts` — Admin only; create shift.
- `PATCH /api/shifts/{id}/inactivate` — Admin only; soft delete shift.
- `GET /api/shifts/{id}` — Allow anonymous; get shift by id.
- `GET /api/shifts` — Allow anonymous; list active shifts.
- `GET /api/shifts/all` — Admin only; list all shifts.

## Stock Lots
- `POST /api/stocklots` — Admin only; create stock lot.
- `PATCH /api/stocklots/{id}` — Admin only; update stock lot metadata.
- `GET /api/stocklots` — Allow anonymous; list active stock lots.
- `GET /api/stocklots/{id}` — Allow anonymous; get stock lot by id.
- `PATCH /api/stocklots/{id}/inactivate` — Admin only; soft delete stock lot.

## Stock Lot Adjustments
- `POST /api/stocklotadjustments` — Admin only; create stock lot adjustment.
- `GET /api/stocklotadjustments/{id}` — Allow anonymous; get adjustment by id.
- `GET /api/stocklotadjustments/by-stocklot/{stockLotId}` — Allow anonymous; list adjustments for a stock lot.
- `GET /api/stocklotadjustments/all` — Admin only; list all adjustments.
- `PATCH /api/stocklotadjustments/{id}/notes` — Admin only; update notes.
- `PATCH /api/stocklotadjustments/{id}/inactivate` — Admin only; soft delete adjustment.

## Users
Class-level `Authorize` applies; everything requires authentication unless noted.
- `POST /api/users` — Admin only; create user.
- `PATCH /api/users/{id}` — Admin only; update user roles.
- `PATCH /api/users/{id}/inactivate` — Admin only; soft delete user.
- `GET /api/users` — Auth required; list active users.
- `GET /api/users/all` — Admin only; list all users.
- `GET /api/users/{id}` — Auth required; get user by id.

