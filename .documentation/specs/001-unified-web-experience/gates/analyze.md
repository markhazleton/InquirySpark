---
gate: analyze
status: warn
blocking: false
severity: warning
summary: "Cross-artifact alignment is mostly strong with full requirement coverage, but there are high-impact consistency gaps around greenfield positioning, decommission execution depth, and measurable performance validation tasks."
---

## Specification Analysis Report

| ID | Category | Severity | Location(s) | Summary | Recommendation |
|----|----------|----------|-------------|---------|----------------|
| I1 | Inconsistency | HIGH | [.documentation/specs/001-unified-web-experience/spec.md](../spec.md#L23), [.documentation/specs/001-unified-web-experience/tasks.md](../tasks.md#L77), [.documentation/specs/001-unified-web-experience/tasks.md](../tasks.md#L78) | Spec clarifies greenfield-only development, but task wording still frames feature shells as "migrated" from legacy apps. | Rename T027/T028 wording to greenfield capability implementation language to match spec intent. |
| I2 | Inconsistency | HIGH | [.documentation/specs/001-unified-web-experience/spec.md](../spec.md#L129), [.documentation/specs/001-unified-web-experience/tasks.md](../tasks.md#L157), [.documentation/specs/001-unified-web-experience/tasks.md](../tasks.md#L158), [.documentation/specs/001-unified-web-experience/tasks.md](../tasks.md#L159) | FR-016 requires removal from active runtime deployment, but tasks only mention solution config and docs updates, not runtime/deployment manifest updates. | Add explicit tasks for deployment pipeline/runtime host decommission and post-deploy verification evidence. |
| U1 | Underspecification | MEDIUM | [.documentation/specs/001-unified-web-experience/spec.md](../spec.md#L126), [.documentation/specs/001-unified-web-experience/tasks.md](../tasks.md#L155), [.documentation/specs/001-unified-web-experience/tasks.md](../tasks.md#L156) | Performance target (95% <=2s) exists, but tasks do not explicitly define measurement instrumentation or benchmark method. | Add dedicated performance validation task with metric collection method and pass/fail evidence output path. |
| C1 | Coverage Gap | MEDIUM | [.documentation/specs/001-unified-web-experience/spec.md](../spec.md#L125), [.documentation/specs/001-unified-web-experience/tasks.md](../tasks.md#L121), [.documentation/specs/001-unified-web-experience/tasks.md](../tasks.md#L159) | Stakeholder communication requirement is only partially covered by runbook/docs tasks; no explicit stakeholder comm artifact task is named. | Add a task for stakeholder migration/cutover communication pack under `.documentation/specs/001-unified-web-experience/contracts/`. |
| T1 | Terminology | LOW | [.documentation/specs/001-unified-web-experience/spec.md](../spec.md#L23), [.documentation/specs/001-unified-web-experience/plan.md](../plan.md#L8), [.documentation/specs/001-unified-web-experience/tasks.md](../tasks.md#L107) | Mixed use of "migration", "completion", and "decommission" introduces interpretation drift. | Normalize terms in all three artifacts: use "capability completion" for build-out, "decommission" for final removal. |

**Coverage Summary Table:**

| Requirement Key | Has Task? | Task IDs | Notes |
|-----------------|-----------|----------|-------|
| unified-functional-coverage | Yes | T002, T040, T041, T042, T043 | Capability inventory + parity workflows cover full feature matrix tracking. |
| single-entrypoint-single-session | Yes | T019, T023, T024, T029 | Unified workspace and navigation/session behavior are represented. |
| single-navigation-model | Yes | T022, T023, T024, T035 | Shared navigation model and terminology alignment tasks exist. |
| role-permission-semantics | Yes | T012, T013, T041, T052 | Identity bridge and parity/access validation tasks cover role mapping checks. |
| capability-domain-phasing | Yes | T003, T037, T042, T044 | Migration phase model and status workflows are covered. |
| rollback-safe-cutover | Yes | T043, T044, T045, T053 | Cutover and incident-response artifacts cover rollback governance. |
| operational-auditability | Yes | T046, T047, T048, T049 | Dedicated audit model/service/tasks present. |
| canonical-routes-no-legacy-compat | Yes | T001, T026, T055 | Route policy is represented; naming consistency issue remains (I1). |
| consistent-ux-patterns | Yes | T030, T031, T032, T033, T036 | Shared UX components and checklist included. |
| capability-matrix-parity-status | Yes | T002, T037, T040 | Matrix and query workflows covered. |
| objective-completion-cutover-criteria | Yes | T044, T045, T052 | Policy and checklists defined; may need stronger objective metric details. |
| stakeholder-communication-artifacts | Partial | T045, T159 | Runbook/docs touchpoints exist; explicit stakeholder comm pack missing (C1). |
| performance-95p-under-2s | Partial | T057, T058 | Validation exists, but no explicit instrumentation/measurement task (U1). |
| post-cutover-parity-validation | Yes | T041, T052, T057 | Parity and readiness checks are represented. |
| canonical-identity-with-bridge | Yes | T012, T013, T041 | Identity bridge and parity checks covered. |
| remove-legacy-runtime-deployment | Partial | T059, T060, T061 | Needs explicit runtime/deployment pipeline decommission tasks (I2). |

**Constitution Alignment Issues:**

- None detected as CRITICAL. Current artifacts remain aligned with constitution mandates on documentation location, DI/EF conventions, and UI standardization direction.

**Unmapped Tasks:**

- None significant. All tasks map to at least one requirement/story theme.

**Metrics:**

- Total Requirements: 16
- Total Tasks: 61
- Coverage % (requirements with >=1 task): 100%
- Ambiguity Count: 0
- Duplication Count: 0
- Critical Issues Count: 0

## Next Actions

- Since there are no CRITICAL issues, implementation can proceed, but HIGH/MEDIUM issues should be addressed first to reduce rework.
- Recommended pre-implement adjustments:
  - Add deployment/runtime decommission tasks for FR-016.
  - Add explicit performance measurement task for FR-013.
  - Add explicit stakeholder communication artifact task for FR-012.
  - Normalize "migration" vs "completion" terminology in tasks.

Suggested commands:
1. Update `.documentation/specs/001-unified-web-experience/tasks.md` manually for the above deltas.
2. Re-run `/devspark.analyze` after task refinements.
3. Continue with `/devspark.implement` once analysis status is acceptable.

Would you like me to suggest concrete remediation edits for the top 4 issues?
