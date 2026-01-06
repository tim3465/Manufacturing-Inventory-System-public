SCOPE
Only modify namespace declarations in existing C# files under these folders:
- CncApp.Application.Tests/Services
- CncApp.Application/Services
- CncApp.Infrastructure/Repositories

APPLIES TO SLICES
Jobs
Materials
Orders
Parts
Shifts
StockLotAdjustments
StockLots

GOAL
Fix partial-class namespace mismatches.
All partial class files for a given type must share the same namespace as the slice root.

WHAT TO CHANGE
Remove `.Commands` or `.Queries` from the namespace declaration ONLY.

Examples:
- namespace CncApp.Application.Tests.Services.Jobs.Commands;
  → namespace CncApp.Application.Tests.Services.Jobs;

- namespace CncApp.Application.Services.Jobs.Queries;
  → namespace CncApp.Application.Services.Jobs;

- namespace CncApp.Infrastructure.Repositories.Jobs.Queries;
  → namespace CncApp.Infrastructure.Repositories;

RULES (HARD)
1) Do NOT move files.
2) Do NOT rename files or folders.
3) Do NOT change class names, methods, usings, or logic.
4) Change ONLY the single `namespace ...;` line when it contains `.Commands` or `.Queries`.
5) If a file already has the correct namespace, leave it untouched.
6) Do NOT infer new structure or refactor.

VALIDATION
After changes, partial classes (Tests, Services, Repositories) must compile and stitch correctly.

DELIVERABLE
A single commit containing namespace-only fixes.
