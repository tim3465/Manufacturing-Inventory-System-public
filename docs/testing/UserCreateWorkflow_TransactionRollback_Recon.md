### Testing Recon: User Create Workflow — Transaction Rollback

### Locked testing philosophy (do not deviate)

- **Domain tests**
  - Verify domain invariants and prevent invalid states.
  - **No DB access**.
  - Focus on entities/value objects and pure business rules.

- **Application tests**
  - Verify workflows/use-cases by calling **service methods** (not controllers).
  - Should not re-test domain invariants (assume domain layer already covers those).
  - Can be **integration-style** when validating cross-system behavior (e.g., transactions).

### Problem definition (what we are testing)

Current user provisioning flow is multi-step:

- Create **Identity user** (ASP.NET Core Identity / `UserManager`)
- Assign **Identity roles**
- Create **Domain user** (`DomainUsers` table) linked by `IdentityUserId`

Failure mode:

- Identity user can be created successfully, then a later step fails (role assignment, domain user save, etc.).
- This can leave an **orphan Identity user** (Identity exists, Domain user does not).
- Retries then fail because **Identity username/email already exists**.

### Target behavior (Definition of Done)

Checklist:

- [ ] If the workflow fails at **any** step (e.g., invalid role), the system **must not** leave an orphan Identity user.
- [ ] After a failure, a **retry** with the same email should succeed (no duplicate email conflict from the failed attempt).

In other words: the workflow must be **effectively atomic** from the caller’s perspective.

### Where the test belongs (scope + category)

- This is an **Application-level workflow test**.
- It does **not** belong in Domain tests.
- We are **not** creating a new test category; we will add an **integration-style harness** under existing Application tests (e.g., `CncApp.Application.Tests`).

### Why EF InMemory is insufficient

EF Core InMemory provider:

- Does not behave like a relational database.
- **Does not provide real transactional guarantees** (rollback semantics differ).
- Therefore it cannot validate that “Identity + Domain writes are rolled back together” or that partial writes are cleaned up.

### Recommended harness options

#### Primary (recommended): SQLite in-memory with an open connection

Reason:

- Runs fast in tests.
- Uses a relational provider with transactions.
- Can keep the database alive for the test lifetime by holding an open `SqliteConnection`.

Sketch:

```csharp
// Pseudocode
var connection = new SqliteConnection("DataSource=:memory:");
await connection.OpenAsync();

var options = new DbContextOptionsBuilder<AppDbContext>()
  .UseSqlite(connection)
  .Options;

using var db = new AppDbContext(options, currentUserService: null);
await db.Database.EnsureCreatedAsync();
```

#### Alternatives

- **Testcontainers** (real SQL Server/Postgres/etc.)
  - Highest fidelity, slower, requires Docker.
- **Dedicated test database**
  - Higher ops overhead, risk of shared-state flakiness unless isolated per run.
- **SQLite file-based database**
  - More persistent than in-memory, still lightweight, but slower than pure in-memory.

### Proposed tests (red → green)

#### Initial failing test (RED)

Name:

- `CreateUserAsync_WhenRoleInvalid_DoesNotLeaveOrphanIdentityUser`

Arrange:

- Build `CreateUserRequestDto` with:
  - `Email = "rollback@example.com"`
  - `TemporaryPassword = "TempPass123!"`
  - `Roles = ["NotARealRole"]`
- Use real `UserService` with real `IdentityProvisioningService` and real `UserRepository`, backed by SQLite in-memory.

Act:

- Call `UserService.CreateAsync(dto)` and expect it to fail (exception or failure result depending on current behavior).

Assert **BEFORE fix** (current expected failing behavior):

- Identity user exists (by email) **== true**
- Domain user does not exist (by `IdentityUserId`) **== true**

Sketch assertions:

```csharp
// Pseudocode
await Assert.ThrowsAsync<InvalidOperationException>(() => userService.CreateAsync(dto));

var identityUser = await userManager.FindByEmailAsync(dto.Email);
identityUser.Should().NotBeNull();              // orphan exists (bad)

var domainUser = await db.DomainUsers.SingleOrDefaultAsync(u => u.IdentityUserId == identityUser.Id);
domainUser.Should().BeNull();
```

#### Post-fix test expectation (GREEN)

Same test name, same arrange/act, but asserts become:

- Identity user exists **== false**
- Domain user exists **== false**

```csharp
await Assert.ThrowsAsync<InvalidOperationException>(() => userService.CreateAsync(dto));

(await userManager.FindByEmailAsync(dto.Email)).Should().BeNull();
// Domain user obviously absent as well
```

### Implementation plan (high-level)

- **Step 1**: Add the failing Application test using **SQLite in-memory** harness.
- **Step 2**: Implement **Option A**: enforce a transaction boundary around the Identity + Domain create operations (or equivalent compensating delete if Identity can’t participate in the same transaction).
- **Step 3**: Verify the test turns **green**.
- **Step 4**: Add regression test for success path:
  - `CreateUserAsync_WhenValidRole_CreatesIdentityAndDomainUser`

### Open questions

- **DbContext/connection alignment**
  - Does ASP.NET Identity (`UserManager` / Identity stores) and DomainUsers use the **same `AppDbContext` instance and underlying connection** in the test harness?
  - If not, a single EF transaction may not cover both operations and we’ll need compensating behavior.

- **Role assignment failure semantics**
  - When a role does not exist, does `AssignRolesAsync`:
    - throw (`InvalidOperationException`), or
    - return a failure result that we translate?
  - The test should match actual behavior (exception type/message may differ).


