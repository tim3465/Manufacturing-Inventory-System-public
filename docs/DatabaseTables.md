# Database Tables

This document lists all database tables, their primary keys, foreign keys, and classification as ledger, snapshot, or lookup tables.

## Application Domain Tables

### Machines
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: None
- **Type**: **Snapshot**
- **Description**: Current state of manufacturing machines. Tracks serial number, model number, and soft-delete status via `InactivatedDateTime`.

### Materials
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: None
- **Type**: **Lookup**
- **Description**: Reference data for material types. Contains heat number and material name. Referenced by StockLots.

### Parts
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: None
- **Type**: **Lookup**
- **Description**: Reference data for part specifications. Contains approximate cycle time and check frequency. Referenced by Orders.

### Users
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: None
- **Unique Constraints**: `IdentityUserId` (unique index)
- **Type**: **Snapshot**
- **Description**: Domain users (distinct from Identity users). Links to ASP.NET Identity via `IdentityUserId`. Contains user profile information.

### StockLots
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: 
  - `MaterialId` → `Materials.Id` (Cascade)
- **Check Constraints**: `CK_StockLots_AmountOfBars_NonNegative` (`AmountOfBars >= 0`)
- **Type**: **Snapshot**
- **Description**: Current inventory state of stock lots. Tracks lot number, material, quantity (AmountOfBars), dimensions (Diameter, BarLength), condition, and check-in date. Quantity is updated via StockLotAdjustments ledger.

### Orders
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: 
  - `PartId` → `Parts.Id` (Cascade)
- **Type**: **Snapshot**
- **Description**: Current state of customer orders. Contains part requested, customer ID, quantity requested, and parts per bar calculation. Referenced by Jobs.

### Jobs
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: 
  - `OrderId` → `Orders.Id` (Cascade)
  - `StockLotId` → `StockLots.Id` (Cascade)
  - `MachineId` → `Machines.Id` (Cascade)
- **Type**: **Snapshot**
- **Description**: Current state of production jobs. Links order, stock lot, and machine. Contains planned quantities (PartAmountPlanned, BarAmountPlanned), actual progress (BarsInJob), cycle time, and estimated parts per bar.

### Shifts
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: 
  - `JobId` → `Jobs.Id` (Cascade)
  - `OperatorId` → `Users.Id` (Restrict)
- **Type**: **Ledger**
- **Description**: Historical record of work shifts. Immutable records of production activity. Contains parts made, scrap, bars consumed, start/stop times, downtime, and operator information.

### StockLotAdjustments
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: 
  - `StockLotId` → `StockLots.Id` (Cascade)
  - `JobId` → `Jobs.Id` (nullable, no FK constraint)
- **Type**: **Ledger**
- **Description**: Historical record of inventory changes. Immutable records tracking delta changes to stock lot quantities. Contains reason code, delta amount, optional job reference, and notes.

## ASP.NET Identity Tables

### AspNetUsers
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: None
- **Unique Constraints**: `NormalizedUserName` (unique index)
- **Type**: **Snapshot**
- **Description**: ASP.NET Identity user accounts. Used for authentication and authorization. Linked to domain Users via `IdentityUserId`.

### AspNetRoles
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: None
- **Unique Constraints**: `NormalizedName` (unique index)
- **Type**: **Lookup**
- **Description**: ASP.NET Identity roles (e.g., "Admin"). Reference data for authorization.

### AspNetUserRoles
- **Primary Key**: Composite (`UserId`, `RoleId`)
- **Foreign Keys**: 
  - `UserId` → `AspNetUsers.Id` (Cascade)
  - `RoleId` → `AspNetRoles.Id` (Cascade)
- **Type**: **Snapshot**
- **Description**: Many-to-many relationship between users and roles. Current assignment state.

### AspNetUserClaims
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: 
  - `UserId` → `AspNetUsers.Id` (Cascade)
- **Type**: **Snapshot**
- **Description**: Claims associated with users. Current claim assignments.

### AspNetRoleClaims
- **Primary Key**: `Id` (int, identity)
- **Foreign Keys**: 
  - `RoleId` → `AspNetRoles.Id` (Cascade)
- **Type**: **Snapshot**
- **Description**: Claims associated with roles. Current claim assignments.

### AspNetUserLogins
- **Primary Key**: Composite (`LoginProvider`, `ProviderKey`)
- **Foreign Keys**: 
  - `UserId` → `AspNetUsers.Id` (Cascade)
- **Type**: **Snapshot**
- **Description**: External login providers (e.g., Google, Facebook) linked to users. Current login configuration.

### AspNetUserTokens
- **Primary Key**: Composite (`UserId`, `LoginProvider`, `Name`)
- **Foreign Keys**: 
  - `UserId` → `AspNetUsers.Id` (Cascade)
- **Type**: **Snapshot**
- **Description**: Authentication tokens for users. Current token state.

## Common Audit Fields

All application domain tables (except Identity tables) include the following audit fields:
- `CreatedDateTime` (DateTimeOffset, required)
- `CreatedByUserId` (int, nullable)
- `UpdatedDateTime` (DateTimeOffset, nullable)
- `UpdatedByUserId` (int, nullable)
- `InactivatedDateTime` (DateTimeOffset, nullable) - Used for soft-delete
- `InactivatedByUserId` (int, nullable)

## Table Type Classifications

### Snapshot Tables
Tables that represent current state and can be updated:
- **Machines** - Current machine inventory
- **Users** - Current user profiles
- **StockLots** - Current inventory quantities (updated via ledger)
- **Orders** - Current order state
- **Jobs** - Current job state
- **AspNetUsers** - Current authentication accounts
- **AspNetUserRoles** - Current role assignments
- **AspNetUserClaims** - Current user claims
- **AspNetRoleClaims** - Current role claims
- **AspNetUserLogins** - Current login configurations
- **AspNetUserTokens** - Current token state

### Ledger Tables
Immutable historical records that track changes over time:
- **Shifts** - Historical production shift records
- **StockLotAdjustments** - Historical inventory change records

### Lookup Tables
Reference data that changes infrequently:
- **Materials** - Material type catalog
- **Parts** - Part specification catalog
- **AspNetRoles** - Role definitions

## Notes

1. **StockLots** is classified as a snapshot table, but its quantity (`AmountOfBars`) is maintained via the **StockLotAdjustments** ledger. The current quantity should be calculated by summing adjustments, but appears to be stored denormalized for performance.

2. **Jobs** contains both planned values (`PartAmountPlanned`, `BarAmountPlanned`) and actual progress values (`BarsInJob`, `EstimatedPartsPerBar`), making it a hybrid snapshot that tracks both plan and current state.

3. **UserRoles** table was created in the initial migration but appears to have been removed in a later migration (`20260103120943_RemoveDomainUserRolesTable`). Role assignments are now handled via ASP.NET Identity's `AspNetUserRoles` table.

4. **StockLotAdjustments.JobId** is nullable and has no foreign key constraint, allowing adjustments to be recorded without a job reference (e.g., manual adjustments, corrections).

5. **Shifts.OperatorId** uses `Restrict` delete behavior, preventing deletion of users who have shift records, preserving historical data integrity.

6. **Orders.CustomerId** is an integer but has no foreign key constraint, suggesting customers may be managed externally or the relationship is not enforced at the database level.

