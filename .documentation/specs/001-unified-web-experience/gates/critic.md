---
gate: critic
status: fail
blocking: true
severity: showstopper
summary: "Project plan proposes writing state (audit logs, migration config) to a database explicitly defined as read-only and immutable by the project constitution."
---

## Technical Risk Assessment

**Analysis Date:** 2026-04-12
**Risk Posture:** RED
**Detected Stack:** C# + ASP.NET Core MVC/Razor + Entity Framework Core (SQLite)

### Executive Summary
This project cannot proceed to implementation. The plan relies on creating new database models and writing state (audit events, capability matrices, cutover decisions) to a SQLite database. However, the project Constitution explicitly dictates that all SQLite databases are strictly `Mode=ReadOnly`, `.db` assets are immutable, and `Database.Migrate()` is disabled. Every write operation or schema update will result in a fatal exception. 

### Showstopper Risks (Must Fix Before Implementation)

| ID | Category | Location | Risk Description | Likely Impact | Mitigation Required |
|----|----------|----------|------------------|---------------|---------------------|
| 1 | Architecture / Constitution Violation | `tasks.md` (T005-T009, T048-T051) | The plan defines new EF models (CapabilityItem, UnifiedAuditEventItem) and implies writing tracking data (ParityValidationRecord, CutoverDecision) to the database. Constitution §II states SQLite is `Mode=ReadOnly`, immutable, and migrations are disabled. | **System Crash:** EF Core will throw exceptions on any `SaveChanges()` or migration attempt. | Define a separate, writable data store for migration tracking and audit logs, or revise the Constitution if the read-only constraint is obsolete. |
| 2 | Security & Identity | `spec.md` (FR-015), `tasks.md` (T013) | Merging identities into a "canonical identity authority" without a writable database means role-mapping and identity bridging exist only in-memory or in config, which is non-scalable and prone to data loss upon app restart. | **Authentication Failure:** Inability to persist identity mappings will cause login failures and privilege escalation risks. | Implement an external identity provider or a dedicated writable auth database. |

### Critical Risks (High Probability of Costly Issues)

| ID | Category | Location | Risk Description | Likely Impact | Recommended Action |
|----|----------|----------|------------------|---------------|--------------------|
| 3 | Performance / Scale | `spec.md` (FR-013) | The target of 95% of key actions completing in <=2 seconds is at risk if cross-domain queries execute synchronously over massive read-only SQLite files without pagination or indexing strategies explicitly mentioned. | Slow page loads and timeout errors. | Add explicit caching (Redis/In-Memory) for the "capability matrix" and role mappings. |
| 4 | Operational Hazard | `tasks.md` (T047A) | Rollback is defined as "code-level rollback guards" to revert domain cutover status. If state cannot be written to DB, rolling back requires application redeployment or config file edits. | Prolonged downtime during a rollback scenario. | Externalize migration state to a feature flag management system (e.g., Azure App Configuration, LaunchDarkly). |
| 5 | Implementation Trap | `tasks.md` (T028, T029) | "Greenfield capability controllers" assume existing repository services are perfectly decoupled from their legacy UI models. Legacy services often return ViewModels directly or rely on legacy `HttpContext`. | Extensive rework of `InquirySpark.Repository` services. | Add tasks to audit and decouple legacy repository methods before UI implementation. |

### High-Priority Concerns

| ID | Category | Location | Issue | Impact | Suggestion |
|----|----------|----------|-------|--------|------------|
| 6 | Testing | `tasks.md` | Missing explicit end-to-end (E2E) testing framework tasks (e.g., Playwright) for a complex UX migration. | UX regressions slip into production. | Add tasks to establish E2E tests for the MVP (User Story 1). |
| 7 | Deployment | `tasks.md` | Integration of `InquirySpark.Web` into CI/CD pipelines is missing context regarding environment variables for the new "Unified" settings. | Production deployment failures. | Add explicit tasks to update CI/CD secrets and configuration. |

### Framework-Specific Red Flags

**C# + ASP.NET Core + EF Core (SQLite):**

- [x] Database writes attempted against `Mode=ReadOnly` connection strings.
- [x] Implied Entity Framework migrations on immutable `.db` files.
- [ ] Concurrency bottlenecks with SQLite (mitigated if truly read-only, but exacerbated by unified traffic).
- [ ] No distributed caching strategy for session management in the unified app.

### Architecture Red Flags

- [ ] Over-engineered for stated requirements
- [x] Under-engineered for implied scale (assuming config files can hold dynamic migration state)
- [x] Single point of failure without redundancy (unified app taking all traffic previously split)
- [x] Missing standard patterns for problem domain (Feature flags for migration cutover)
- [ ] Inadequate async/concurrency handling

### Missing Critical Tasks

- **Ops/Infrastructure:** Provisioning a writable data store for audit logs and migration state.
- **Ops/Infrastructure:** Feature flag service integration for safe, runtime cutovers that don't require database writes or app restarts.
- **Testing:** Automated UI/E2E testing (Playwright/Selenium) to guarantee UX consistency and cross-domain navigation.
- **Observability:** Distributed tracing (OpenTelemetry) to track performance across the "canonical identity bridge" and legacy repositories.

### Questionable Assumptions

1. **Assumption:** All migration state, parity records, and audit events can be persisted using standard `InquirySpark.Repository` EF patterns.
   **Why this will fail:** The Constitution completely forbids writing to the SQLite databases and disables EF migrations.
2. **Assumption:** Existing repository services can seamlessly back the greenfield `InquirySpark.Web` UX.
   **Why this will fail:** Legacy services often contain hidden assumptions about the requester's identity context, legacy session state, or return DTOs tightly coupled to the old UIs.

### Dependencies Risk Assessment

| Dependency | Concern | Alternative to Consider |
|------------|---------|-------------------------|
| EF Core SQLite | Immutable and read-only per Constitution, making it impossible to track migration state natively. | File-based JSON state (if single instance), Redis, or an external Feature Flag provider. |

### Estimated Technical Debt at Launch

- **Code Debt:** High coupling in the `IdentityMigrationBridgeService` if trying to hack around the read-only DB constraint.
- **Operational Debt:** High, due to manual deployment required to toggle "code-level rollback guards".
- **Documentation Debt:** Low, documentation tasks are comprehensive.
- **Testing Debt:** High, due to reliance on unit/integration tests without E2E browser automation for a major UX rewrite.

### Metrics

- Showstopper Count: 2
- Critical Risk Count: 3
- Missing Operational Tasks: 4 
- Underspecified Security Requirements: 1
- Scale Bottlenecks Identified: 1

**GO/NO-GO RECOMMENDATION:**

```text
[X] STOP - Showstoppers present, cannot proceed to implementation
[ ] CONDITIONAL - Fix critical risks first, then reassess
[ ] PROCEED WITH CAUTION - Document acknowledged risks, add mitigation tasks
```

**Required Actions Before Implementation:**

1. Resolve the Constitution violation: Either amend the Constitution to allow a writable database for `InquirySpark.Web` (e.g., a dedicated operational tracking DB), or completely remove all tasks requiring EF models and database writes for migration parity, cutover decisions, and audit events, replacing them with a strict configuration-based or external feature flag approach.
2. Detail exactly how the `IdentityMigrationBridgeService` will persist role mappings and unified session state without a writable database.

**Recommended Risk Mitigations:**

- Add tasks for configuring an external Feature Flag provider (like Azure App Config) to manage the phase cutover without needing database writes or app redeployments.
- Add Playwright E2E UI testing tasks to definitively prove the "Unified Navigation" and "UX Consistency" stories.