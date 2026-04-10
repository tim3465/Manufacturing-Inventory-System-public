# Agent Output Contracts

Required section headings for each report type. This skill depends on specialist agents
returning these exact headings. If they are missing, gating and review logic may drift.

---

## Backend Triage Report

Returned by `backend-implement` during the triage-only pass (Step 4).

- **Target areas inspected**
- **Best escalation path**
- **Proposed file changes**
- **Proposed endpoints**
- **Proposed DTO shapes**
- **Proposed new tables**
- **Risks or unknowns**
- **Questions requiring approval**

---

## Backend Complete Summary

Returned by `backend-implement` after implementation (Step 6).

- **Summary**
- **Files changed/added**
- **New tables and migrations**
- **Endpoints exposed**
- **DTO shapes**
- **Build/test verification**
- **Notes or follow-ups**

---

## Frontend Triage Report

Returned by `frontend-implement` during the triage-only pass (Step 8).

- **Target files inspected**
- **Best escalation path**
- **Proposed file changes**
- **Components to create or modify**
- **API clients to create or update**
- **Signal/form/state patterns to apply**
- **Risks or unknowns**
- **Questions requiring approval**

---

## Frontend Complete Summary

Returned by `frontend-implement` after implementation (Step 9).

- **Summary**
- **Files changed/added**
- **Components created or modified**
- **API clients created or updated**
- **How to verify in the browser**
- **Notes or follow-ups**

---

## Agent File Update Requirement

Before relying on this orchestration flow, ensure both agent files are updated:

- `backend-implement.md` — must support a triage-only pass and return the headings above
- `frontend-implement.md` — must support a triage-only pass and return the headings above
