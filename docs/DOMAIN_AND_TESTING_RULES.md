# CNC App — Domain & Testing Rules (Locked)

## Purpose
This document defines the **non-negotiable rules** for:
- Domain safeguards (invariants)
- Application workflows
- Testing responsibilities

These rules are intentionally locked to prevent architectural drift as new slices are generated quickly using AI assistance.

---

## Core Principle

> **The domain protects valid state.  
> The application orchestrates workflows.  
> The database enforces integrity as a last line of defense.**

---

## Domain Rules (Invariants)

### What Belongs in the Domain
The Domain layer enforces **invariants** — rules that must *always* be true for an entity.

Examples:
- Required values (non-null, non-whitespace)
- Value shape (length, ranges, ordering like Start < End)
- State transitions (cannot inactivate twice)
- Relationships that must always exist (e.g., Shift must belong to a Job)

These rules must be enforced **without database access**.

The domain should make invalid states:
- impossible, or
- fail immediately via a DomainException

---

### What Does NOT Belong in the Domain
The domain does **not**:
- Query the database
- Check existence of other aggregates
- Enforce authorization or roles
- Implement workflows or sequencing

Those belong in the Application layer.

---

## Application Rules (Workflows)

The Application layer:
- Orchestrates use-cases
- Coordinates repositories
- Checks existence of related entities
- Handles cross-aggregate rules
- Calls domain methods assuming invariants are enforced

Application services should:
- Call domain factory/methods
- Not duplicate invariant checks
- Let DomainExceptions bubble naturally

---

## Database Rules

The database enforces:
- Foreign keys
- NOT NULL constraints
- Unique indexes
- Precision/scale constraints

Database constraints are **not a substitute** for domain invariants.

They are the final safety net.

---

## Testing Philosophy (Locked)

### Domain Tests
- Test **whether invalid states are possible to create**
- Do NOT:
  - Query the database
  - Test workflows
  - Test EF mappings
- Examples:
  - Creating an entity with invalid data throws
  - Illegal state transitions are blocked

---

### Application Tests
- Test **use-case workflows**
- Call application service methods directly (not controllers)
- Assume domain invariants are correct
- Do NOT re-test domain rules

---

### Persistence / Integration Tests (Optional)
- Verify EF mappings, relationships, and SaveChanges behavior
- Used only when persistence behavior is non-trivial

---

## Rule of Thumb

> If a rule can be violated **without touching the database**, it belongs in the Domain.  
> If a rule depends on **other data or sequencing**, it belongs in the Application.

---

## Enforcement
All new slices must follow these rules.
Any AI-generated code that violates them must be corrected before merge.
