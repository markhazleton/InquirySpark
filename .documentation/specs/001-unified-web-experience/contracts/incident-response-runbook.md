# Incident Response Runbook

**Spec:** `001-unified-web-experience`  
**Task:** T055  
**Status:** Active Operations Document

---

## Severity Definitions

| Severity | Definition | Response Time | Escalation |
|---|---|---|---|
| P1 — Critical | Total capability unavailability, data loss, or security incident | Immediate (< 15 min) | All team leads + management |
| P2 — High | Capability partially impaired, parity regression affecting users | < 1 hour | On-call lead |
| P3 — Medium | Degraded experience, cosmetic/UX issues, non-blocking errors | < 4 hours | Assigned developer |
| P4 — Low | Documentation gap, minor inconsistency | Next sprint | Backlog triage |

---

## Incident Categories

### SC-001: Total Capability Unavailability

**Trigger:** Unified area returns 500/503 for all users. Login succeeds but all `/Unified/*` routes fail.

**Response:**
1. Check application logs for unhandled exceptions
2. Verify `InquirySpark.db` file is present and readable (`Mode=ReadOnly`)
3. Verify `ControlSparkUser.db` is present and writable
4. Check IIS/Kestrel process status; restart if hung
5. If EF Core migration issue: databases are read-only in production — do NOT run `Database.Migrate()`
6. Escalate if unresolved within 15 minutes

### SC-002: Authentication Failure

**Trigger:** Users cannot log in; Identity pages return errors.

**Response:**
1. Check `ControlSparkUser.db` accessibility (ReadWriteCreate mode required — not ReadOnly)
2. Verify Identity services registered in `Program.cs`
3. Check `DataProtection` keys — if keys rotated, existing auth cookies are invalidated (users must re-login)
4. Verify `Cookie.SecurePolicy` and `Cookie.SameSite` are compatible with hosting environment

### SC-003: Authorization Regression

**Trigger:** Users can access routes they should not, or are incorrectly denied access.

**Response:**
1. Check policy definitions in `Program.cs` (`AddAuthorization(...)`)
2. Check `[Authorize(Policy="...")]` annotations on affected controller/action
3. Verify user claims (roles) are correctly assigned in Identity
4. If policy was recently changed: review the change and roll back if needed

### SC-004: Capability Parity Regression

**Trigger:** A capability that was functioning in the unified workspace now behaves differently from the legacy app (wrong data, missing features, broken UI).

**Response:**
1. Identify the affected capability ID from the URL or user report
2. Check `CapabilityCompletionMatrix` — confirm the capability's current phase
3. Compare unified output against legacy app output directly
4. If regression confirmed:
   - File GitHub issue with severity P1 or P2 
   - Roll back the specific capability with `RevertDomainCutoverAsync` if domain is Cut Over
   - Do not roll back entire domain unless multiple capabilities are affected
5. Document regression in `contracts/rollback-records/`

### SC-005: Performance Degradation

**Trigger:** Pages take >3 seconds to load; users report slowness.

**Response:**
1. Check for N+1 query patterns in EF Core calls (use `.Include()` appropriately)
2. Check database file location — SQLite read performance degrades on network shares
3. Review `IMemoryCache` usage — capability service uses in-memory state, but DB queries should be minimal
4. Consider adding response caching for read-only views if load is high

### SC-006: Legacy Application Reverted

**Trigger:** Traffic unexpected rerouted to legacy DecisionSpark or InquirySpark.Admin after a unified cutover.

**Response:**
1. Identify which domain was reverted and why
2. Check `RevertDomainCutoverAsync` was called and logged (`[UnifiedWeb] ROLLBACK: Domain...`)
3. Confirm legacy application is operational and accessible  
4. Open `rollback-integrity-checklist.md` and complete the checklist
5. File incident report with root cause
6. Resume monitoring window (reset 30-day clock after root cause fix)

### SC-007: Audit Log Gap

**Trigger:** Expected audit entries missing from structured logs; compliance review flags missing events.

**Response:**
1. Verify `IUnifiedAuditService` is registered (`UnifiedAuditService` implementation)
2. Verify log sink is configured to capture `Information` level (audit events are `Information` or above)
3. Check structured log retention policy — logs may have rotated
4. If emission gap confirmed in `UnifiedWebCapabilityService`, file P2 issue to add missing `_audit.Emit*` calls

---

## Post-Incident Procedure

After any P1 or P2 incident:

1. Complete incident record in `contracts/rollback-records/[incident-type]-[date].md`
2. Perform 5-why root cause analysis
3. Update relevant checklists (`operational-readiness.md`, `rollback-integrity-checklist.md`) if gaps found
4. Review whether constitution `performance-validation.md` or `pre-cutover-gate-criteria.md` needs updating
5. If incident reveals a missing gate criterion: open issue to add it before next cutover attempt

---

## Contact Matrix

| Role | Responsibility | Escalation Trigger |
|---|---|---|
| On-call developer | First responder, triage | All P1 and P2 incidents |
| Tech Lead | Root cause analysis, rollback authorization | Any P1; P2 if unresolved >1hr |
| Product Owner | Stakeholder communication, decommission decisions | Any P1 affecting end users |

_Note: Contact information is in the team's internal runbook (not committed to repo for security)._
