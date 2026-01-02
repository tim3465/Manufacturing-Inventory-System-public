# User & Role Architecture Rules

## Purpose
This document defines the **non-negotiable architectural rules** for User and Role handling in the CNC App.  
It exists to prevent accidental re-architecture during implementation (especially when using AI tools).

These rules are **intentional design decisions** and should be treated as constraints.

---

## Authentication & Authorization

### Identity Is the Authority

- ASP.NET Core **Identity** is used for:
  - Authentication (login, passwords)
  - Authorization (roles)
  - JWT token issuance
- **Identity roles are the single source of truth for access control**
- API authorization is enforced using Identity roles

There is **one role system for enforcement**.

---

## Roles

### Identity Roles

- Identity roles control **who can access which API endpoints**
- A user may have **multiple Identity roles**
- API endpoints may allow access via **OR logic** (any allowed role)

Roles are:
- Added and removed **only via Identity**
- Never inferred or granted by Domain logic

---

### Domain Roles

- The Domain does **not** own or enforce roles
- No Domain `Roles` table exists
- Any Domain role representation (e.g., enum) is:
  - Descriptive only
  - Read-only
  - Not authoritative

Domain roles must **never** be used to grant or deny API access.

---

## Domain User

### Domain User Exists

- A Domain `User` entity **does exist**
- It represents **business identity**, not authentication

Examples:
- Shop profile
- Business relationships
- Shifts
- Preferences
- Audit relationships

---

### Identity ↔ Domain Link

- Domain `User` links to Identity via:
  - `IdentityUserId`
- This is a **one-to-one relationship**
- Primary keys are **not shared**

---

### Email & Credentials

- Email is owned by **Identity**
- Domain User does **not** own email as a source of truth
- If Domain needs email, it is resolved via Identity

---

## User Creation

### Admin-Only Creation

- Users are created **by administrators only**
- There is no self-registration flow in this application
- This models internal business software intentionally

---

## Auditing

### Audit Fields

- All tables inherit audit fields
- Audit fields reference the **Identity user**
- `CreatedBy`, `UpdatedBy`, etc. represent `IdentityUserId`

No Domain role or Domain user is required for auditing to function.

---

## Explicit Non-Goals

The following are **out of scope** by design:

- Self-service user registration
- Domain-controlled authorization
- Multiple competing role systems
- SQL triggers to sync roles
- Bidirectional role synchronization

---

## Summary

- **Identity owns access**
- **Domain owns business meaning**
- **One enforcement system**
- **One source of truth**
- **No ambiguity**

These rules must be followed during implementation.
