# CNC App – File Structure Cleanup Plan (Corrected)

This document defines the **corrected and canonical cleanup steps** for aligning test, service, and repository file structures. All corrections below address **pluralization, naming consistency, folder placement, and namespace accuracy**. No behavioral changes are intended.

---

## CncApp.Application.Tests / Services

### 1. Machines — Align Tests With Application Services Structure

**Goal:** Ensure the Machines test structure mirrors the Machines application service structure so developers and AI can navigate quickly and predictably.

```
CncApp.Application.Tests
└─ Services
   └─ Machines
      ├─ Commands
      │  ├─ MachineTests.Create.cs
      │  └─ MachineTests.Inactivate.cs
      ├─ Queries
      │  ├─ MachineTests.Get.cs
      │  ├─ MachineTests.ListActive.cs
      │  └─ MachineTests.ListAll.cs
      └─ MachineTests.cs
```

**Notes:**
- All test files are **partial classes** derived from `MachineTests`.
- File and folder structure intentionally mirrors `CncApp.Application.Services.Machines`.
- This structure should have been established earlier, but is now being canonized.

**Restriction:**
- Do **not** change any other files in this section.

---

### 2. Other Entities — Normalize Test Folder Structure

**Applies to:**
- Jobs
- Materials
- Orders
- Parts
- Shifts
- StockLotAdjustments
- StockLots

**Canonical Structure (example shown for Jobs):**

```
CncApp.Application.Tests
└─ Services
   └─ Jobs
      ├─ Commands
      │  └─ JobTests.Placeholder.cs
      ├─ Queries
      │  └─ JobTests.Placeholder.cs
      └─ JobTests.cs
```

#### Commands Placeholder Example

```csharp
namespace CncApp.Application.Tests.Services.Jobs.Commands;

// TODO: Replace with actual command tests when implementing Job commands.
// Each file should contain tests for a single command.
// File naming convention: JobTests.{CommandType}.cs
// Examples: Create, Inactivate, Delete.
public partial class JobTests
{
    // TODO: Add command tests
}
```

#### Queries Placeholder Example

```csharp
namespace CncApp.Application.Tests.Services.Jobs.Queries;

// TODO: Replace with actual query tests when implementing Job queries.
// Each file should contain tests for a single query.
// File naming convention: JobTests.{QueryType}.cs
// Examples: Get, ListActive, ListAll.
public partial class JobTests
{
    // TODO: Add query tests
}
```

#### Root Test File Example

```csharp
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Jobs;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    // Shared setup, mocks, and helpers for Job service tests
}
```

**Restriction:**
- Do **not** change any other files in this section.

---

## CncApp.Application / Services

### 3. Add Placeholder Files for Commands and Queries

**Purpose:**
- Prevent empty folders
- Define canonical extension points for future slice implementation
- Avoid accidental structure drift

**Applies to:**
- Jobs
- Materials
- Orders
- Parts
- Shifts
- StockLotAdjustments
- StockLots

**Canonical Structure (example shown for Jobs):**

#### Commands Placeholder

```csharp
namespace CncApp.Application.Services.Jobs.Commands;

// TODO: Replace with actual command methods when implementing Job commands.
// Each file should contain a single command method.
// File naming convention: JobService.{CommandType}.cs
public partial class JobService
{
    // TODO: Add command method
}
```

#### Queries Placeholder

```csharp
namespace CncApp.Application.Services.Jobs.Queries;

// TODO: Replace with actual query methods when implementing Job queries.
// Each file should contain a single query method.
// File naming convention: JobService.{QueryType}.cs
public partial class JobService
{
    // TODO: Add query method
}
```

**Restriction:**
- Do **not** change any other files in this section.

---

## CncApp.Infrastructure / Repositories

### 4. Add Placeholder Files for Repository Commands and Queries

**Purpose:**
- Mirror Application Services structure
- Establish canonical repository extension points
- Prevent duplicated or inconsistent repository patterns

**Applies to:**
- Jobs
- Materials
- Orders
- Parts
- Shifts
- StockLotAdjustments
- StockLots

**Canonical Structure (example shown for Jobs):**

#### Commands Placeholder

```csharp
namespace CncApp.Infrastructure.Repositories.Jobs.Commands;

// TODO: Replace with actual command methods when implementing Job repository commands.
// Each file should contain a single command method.
// File naming convention: JobRepository.{CommandType}.cs
public partial class JobRepository
{
    // TODO: Add command method
}
```

#### Queries Placeholder

```csharp
namespace CncApp.Infrastructure.Repositories.Jobs.Queries;

// TODO: Replace with actual query methods when implementing Job repository queries.
// Each file should contain a single query method.
// File naming convention: JobRepository.{QueryType}.cs
public partial class JobRepository
{
    // TODO: Add query method
}
```

**Restriction:**
- Do **not** change any other files in this section.

---

## Final Notes

- All entity folders and namespaces are **pluralized** and consistent.
- No behavioral logic is introduced by this cleanup.
- This document defines **structure only**, not implementation.
- Machines remain the canonical reference slice.

This file is intended to be used as a **Cursor-ready cleanup guide** and as a long-term reference to prevent future structural drift.

