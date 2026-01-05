# CNC App – File Structure Cleanup Plan (Corrected)

This document defines the **corrected and canonical cleanup steps** for aligning test, service, and repository file structures. All corrections below address **pluralization, naming consistency, folder placement, and namespace accuracy**. No behavioral changes are intended.

---

## Global C# File Header Rules (Prevents Namespace/Using Glitches)

These rules apply to **all new and modified C# files created by this cleanup**:

1. **All `using` statements must be at the very top of the file** (before the namespace).
2. Each file must contain **exactly one** `namespace` declaration.
3. Do **not** place `using` statements on the same line as `namespace`.
4. Prefer the following header layout:

```csharp
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs.Commands;

public partial class JobTests
{
    // ...
}
```

**Verification step:** For every file touched by this cleanup, confirm the header order is:
- `using ...`
- blank line
- `namespace ...;`
- blank line
- class declaration

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
      │  └─ JobTests.PlaceholderCommand.cs
      ├─ Queries
      │  └─ JobTests.PlaceholderQuery.cs
      └─ JobTests.cs
```

#### Commands Placeholder Example

```csharp
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs.Commands;

// TODO: Replace with actual command tests when implementing Job commands.
// Each file should contain tests for a single command.
// Future file naming convention: JobTests.{CommandType}.cs
// Examples: Create, Inactivate, Delete.
public partial class JobTests
{
    // TODO: Add command tests
}
```

#### Queries Placeholder Example

```csharp
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs.Queries;

// TODO: Replace with actual query tests when implementing Job queries.
// Each file should contain tests for a single query.
// Future file naming convention: JobTests.{QueryType}.cs
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

File: `JobService.PlaceholderCommand.cs`

```csharp
namespace CncApp.Application.Services.Jobs.Commands;

// TODO: Replace with actual command methods when implementing Job commands.
// Each placeholder contains no real logic.
public partial class JobService
{
    // TODO: Add command method
}
```

#### Queries Placeholder

File: `JobService.PlaceholderQuery.cs`

```csharp
namespace CncApp.Application.Services.Jobs.Queries;

// TODO: Replace with actual query methods when implementing Job queries.
// Each placeholder contains no real logic.
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

File: `JobRepository.PlaceholderCommand.cs`

```csharp
namespace CncApp.Infrastructure.Repositories.Jobs.Commands;

// TODO: Replace with actual command methods when implementing Job repository commands.
// Each placeholder contains no real logic.
public partial class JobRepository
{
    // TODO: Add command method
}
```

#### Queries Placeholder

File: `JobRepository.PlaceholderQuery.cs`

```csharp
namespace CncApp.Infrastructure.Repositories.Jobs.Queries;

// TODO: Replace with actual query methods when implementing Job repository queries.
// Each placeholder contains no real logic.
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

