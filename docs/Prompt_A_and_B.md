

These prompts A and B are meant to seed the folder and file structure for each table listed in Prompt A.

===============================================================================================

Prompt 0

===============================================================================================

OK, I just want to be clear: I’m going to give you a prompt, and that prompt will give you instructions to build folders in this application. You don’t have to give me a response, and if you do, please keep it very short. I know this is set to “Ask” right now; after this prompt, I will switch it to Agent.


===============================================================================================

Prompt A

===============================================================================================

GOAL
Create actual directory folders in the repository (filesystem changes), NOT a markdown document.

You have 3 reference files at repo root:
- MachinesStructure.md (facts)
- MachinesConventions.md (rules)
- DatabaseTables.md (tables)

TASK (IMPORTANT)
Physically CREATE DIRECTORY FOLDERS in the repo for each Application Domain table listed in DatabaseTables.md:

Machines
Materials
Parts
StockLots
Orders
Jobs
Shifts
StockLotAdjustments

Follow MachinesConventions.md folder conventions EXACTLY.

CREATE THESE FOLDERS (directories only):

Application
- Application/Dtos/{EntityPlural}/
- Application/Services/{EntityPlural}/Commands/
- Application/Services/{EntityPlural}/Queries/

Infrastructure
- Infrastructure/Repositories/{EntityPlural}/Commands/
- Infrastructure/Repositories/{EntityPlural}/Queries/

API
- Api/Controllers/

Domain
- Domain/Entities/              (shared, no per-entity subfolder)

Tests
- Application.Tests/Services/{EntityPlural}/Commands/
- Application.Tests/Services/{EntityPlural}/Queries/
- Domain.Tests/Entities/

RULES (STRICT)
- DO NOT create any files.
- DO NOT create any markdown (.md) files.
- DO NOT modify or rename existing folders or files.
- If a folder already exists, leave it unchanged (idempotent).
- Entity folder names must be plural (Machines, StockLots, etc.).
- Do NOT scaffold ASP.NET Identity tables (AspNet*).

OUTPUT (NON-FILE)
After creating folders, PRINT a short Markdown-style checklist in the chat output
showing which folders were created, grouped by table name.
This output must NOT be saved as a file.



===============================================================================================

Prompt B

===============================================================================================

GOAL
Create actual C# FILES in the repository (filesystem changes), NOT markdown documents.

These files are EMPTY STUBS only — structure, namespaces, class declarations, TODOs.
No business logic.

You have 3 reference files at repo root:
- MachinesStructure.md (facts)
- MachinesConventions.md (rules)
- DatabaseTables.md (tables)

TASK (IMPORTANT)
Physically CREATE C# FILES in the repo for each Application Domain table:

Machines
Materials
Parts
Users
StockLots
Orders
Jobs
Shifts
StockLotAdjustments

Follow MachinesConventions.md naming rules and partial-class split rules EXACTLY.
Mirror the Machines slice — do NOT refactor or "improve" existing Machines code.

--------------------------------
CRITICAL RULES (DO NOT VIOLATE)
--------------------------------

ALLOWED FILES ONLY (hard constraint):
- For every entity, you may ONLY create these files:
  1) Application/Dtos/{EntityPlural}/ DTO stubs (per table classification rules below)
  2) Application/Services/{EntityPlural}/{Entity}Service.cs
     - partial class
     - constructor + injected fields ONLY
     - NO methods
  3) Application/Interfaces/Repositories/I{Entity}Repository.cs
     - empty or minimal signatures ONLY
     - DO NOT add full CRUD surface area
  4) Infrastructure/Repositories/{EntityPlural}/{Entity}Repository.cs
     - partial class
     - constructor + _context ONLY
     - NO methods
  5) Api/Controllers/{EntityPlural}Controller.cs
     - constructor ONLY
     - NO actions (no HttpGet/associated methods yet)
     - allowed: [ApiController] and [Route(...)]
     - TODO comments only
  6) Application/Mapping/{Entity}Profile.cs
     - true stub: empty Profile class + TODO comments ONLY
     - NO CreateMap calls yet
  7) Domain/Entities/{Entity}.cs
     - entity stub + TODO invariants ONLY
  8) Domain.Tests/Entities/{Entity}Tests.cs
     - regions + TODOs ONLY
  9) Application.Tests/Services/{EntityPlural}/Commands/PlaceholderCommandTests.cs
  10) Application.Tests/Services/{EntityPlural}/Queries/PlaceholderQueryTests.cs

DO NOT create ANY of these yet:
- Application/Services/{EntityPlural}/Commands/{Entity}Service.<Method>.cs
- Application/Services/{EntityPlural}/Queries/{Entity}Service.<Method>.cs
- Infrastructure/Repositories/{EntityPlural}/Commands/{Entity}Repository.<Method>.cs
- Infrastructure/Repositories/{EntityPlural}/Queries/{Entity}Repository.<Method>.cs
Those method-level files will be created only when implementing a specific slice.

GENERAL RULES:
- CREATE REAL .cs FILES in the repo.
- DO NOT create markdown (.md) files.
- DO NOT add business logic.
- Allowed bodies:
  - `throw new NotImplementedException();`
  - OR empty blocks with `// TODO`
  - Choose ONE style and use it consistently.
- Use correct namespaces that match existing Machines slice.
- Do NOT invent REST patterns or extra methods.
- DTO comment rule:
  - DTOs must NOT claim they mirror Infrastructure.Persistence.Configurations.*
  - Use only: `// TODO: add DataAnnotations to mirror EF configuration when available`
- If something is ambiguous:
  - Add `// TODO: clarify` inside the file
  - Do NOT guess.
- Formatting:
  - one using per line, blank line before namespace, braces on new lines (keep files readable)

--------------------------------
APPLICATION
--------------------------------

Dtos/{EntityPlural}/
- Snapshot & Lookup tables:
  - Create{Entity}RequestDto.cs
  - {Entity}Dto.cs
  - DTOs should include:
    - namespace only
    - class declaration
    - TODO comments (no “mirrored from EF config” claims)

- Ledger tables:
  - Create{Entity}RequestDto.cs
  - {Entity}ResultDto.cs
  - DO NOT create {Entity}Dto.cs for ledger tables
  - Mirror StockLotAdjustmentResultDto naming pattern where applicable

Services/{EntityPlural}/
- {Entity}Service.cs
  - Partial class
  - Constructor + injected fields only
  - NO service methods yet

Mapping/
- {Entity}Profile.cs
  - true stub (empty Profile + TODO only)
  - NO CreateMap calls yet

Interfaces/Repositories/
- I{Entity}Repository.cs
  - empty or minimal signatures ONLY
  - If ambiguous, leave TODO: clarify rather than guessing

--------------------------------
INFRASTRUCTURE
--------------------------------

Repositories/{EntityPlural}/
- {Entity}Repository.cs
  - Partial class
  - Constructor + _context only
  - NO repository methods yet

--------------------------------
API
--------------------------------

Controllers/
- {EntityPlural}Controller.cs
  - Thin stub
  - Constructor only
  - Allowed attributes: [ApiController], [Route("api/<plural-lower>")]
  - NO actions (no HttpGet/Post/etc yet)
  - TODO comments only

--------------------------------
DOMAIN
--------------------------------

Domain/Entities/
- {Entity}.cs
  - Entity stub
  - TODO invariants
  - No EF configuration

--------------------------------
TESTS
--------------------------------

Domain.Tests/Entities/
- {Entity}Tests.cs
  - Regions + TODOs only

Application.Tests/Services/{EntityPlural}/Commands/
- PlaceholderCommandTests.cs (stub only)

Application.Tests/Services/{EntityPlural}/Queries/
- PlaceholderQueryTests.cs (stub only)

--------------------------------
TABLE CLASSIFICATION (from DatabaseTables.md)
--------------------------------

Snapshot:
- Machines
- Users
- StockLots
- Orders
- Jobs

Lookup:
- Materials
- Parts

Ledger:
- Shifts
- StockLotAdjustments

Ledger rules:
- Prefer command-centric methods (CreateAsync, GetAsync, ListByParentAsync)
- BUT do NOT create any method files yet in this scaffolding step

--------------------------------
OUTPUT (NON-FILE)
--------------------------------

After creating files, PRINT a Markdown-style list in chat output
grouped by table showing which files were created.
This output must NOT be saved as a file.
