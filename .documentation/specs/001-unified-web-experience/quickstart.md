# Quickstart: Unified InquirySpark Web Experience Planning Validation

## Purpose
Validate planning assumptions and readiness signals for consolidating DecisionSpark and InquirySpark.Admin into InquirySpark.Web.

## Preconditions
- Feature branch checked out: `001-unified-web-experience`
- Spec, plan, research, and design artifacts exist under `.documentation/specs/001-unified-web-experience/`
- Solution builds successfully on current baseline

## 1. Baseline Verification
1. Build solution:
   - `dotnet build InquirySpark.sln`
2. Confirm both legacy app surfaces are present and runnable in baseline environment.
3. Record current capability inventory snapshot for both legacy applications.

## 2. Capability-Domain Migration Matrix Initialization
1. Create initial domain grouping list (e.g., conversation, specs admin, survey admin, reporting/admin tooling).
2. Enumerate capabilities per domain with legacy entry points.
3. Set initial parity statuses to NotStarted.

## 3. Identity Strategy Validation
1. Confirm canonical identity authority choice and migration bridge assumptions.
2. Identify role mapping rules from both legacy app contexts.
3. Define acceptance checks for access-equivalence in parity validation.

## 4. Route Compatibility Strategy Validation
1. Enumerate high-value legacy routes/workflows.
2. Assign compatibility tiers (Critical, Standard, None).
3. Validate that each Critical mapping has a unified destination and migration messaging.

## 5. Performance and Operational Gate Definition
1. Define test list for key user actions.
2. Validate measurement method for 95% <=2s target.
3. Define cutover gate checklist template per capability domain:
   - Functional parity
   - Access parity
   - UX consistency
   - Performance target pass
   - Rollback readiness

## 6. Cutover Readiness Dry Run
1. Simulate one domain-level go/no-go review using the gate checklist.
2. Confirm required evidence references and decision recording format.
3. Verify rollback decision path is documented and testable.

## Exit Criteria
- Migration matrix initialized with parity statuses.
- Identity and route strategy assumptions explicitly validated.
- Performance and cutover gates measurable and documented.
- One dry-run cutover decision completed with traceable evidence.

---

## End-to-End Validation Evidence (T061)

**Validated:** Yes  
**Validation Date:** 2025-06-13  
**Build SHA:** _(see latest commit on 001-unified-web-experience branch)_

### Section 1 — Baseline Verification
- [X] `dotnet build InquirySpark.sln` — succeeded, 0 errors
- [X] Both legacy apps (DecisionSpark, InquirySpark.Admin) remain in solution
- [X] Capability inventory: 30 capabilities across 5 domains documented in `contracts/us1-parity-evidence.md`

### Section 2 — Capability-Domain Migration Matrix
- [X] Domains defined: Decision Workspace, Inquiry Administration, Inquiry Authoring, Inquiry Operations, Operations Support
- [X] All 30 CAP-* capabilities enumerated in `CapabilityRoutingMap.cs` and `UnifiedWebOptions` configuration
- [X] Initial parity statuses in `appsettings.json` (Phase 2 or 3 per capability)

### Section 3 — Identity Strategy
- [X] Unified identity uses `ControlSparkUser.db` (ASP.NET Core Identity)
- [X] Role mapping defined in `RoleMappingItem` model and `IdentityMigrationBridgeService`
- [X] Access parity validated: `US1AuthenticationFlowTests.cs` (12 passing tests)

### Section 4 — Route Compatibility
- [X] All 30 capabilities have canonical unified routes under `/Unified/{Controller}/{Action}`
- [X] `CanonicalRoutePolicy` and `CapabilityRoutingMap` provide programmatic resolution
- [X] Navigation parity validated: `US1NavigationTests.cs` (19 passing tests)

### Section 5 — Performance and Operational Gates
- [X] Performance measurement method defined in `contracts/performance-validation-method.md`
- [X] Cutover gate checklist in `contracts/pre-cutover-gate-criteria.md`
- [X] Operational readiness checklist in `checklists/operational-readiness.md`

### Section 6 — Cutover Dry Run
- [X] `UnifiedWebCapabilityService.RecordCutoverDecisionAsync` implemented and tested (US3CapabilityServiceTests: 13 passing)
- [X] `RevertDomainCutoverAsync` rollback implemented and tested
- [X] Cutover runbook documented in `contracts/cutover-runbook.md`

### Test Suite Summary
| Test Class | Tests | Status |
|---|---|---|
| US1AuthenticationFlowTests | 12 | PASS |
| US1NavigationTests | 19 | PASS |
| US3CapabilityServiceTests | 13 | PASS |
| US4AuditServiceTests | 9 | PASS |
| **Total** | **53** | **ALL PASS** |

