# Post-Deploy Decommission Verification Evidence

**Spec:** `001-unified-web-experience`  
**Task:** T067  
**Date:** 2026-04-14  
**Status:** PASS

---

## Purpose

Record that `DecisionSpark` and `InquirySpark.Admin` are fully decommissioned from the active solution and that the unified `InquirySpark.Web` application has assumed all capability delivery. This document satisfies the decommission-readiness requirement per `contracts/route-and-decommission-readiness.md` and the operational readiness checklist item under "Decommission Readiness."

---

## 1. Solution Decommission — Project Removal (T063 / T064)

### Active projects in `InquirySpark.sln` (verified 2026-04-14)

| Project | Type | Status |
|---|---|---|
| `InquirySpark.Common` | Class library (shared domain) | ✅ Active |
| `InquirySpark.Repository` | Class library (EF Core + services) | ✅ Active |
| `InquirySpark.Common.Tests` | MSTest project | ✅ Active |
| `InquirySpark.Web` | ASP.NET Core MVC web app | ✅ Active |
| `DecisionSpark` | — | ✅ **Removed** (T063) |
| `InquirySpark.Admin` | — | ✅ **Removed** (T064) |

**Evidence**: `get-content InquirySpark.sln` shows exactly 4 `csproj` references — no `DecisionSpark` or `InquirySpark.Admin` entries remain.

---

## 2. Deployment Documentation Update (T065)

### README.md — Verified 2026-04-14

The README no longer lists DecisionSpark or InquirySpark.Admin as runnable applications. The following lines confirm the updated state:

```
- **InquirySpark.Web**: Unified operations workspace (Bootstrap 5 + DataTables, Razor Views, ASP.NET Core Areas)
> **Legacy applications removed**: `DecisionSpark` and `InquirySpark.Admin` have been decommissioned
  and are no longer part of the active solution. Their capabilities are fully delivered by `InquirySpark.Web`.
dotnet run --project InquirySpark.Web
```

**Status:** ✅ PASS — Only `InquirySpark.Web` is referenced as the runnable application.

---

## 3. CI/CD Pipeline Verification (T066)

### `.github/workflows/sqlite-baseline.yml` — Verified 2026-04-14

The active CI pipeline contains:

| Job | Target | Legacy References |
|---|---|---|
| `build-and-test` | `dotnet build InquirySpark.sln` | None |
| `web-npm-build` | `InquirySpark.Web/` npm pipeline | None |
| `sqlite-data-integrity` | SQLite file integrity check | None |
| `build-summary` | Status report | None |

No `DecisionSpark` or `InquirySpark.Admin` build steps, test invocations, or deployment actions remain in any workflow file. Legacy project removal from the solution file propagates automatically to `dotnet build InquirySpark.sln` — no per-project CI steps existed for legacy apps.

**Status:** ✅ PASS — CI/CD pipeline is free of legacy project references.

---

## 4. Capability Coverage Verification

All 30 CAP-* capabilities previously split across DecisionSpark and InquirySpark.Admin are delivered by InquirySpark.Web under the Unified area.

| Legacy App | Capability Domains | Unified Replacement |
|---|---|---|
| DecisionSpark | Decision Workspace (conversations, decision specs, AI chat) | `DecisionConversationController`, `DecisionSpecificationController` |
| InquirySpark.Admin | Inquiry Administration (applications, users, roles, site menus) | `InquiryAdministrationController` |
| InquirySpark.Admin | Inquiry Authoring (surveys, questions, groups, answers) | `InquiryAuthoringController` |
| InquirySpark.Admin | Inquiry Operations (companies, import history, review status) | `InquiryOperationsController` |
| Both | Operations Support (charting, health, API parity, roles, preferences) | `OperationsSupportController` |

Full parity evidence recorded in `contracts/us1-parity-evidence.md`.

---

## 5. Test Suite Verification (post-decommission)

Build and test results captured in `contracts/validation-evidence.md`:

| Gate | Result |
|---|---|
| `dotnet build InquirySpark.sln -warnaserror` | ✅ 0 errors, 0 warnings |
| `dotnet test` — feature-specific tests (53) | ✅ 53/53 PASS |
| EF migrations check (`dotnet ef migrations list`) | ✅ Zero new migrations |
| US1AuthenticationFlowTests (12) | ✅ PASS |
| US1NavigationTests (19) | ✅ PASS |
| US3CapabilityServiceTests (13) | ✅ PASS |
| US4AuditServiceTests (9) | ✅ PASS |

---

## 6. Pre-Cutover Gate Summary

Verified against `contracts/pre-cutover-gate-criteria.md`:

| Gate ID | Gate | Result |
|---|---|---|
| G-001 | Zero-Warning Build | ✅ PASS |
| G-002 | All Feature Tests Pass | ✅ PASS |
| G-003 | Functional Parity Evidence | ✅ PASS (us1-parity-evidence.md) |
| G-004 | Permission Parity Evidence | ✅ PASS (US1AuthenticationFlowTests) |
| G-005 | Performance Validation | ⏳ Runtime validation required — see note below |
| G-006 | Navigation Parity | ✅ PASS (US1NavigationTests) |
| G-007 | Rollback Capability Verified | ✅ PASS (US3CapabilityServiceTests) |
| G-008 | Post-Cutover Monitoring Plan | ✅ PASS (stakeholder-communication-pack.md) |

> **G-005 Note:** Performance validation (95% of key actions ≤ 2 seconds) requires a running application instance against the production SQLite database. This gate must be verified during the first production deployment. Evidence must be recorded here (update "PENDING" row above) before the 30-day monitoring window begins.

---

## 7. 30-Day Post-Cutover Monitoring Status

Per `contracts/cutover-runbook.md`, ops must track SC-004 (functional regression incidents) and SC-006 (user-reported access failures) for 30 days after each domain cutover.

| Domain | Cutover Date | 30-Day Window Ends | SC-004 Incidents | SC-006 Incidents | Status |
|---|---|---|---|---|---|
| Decision Workspace | Pending deployment | — | — | — | ⏳ Pending |
| Inquiry Administration | Pending deployment | — | — | — | ⏳ Pending |
| Inquiry Authoring | Pending deployment | — | — | — | ⏳ Pending |
| Inquiry Operations | Pending deployment | — | — | — | ⏳ Pending |
| Operations Support | Pending deployment | — | — | — | ⏳ Pending |

_Update this table after each domain enters production and after the 30-day window closes. Sign off with name and date when all domains reach "Closed — No P1/P2 incidents."_

---

## 8. Physical Artifact Retention

The `DecisionSpark/` and `InquirySpark.Admin/` source directories remain in the repository tree for historical reference but are excluded from the active solution and all build/run pipelines. They are not deployed.

If these directories are to be physically removed from the repository, that action requires a separate decommission task and explicit approval, as it is irreversible. No such removal has been performed or is required for this spec.

---

## Summary

| Verification Item | Status |
|---|---|
| DecisionSpark removed from InquirySpark.sln | ✅ PASS |
| InquirySpark.Admin removed from InquirySpark.sln | ✅ PASS |
| README.md deployment section updated | ✅ PASS |
| CI/CD pipeline free of legacy project references | ✅ PASS |
| All 30 capabilities delivered by InquirySpark.Web | ✅ PASS |
| Zero-warning build confirmed | ✅ PASS |
| Feature test suite (53 tests) all passing | ✅ PASS |
| Post-cutover monitoring window active | ⏳ Pending first production deployment |

**Overall decommission verification: PASS** — all solution-level, documentation, and pipeline decommission steps are complete. Post-cutover monitoring (G-005 runtime perf + 30-day incident tracking) is deferred to first production deployment.

---

_Signed off: Mark Hazleton — 2026-04-14_
