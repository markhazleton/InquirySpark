# Operational Readiness Checklist

**Spec:** `001-unified-web-experience`  
**Task:** T054  
**Status:** Pre-launch validation checklist

Complete this checklist before declaring any domain ready for production cutover.

---

## Build and Test Gate

- [X] `dotnet build InquirySpark.sln` — 0 errors, 0 warnings
- [X] `dotnet test` — All tests pass
- [X] US1AuthenticationFlowTests — all passing
- [X] US1NavigationTests — all 19 passing
- [X] US3CapabilityServiceTests — all 13 passing
- [X] US4AuditServiceTests — all passing
- [X] No TODO or FIXME comments in production code paths

## Capability Completion Matrix Check

- [ ] `/Unified/CapabilityCompletionMatrix` loads without error
- [ ] Target domain shows all capabilities at Phase 3 or higher
- [ ] No capability in the target domain is in "not-started" or "scoped" status

## Operational Readiness Dashboard Check

- [ ] `/Unified/OperationalReadiness` loads without error
- [ ] Target domain shows status "Ready" or "Cut Over"
- [ ] `ReadyForCutoverDomains` count matches expectation

## Pre-Cutover Gate Criteria Verification

Verified against `contracts/pre-cutover-gate-criteria.md`:

- [X] G-001 Zero-Warning Build — PASS
- [X] G-002 All Tests Pass — PASS
- [X] G-003 Functional Parity Evidence — PASS
- [X] G-004 Permission Parity Evidence — PASS
- [ ] G-005 Performance Validation — PASS
- [X] G-006 Navigation Parity — PASS
- [X] G-007 Rollback Capability Verified — PASS
- [X] G-008 Post-Cutover Monitoring Plan — PASS

## Audit Infrastructure Check

- [X] `IUnifiedAuditService` registered in DI with `UnifiedAuditService` implementation
- [X] Structured log entries appear for parity validation events (`UC.Parity.ValidationSubmitted`)
- [X] Structured log entries appear for cutover decision events (`UC.Cutover.DecisionRecorded`)
- [X] Structured log entries appear for rollback events (`UC.Cutover.Reverted`) with `Critical` severity
- [X] No audit events contain PII beyond user ID

## Authorization Check

- [ ] Analyst role can access Decision Workspace capabilities
- [ ] Operator role can access Inquiry Operations capabilities
- [ ] Consultant role can access appropriate read-only capabilities
- [ ] Executive role can access readonly views
- [ ] Anonymous user is redirected to login for all protected routes
- [ ] Health endpoint (`/Unified/OperationsSupport/Health`) accessible with AllowAnonymous

## Identity and Authentication

- [ ] Login/logout flow works end-to-end
- [ ] ASP.NET Core Identity database accessible
- [ ] Cookie expiry is set appropriately (no "remember me" by default)
- [ ] Password requirements enforced

## Capability Route Verification

- [X] All 30 CAP-* capabilities have an accessible unified route
- [ ] All unified routes resolve with HTTP 200 for authorized users
- [X] No legacy-only routes remain in navigation without a unified equivalent

## Decommission Readiness (when domain is Cut Over)

- [ ] Legacy application removed from active solution (T063/T064)
- [ ] README.md deployment section updated (T065)
- [ ] CI/CD pipeline updated — legacy project build removed (T066)
- [ ] 30-day monitoring window started; SC-004/SC-006 tracking active

---

_Completion record: [domain] [date] — signed off by [name]_
