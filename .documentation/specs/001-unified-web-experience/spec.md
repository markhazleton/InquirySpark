---
classification: full-spec
risk_level: high
target_workflow: specify-full
required_artifacts: spec, plan, tasks
recommended_next_step: plan
required_gates: checklist, analyze, critic
---

# Feature Specification: Unified InquirySpark Web Experience

**Feature Branch**: `001-unified-web-experience`  
**Created**: 2026-04-12  
**Last Reviewed**: 2026-04-14  
**Status**: Complete <!-- Valid: Draft | In Progress | Complete -->  
**Progress**: 66/66 tasks complete. Runtime validation checklists (ux-consistency, operational-readiness) require a running production instance and are tracked in decommission-verification-evidence.md.  
**Input**: User description: "we now have two overlapping amin applications \"DecisionSpark\" and \"InquirySpark.Admin\" review both and create a plan to create a new application that merges both into a single unified application called it \"InquirySpark.Web\" it will have all the functionality of both, but in a unified single experience"

## Clarifications

### Session 2026-04-12

- Q: What parity standard applies to the unified app scope? → A: Full feature parity across BOTH DecisionSpark and InquirySpark.Admin; no discovered user-facing capability is omitted without explicit decommission approval.
- Q: What is the migration rollout unit for cutover gates? → A: Capability-domain phased cutover.
- Q: What identity model should InquirySpark.Web use during consolidation? → A: Reuse the existing InquirySpark.Admin authentication and sign-in implementation over `ControlSparkUserContextConnection` as the canonical identity baseline, with migration bridge support only where phased account transition requires it.
- Q: How should legacy URLs/bookmarks be handled post-cutover? → None, this is greenfield-only development.
- Q: What user-facing performance target should define migration success? → A: 95% of key actions complete in <=2 seconds.

## User Scenarios & Testing *(mandatory)*

<!--
  IMPORTANT: User stories should be PRIORITIZED as user journeys ordered by importance.
  Each user story/journey must be INDEPENDENTLY TESTABLE - meaning if you implement just ONE of them,
  you should still have a viable MVP (Minimum Viable Product) that delivers value.
  
  Assign priorities (P1, P2, P3, etc.) to each story, where P1 is the most critical.
  Think of each story as a standalone slice of functionality that can be:
  - Developed independently
  - Tested independently
  - Deployed independently
  - Demonstrated to users independently
-->

### User Story 1 - Unified Operations Workspace (Priority: P1)

An operations user can access all capabilities currently split across DecisionSpark and InquirySpark.Admin from one web application experience, without switching applications or re-authenticating in a separate portal.

**Why this priority**: This is the core business outcome: remove workflow fragmentation and reduce context-switching overhead.

**Independent Test**: A user can log in once to InquirySpark.Web, navigate to features previously exclusive to each legacy app, and complete at least one end-to-end task from each domain in one session.

**Acceptance Scenarios**:

1. **Given** a user with required permissions, **When** they sign in to InquirySpark.Web, **Then** they can access both legacy capability sets from a unified navigation model.
2. **Given** a user performing work across both legacy domains, **When** they switch between feature areas, **Then** session context and identity remain intact.
3. **Given** a user starts a workflow in one feature area, **When** they continue into a dependent workflow from the other legacy area, **Then** the handoff occurs without requiring a separate app launch.

---

### User Story 2 - Consistent User Experience and Navigation (Priority: P2)

A business user gets a consistent interaction model (navigation, page layout, terminology, and actions) across all merged functionality so training and support burden is reduced.

**Why this priority**: Once functionality is unified, inconsistent UX would still produce operational friction and higher support costs.

**Independent Test**: Evaluate representative pages from both legacy capability sets in InquirySpark.Web and confirm they follow shared navigation, terminology, and interaction conventions.

**Acceptance Scenarios**:

1. **Given** a user moves between migrated feature areas, **When** they use primary actions (view, create, edit, delete, run), **Then** controls and page behaviors are consistent.
2. **Given** support staff trains a new user, **When** they demonstrate common tasks, **Then** one shared navigation and terminology model is sufficient.

---

### User Story 3 - Controlled Capability Completion and Cutover (Priority: P3)

An administrator can execute phased feature completion toward InquirySpark.Web as the only web application, with clear visibility of progress and final decommissioning of DecisionSpark and InquirySpark.Admin.

**Why this priority**: Operational safety and service continuity depend on a controlled migration path rather than a risky big-bang replacement.

**Independent Test**: Validate each capability domain in InquirySpark.Web, then confirm final decommission checklist can be executed to remove DecisionSpark and InquirySpark.Admin from active runtime use.

**Acceptance Scenarios**:

1. **Given** capability completion phase controls are defined, **When** a phase is activated, **Then** only approved capability areas are exposed in InquirySpark.Web.
2. **Given** completion validation checks pass, **When** final cutover is approved, **Then** DecisionSpark and InquirySpark.Admin are removed from active runtime use.

---

### User Story 4 - Governance, Auditability, and Operational Readiness (Priority: P4)

Platform owners can prove that the unified app preserves compliance, operational controls, and traceability expected from the two legacy applications.

**Why this priority**: Consolidation must not reduce governance quality or production support readiness.

**Independent Test**: Execute an operational readiness review showing required audit trails, access controls, and support runbooks are complete for InquirySpark.Web.

**Acceptance Scenarios**:

1. **Given** unified application operations, **When** audit and support evidence is reviewed, **Then** required records and runbooks are complete and current.
2. **Given** a production incident scenario, **When** support teams execute standard procedures, **Then** they can diagnose and recover without relying on legacy app pathways.

---

### Edge Cases

- What happens when a capability exists in one legacy app with no direct equivalent in the other? The unified app must surface it explicitly without hidden regressions.
- What happens when user permissions differ across the two legacy applications? Access rules must resolve deterministically and not escalate privileges.
- What happens when old route structures or deep links are referenced? The unified app must expose only new canonical InquirySpark.Web routes and documented navigation paths.
- What happens if partial capability completion causes inconsistent data views between legacy and unified experiences? Users must see authoritative data with clear conflict handling.
- What happens during rollback after a phased cutover issue? Legacy access must be restorable without data loss or inconsistent state.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: InquirySpark.Web MUST provide full feature parity for all user-facing capabilities currently delivered by BOTH DecisionSpark and InquirySpark.Admin without loss of functional coverage. The authoritative capability inventory created from both legacy applications defines the required parity scope until decommission is complete.
- **FR-002**: Users MUST be able to access all merged capabilities through one unified entry point and one authenticated session backed by the existing InquirySpark.Admin authentication and sign-in implementation over the `ControlSparkUserContextConnection` Identity SQLite store (canonical identity authority).
- **FR-003**: The unified experience MUST provide a single navigation model that exposes former DecisionSpark and InquirySpark.Admin features in a coherent information architecture.
- **FR-004**: The system MUST preserve current role and permission semantics for both legacy capability sets during and after migration, including role mapping across identity migration.
- **FR-005**: The solution MUST support phased capability completion by domain so capability groups can be introduced and validated incrementally. FR-010 defines the tracking artifact (capability matrix) that records per-domain completion status for this phased process.
- **FR-006**: The system MUST support rollback-safe cutover controls to prevent irreversible production disruption.
- **FR-007**: The unified application MUST preserve existing operational auditability, including user actions and administrative events needed for support and compliance. Required audit event types include: user authentication/authorization events, CRUD operations on surveys/questions/decisions, capability completion state changes, cutover decisions, role/permission changes, and administrative configuration changes.
- **FR-008**: InquirySpark.Web MUST define canonical routes for all unified workflows and MUST NOT depend on legacy route compatibility behavior.
- **FR-009**: The unified application MUST present consistent UX patterns (page layout, action placement, and terminology) across all migrated capability areas.
- **FR-010**: The solution MUST define and publish a capability-by-capability completion matrix showing parity status: not started, in progress, validated, and cut over. This matrix is the implementation artifact for the phased completion process defined in FR-005.
- **FR-011**: The solution MUST define objective completion and pre-cutover gate criteria per capability domain and require those criteria to pass before finalizing as the only web application. FR-014 defines the post-cutover runtime validation procedures that confirm parity after cutover is executed.
- **FR-012**: The solution MUST provide stakeholder-facing communication artifacts for capability completion phases, cutover windows, decommission timing, and user impact.
- **FR-013**: The unified experience MUST maintain current service-level expectations for availability and response behavior during phased capability completion, including 95% of the following key user actions completing in 2 seconds or less: (1) page load for any primary navigation destination, (2) list/search results display for surveys, questions, decisions, and applications, (3) single-record create/edit/save round-trip, (4) cross-domain navigation switch, (5) login and session establishment.
- **FR-014**: The solution MUST provide post-cutover runtime validation procedures that confirm functional parity, permissions, and critical workflow integrity after each domain cutover is executed. This complements pre-cutover gate criteria defined in FR-011.
- **FR-015**: The consolidation MUST use a single canonical identity store mapped explicitly to the existing `ControlSparkUserContextConnection` (Identity SQLite) and MUST reuse the InquirySpark.Admin authentication/sign-in implementation pattern (`AddDefaultIdentity`, role support, and Identity UI endpoints) as the baseline for InquirySpark.Web. Any completion bridge supports phased user/account transition only and MUST NOT introduce a parallel long-term sign-in model.
- **FR-016**: Once InquirySpark.Web completion criteria are met, DecisionSpark and InquirySpark.Admin MUST be removed from active runtime deployment.

### Key Entities *(include if feature involves data)*

*CRITICAL CONSTRAINT: No new database objects, schemas, or EF Core entities will be created. The existing `InquirySpark.Repository` will be used as-is. All tracking for migration, capabilities, and parity will exist as configuration models (e.g. `appsettings.json`) or in-memory state. DecisionSpark uses file storage rather than a database; it does not use SQLite or any other database engine. Its file-based data mechanisms will be integrated into the unified application securely, adhering to the constitution's mandate that SQLite remains the exclusive database provider for the solution. Audit tracking will be routed completely through standard `ILogger` logging pipelines.*

- **Capability Inventory Item**: Represents one functional area from DecisionSpark or InquirySpark.Admin with attributes for owner, parity status, dependencies, and migration phase (Configuration Model).
- **Unified Navigation Node**: Represents a navigable destination in InquirySpark.Web mapped to one or more legacy entry points (Configuration Model).
- **Migration Phase**: Represents a controlled rollout stage including scope, eligibility criteria, readiness checks, and rollback conditions (Configuration Model).
- **Parity Validation Record**: Represents evidence that a migrated capability meets functional, permission, and UX expectations (Configuration Model).
- **Cutover Decision Record**: Represents go/no-go outcomes for retiring legacy application entry points (Configuration Model).

### Assumptions

- "Greenfield" means new user-facing UX built in InquirySpark.Web that consumes existing shared services in `InquirySpark.Common` and `InquirySpark.Repository`. It does not mean rewriting backend domain logic. Legacy controller/view code in DecisionSpark and InquirySpark.Admin serves as a functional reference and parity baseline, not as code to be ported verbatim.
- New development occurs in InquirySpark.Web and does not rely on legacy runtime behavior.
- Current user roles and access policies are the baseline authority and must be preserved unless explicitly re-approved.
- Consolidation target is a single end-user experience under InquirySpark.Web, even if internal migration occurs in phases.
- Existing business capabilities from BOTH current admin applications are implemented natively in InquirySpark.Web and remain in scope until parity evidence marks them complete.
- The target-state authentication model reuses the existing InquirySpark.Admin authentication/sign-in stack over one canonical identity authority, with temporary migration bridging only during transition.
- **Terminology convention**: "Capability completion" refers to the process of building features in InquirySpark.Web. "Migration" refers to technical data/identity transitions. "Decommission" refers to final removal of legacy apps from active deployment.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of the capabilities inventoried from BOTH DecisionSpark and InquirySpark.Admin are implemented in InquirySpark.Web and marked complete in the capability matrix.
- **SC-002**: At least 95% of validated unified workflows complete successfully in InquirySpark.Web during pre-release validation.
- **SC-003**: Users perform cross-domain tasks in InquirySpark.Web without opening a second application in 100% of scenarios enumerated in the capability parity traceability matrix (`contracts/capability-parity-traceability.md`).
- **SC-004**: Post-cutover, zero user-reported incidents are attributed to app-switching confusion within 30 days of final decommission.
- **SC-005**: Completion and cutover readiness checks (functional parity, access parity, operational readiness) achieve 100% pass rate before final decommissioning of legacy applications.
- **SC-006**: Within 30 days after cutover, no Sev-1 incidents are attributed to missing migrated functionality.
- **SC-007**: During migration validation and after cutover, 95% of key user actions (as enumerated in FR-013) complete in 2 seconds or less.
