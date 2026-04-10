# Orchestration Escalation Report

Use this report format when orchestration cannot continue cleanly.

---

## Format

### Orchestration Escalation Report
- **Phase:** which phase failed (backend triage / backend implementation / frontend triage / frontend implementation)
- **Summary of attempts:** what was tried and what feedback was given
- **Remaining blockers:** what is still unresolved
- **Recommended next step:** suggested direction or action for the user

---

## When to Use It

Emit this report when any of the following occur:

- Backend triage exceeds 2 revisions without resolution
- Frontend triage exceeds 2 revisions without resolution
- Frontend implementation review exceeds 2 revisions without resolution
- Required inputs are missing and cannot be recovered
- A new table is proposed that is not in the approved plan and the user has not approved it
- An approved table structure does not match the issue body and the user has not approved the discrepancy
- Implementation cannot continue without a user decision that has not been provided
