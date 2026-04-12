---
gate: analyze
status: warn
blocking: false
severity: warning
summary: "3 CRITICAL, 4 HIGH, 6 MEDIUM, 3 LOW findings across spec/plan/tasks. Critical issues involve constitution violations (docs in project folders) and missing project scaffold task. Recommend resolving CRITICAL items before /devspark.implement."
---

# Specification Analysis Report

**Feature**: Unified InquirySpark Web Experience
**Artifacts**: spec.md, plan.md, tasks.md
**Analysis Date**: 2026-04-12
**Constitution Version**: 1.1.0

## Findings

| ID | Category | Severity | Location(s) | Summary | Recommendation |
|----|----------|----------|-------------|---------|----------------|
| D1 | Constitution | CRITICAL | tasks.md T004 | T004 creates `InquirySpark.Web/Areas/Unified/README.md` — a `.md` file inside a project folder, violating Constitution § V (all docs under `/.documentation/`). | Move target to `.documentation/specs/001-unified-web-experience/contracts/unified-area-readme.md` or remove task. |
| D2 | Constitution | CRITICAL | tasks.md T031 | T031 creates `InquirySpark.Web/Areas/Unified/UX/UnifiedUxConventions.md` — another `.md` in a project folder, violating Constitution § V. | Move target to `.documentation/specs/001-unified-web-experience/contracts/unified-ux-conventions.md`. |
| E1 | Coverage | CRITICAL | tasks.md (all phases) | No task scaffolds the `InquirySpark.Web` project itself (csproj, Program.cs, Area registration). The project does not exist in the workspace. T016+ assume it already exists. | Add a Phase 2 task (before T005) to create `InquirySpark.Web/InquirySpark.Web.csproj`, `Program.cs`, and register the Unified Area. |
| E2 | Coverage | HIGH | spec.md FR-004, tasks.md Phase 2 | FR-004 requires role/permission preservation and "role mapping across identity migration," but no task creates a role mapping data model or migration logic. T012-T013 cover identity bridge only. | Add tasks for role mapping DTO in `InquirySpark.Common/Models/UnifiedWeb/` and role migration logic in the identity bridge service. |
| E3 | Coverage | HIGH | spec.md FR-006, tasks.md Phase 5 | FR-006 requires rollback-safe cutover controls. T046 is a documentation runbook only — no code-level rollback mechanism task exists. | Add a task implementing rollback logic/guards in `UnifiedWebMigrationService` or a dedicated rollback service. |
| B1 | Ambiguity | HIGH | spec.md FR-013, SC-007 | "Key user actions" that must complete in ≤2 seconds are never enumerated. T019 adds instrumentation config but the target action list is undefined. | Define the explicit list of key user actions in spec.md or a contracts document before T019. |
| B2 | Ambiguity | HIGH | spec.md SC-004 | SC-004 requires 50% decrease in support requests vs. baseline, but no task captures baseline measurement and no mechanism for measurement exists. | Add a pre-migration baseline measurement task or revise the success criterion to a verifiable metric. |
| E4 | Coverage | MEDIUM | tasks.md (all phases) | No automated test tasks. tasks.md acknowledges "test-first tasks were not requested." T062 runs `dotnet test` but no tasks create new tests for the unified services or controllers. | Add test tasks per phase or at minimum per user story checkpoint for new services (UnifiedWebMigrationService, IdentityMigrationBridgeService, navigation builder). |
| F1 | Inconsistency | MEDIUM | spec.md, tasks.md | Terminology drift: spec uses "migration" and "migration bridge"; tasks use "capability completion" and "completion bridge" interchangeably for the same concepts. | Standardize on one term set. Recommend "capability completion" in user-facing contexts, "migration" for technical/internal references. Document in T035 terminology map. |
| F2 | Inconsistency | MEDIUM | spec.md clarifications, tasks.md T028-T029 | Spec clarification states "greenfield-only development" with no legacy URL handling, yet tasks create "representative capability shells for former DecisionSpark/Admin domains" — implying porting rather than greenfield design. | Clarify in spec whether "greenfield" means new UX over existing services (likely intent) or entirely new implementations. Update task descriptions to remove ambiguity. |
| E5 | Coverage | MEDIUM | spec.md FR-014 | FR-014 requires post-cutover validation procedures. Only T062 (build/test) and T067 (decommission evidence) exist — no dedicated functional parity validation task post-cutover. | Add an explicit post-cutover parity validation task using the capability matrix as the verification checklist. |
| A1 | Duplication | MEDIUM | spec.md FR-005, FR-010 | FR-005 (phased capability completion) and FR-010 (capability migration matrix) overlap significantly — both describe tracking capability-level migration progress. | Consider merging FR-010 into FR-005 as the implementation mechanism, or clarify FR-010 as the specific artifact while FR-005 is the process requirement. |
| A2 | Duplication | MEDIUM | spec.md FR-011, FR-014 | FR-011 (objective completion criteria per domain) and FR-014 (post-cutover validation procedures) overlap — both define verification before/after cutover. | Distinguish FR-011 as pre-cutover gate criteria and FR-014 as post-cutover runtime validation to eliminate overlap. |
| E6 | Coverage | LOW | tasks.md T002 | T002 creates a "capability inventory seed document" but no task performs the actual capability discovery/enumeration from DecisionSpark and InquirySpark.Admin source code. | Add a research or analysis task to systematically enumerate capabilities from both legacy apps before seeding the inventory. |
| B3 | Ambiguity | LOW | spec.md FR-007 | "Existing operational auditability" lacks specificity — which audit events are required? Legacy apps may have different audit coverage. | Enumerate required audit event types in spec or contracts before implementing T048-T050. |
| C1 | Style | LOW | tasks.md T001 | T001 records "greenfield-only route policy" in spec.md, but spec.md already states this in clarifications and FR-008. Redundant documentation task. | Remove T001 or redirect it to update a contracts document instead of spec.md. |

## Coverage Summary Table

| Requirement Key | Has Task? | Task IDs | Notes |
|-----------------|-----------|----------|-------|
| FR-001 (full-parity) | Partial | T028, T029, T041 | No systematic capability enumeration task |
| FR-002 (unified-entry) | Yes | T020, T013, T016 | |
| FR-003 (single-navigation) | Yes | T023, T024, T025 | |
| FR-004 (role-permission) | Partial | T012, T013 | Missing role mapping data model and logic |
| FR-005 (phased-completion) | Yes | T038-T045 | |
| FR-006 (rollback-cutover) | Partial | T044, T045, T046 | Documentation only — no code-level rollback |
| FR-007 (auditability) | Yes | T048-T051 | Audit event types unspecified |
| FR-008 (canonical-routes) | Yes | T026, T027 | |
| FR-009 (consistent-ux) | Yes | T031-T037 | |
| FR-010 (migration-matrix) | Yes | T038-T040 | Overlaps FR-005 |
| FR-011 (completion-criteria) | Partial | T043, T046 | Overlaps FR-014 |
| FR-012 (stakeholder-comms) | Yes | T047 | |
| FR-013 (performance-2s) | Partial | T019, T059, T060 | "Key actions" undefined |
| FR-014 (post-cutover-validation) | Partial | T062, T067 | No dedicated parity validation procedure |
| FR-015 (canonical-identity) | Yes | T012, T013 | |
| FR-016 (remove-legacy) | Yes | T063-T066 | |

## Constitution Alignment Issues

| Principle | Status | Details |
|-----------|--------|---------|
| § I Response Wrapper Consistency | OK | T011 description mentions response wrappers. Verify at implementation. |
| § II DI Discipline | OK | T016 registers services in Program.cs. |
| § III EF Core Context Stewardship | OK | Services route through repository layer. |
| § IV Admin UI Standardization | OK | T032-T034 implement shared partials following Bootstrap 5 patterns. |
| § V Documentation Governance | **VIOLATION** | T004 and T031 create `.md` files in project folders. |
| XML Documentation | Deferred Risk | T058 in Phase 7 — should validate earlier to prevent accumulation. |

## Unmapped Tasks

All tasks map to at least one requirement or user story. No orphan tasks detected.

## Metrics

| Metric | Value |
|--------|-------|
| Total Requirements | 16 |
| Total Tasks | 67 |
| Coverage % (requirements with ≥1 task) | 100% (16/16), but 6 are partial |
| Full Coverage % | 62.5% (10/16) |
| Ambiguity Count | 3 |
| Duplication Count | 2 |
| Critical Issues Count | 3 |
| High Issues Count | 4 |
| Medium Issues Count | 6 |
| Low Issues Count | 3 |
| Total Findings | 16 |

## Next Actions

### Before `/devspark.implement` (blocking):

1. **Resolve CRITICAL D1 & D2**: Move documentation targets for T004 and T031 under `.documentation/` to comply with Constitution § V.
2. **Resolve CRITICAL E1**: Add a new foundational task to scaffold `InquirySpark.Web` project (csproj, Program.cs, Area registration) before any tasks reference that project.
3. **Resolve HIGH E2**: Add role mapping data model and migration tasks to Phase 2.
4. **Resolve HIGH E3**: Add code-level rollback mechanism task to Phase 5.
5. **Resolve HIGH B1**: Define the explicit list of "key user actions" for performance targets.

### Recommended but non-blocking:

6. Define automated test tasks per user story checkpoint (E4).
7. Standardize "migration" vs "capability completion" terminology (F1).
8. Clarify "greenfield" scope in spec clarifications (F2).
9. Add post-cutover parity validation task (E5).
10. Add capability discovery/enumeration task before T002 (E6).

### Commands:

- Run `/devspark.specify` to refine spec for B1 (key actions), B2 (baseline measurement), and F2 (greenfield scope).
- Run `/devspark.tasks` to regenerate tasks after addressing D1, D2, E1, E2, E3 in the spec and plan.
- Manually edit `tasks.md` to add project scaffold task (E1) and role mapping tasks (E2) if regeneration is not desired.

---

Would you like me to suggest concrete remediation edits for the top N issues?
