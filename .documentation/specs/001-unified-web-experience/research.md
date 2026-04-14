# Research: Unified InquirySpark Web Experience

## Decision 1: Consolidation Strategy
- Decision: Use capability-domain phased completion in InquirySpark.Web with final decommissioning of DecisionSpark and InquirySpark.Admin at completion gates.
- Rationale: This reduces cutover risk, enables incremental validation, and preserves operational continuity.
- Alternatives considered:
  - Big-bang migration: rejected due to high rollback and regression risk.
  - Persona-based migration first: rejected because capability coupling is stronger than persona boundaries.

## Decision 2: Identity Authority Model
- Decision: Use one canonical identity authority with temporary migration bridging for user/account transition.
- Rationale: This supports a single sign-in experience while avoiding permanent dual-auth complexity.
- Alternatives considered:
  - Long-term dual identity stores: rejected due to drift and governance risk.
  - New third identity store: rejected due to migration complexity and unnecessary re-platforming scope.

## Decision 3: Navigation and Route Strategy
- Decision: Treat InquirySpark.Web as greenfield navigation architecture with canonical route design and no dependency on legacy route compatibility behavior.
- Rationale: New development focus and final single-app target favor clear canonical routes over legacy compatibility maintenance burden.
- Alternatives considered:
  - Permanent redirect for all legacy routes: rejected due to high long-term maintenance burden for a greenfield destination.
  - Partial compatibility mapping: rejected because it prolongs dual-route operational complexity.

## Decision 4: UX and Terminology Unification
- Decision: Establish a single navigation taxonomy and action language baseline for all migrated capability domains.
- Rationale: Reduced support/training burden depends on consistency, not just feature parity.
- Alternatives considered:
  - Preserve per-domain UX patterns: rejected because users would still experience split mental models.

## Decision 5: Cutover Governance and Readiness
- Decision: Define per-domain readiness gates covering parity, access mapping, operational readiness, and rollback validation before final decommissioning of DecisionSpark and InquirySpark.Admin.
- Rationale: Cutover quality must be objective and auditable.
- Alternatives considered:
  - Calendar-based cutover only: rejected because it does not enforce technical or operational readiness.

## Decision 6: Performance Validation Baseline
- Decision: Use 95% <=2s for key user actions as the primary user-facing performance acceptance target.
- Rationale: This is measurable, aligns with the specification success criteria, and is suitable for admin workflow responsiveness.
- Alternatives considered:
  - 95% <=1s: rejected as potentially unrealistic during phased migration.
  - No explicit target: rejected because it weakens acceptance testing.

## Decision 7: Client UI Framework Baseline
- Decision: Use server-rendered ASP.NET Core Razor views with Bootstrap 5 + DataTables and targeted TypeScript modules as the baseline client approach.
- Rationale: This aligns with current project standards, minimizes migration complexity, and preserves established admin interaction patterns.
- Alternatives considered:
  - React/Angular SPA baseline: rejected due to re-platforming overhead and increased implementation risk for consolidation scope.
  - Pure static HTML without shared JS modules: rejected due to weaker maintainability for complex admin workflows.

## Open Planning Note
- Completion requires explicit deployment/runbook tasks to remove DecisionSpark and InquirySpark.Admin from active runtime usage once unified acceptance gates pass.
