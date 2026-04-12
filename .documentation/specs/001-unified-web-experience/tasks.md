# Tasks: Unified InquirySpark Web Experience

**Input**: Design documents from `.documentation/specs/001-unified-web-experience/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/, quickstart.md

**Tests**: Explicit test-first tasks were not requested in the feature specification. Validation tasks are included in story checkpoints and polish.

**Organization**: Tasks are grouped by user story to enable independent implementation and validation.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependency on incomplete tasks)
- **[Story]**: User story label ([US1], [US2], [US3], [US4])
- Every task includes an exact file path

## Path Conventions

- Unified app target: `InquirySpark.Web/`
- Existing admin sources: `InquirySpark.Admin/`, `DecisionSpark/`
- Shared domain and services: `InquirySpark.Common/`, `InquirySpark.Repository/`
- Feature docs: `.documentation/specs/001-unified-web-experience/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish feature scaffolding, authoritative parity inventory, and planning consistency artifacts.

- [ ] T001 Record greenfield-only route policy and legacy-app removal criteria in `.documentation/specs/001-unified-web-experience/contracts/greenfield-route-policy.md`.
- [ ] T001A Enumerate capabilities from DecisionSpark and InquirySpark.Admin source code and record discovery results in `.documentation/specs/001-unified-web-experience/contracts/capability-discovery-results.md`.
- [ ] T002 Create authoritative capability inventory from BOTH legacy applications in `.documentation/specs/001-unified-web-experience/contracts/capability-inventory-seed.md`.
- [ ] T002A Create parity traceability matrix mapping every inventoried legacy capability to a target unified workflow, controller/view family, test asset, and cutover evidence artifact in `.documentation/specs/001-unified-web-experience/contracts/capability-parity-traceability.md`.
- [ ] T002B Create capability family delivery plan grouped by legacy surface areas in `.documentation/specs/001-unified-web-experience/contracts/capability-family-delivery-plan.md`.
- [ ] T001B [P] Document DecisionSpark file-storage access patterns — including file locations, data formats, read/write semantics, and security constraints — by reading `DecisionSpark/` source code and record findings in `.documentation/specs/001-unified-web-experience/contracts/decisionspark-file-storage-integration.md`. This document is the schema authority for T004C/T004D.
- [ ] T003 [P] Create capability completion phase seed document in `.documentation/specs/001-unified-web-experience/contracts/capability-completion-phase-seed.md`.
- [ ] T004 [P] Add unified web feature area description in `.documentation/specs/001-unified-web-experience/contracts/unified-area-description.md` describing area purpose and boundaries.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Build core capability-completion governance and shared integration foundations required by all stories.

**⚠️ CRITICAL**: No user-story implementation should begin until this phase is complete.

- [ ] T004A Scaffold `InquirySpark.Web` project: create `InquirySpark.Web/InquirySpark.Web.csproj`, `InquirySpark.Web/Program.cs`, register Unified Area, and add project to `InquirySpark.sln`. Do NOT run build verification yet — the npm pipeline (T004A1/T004A2) must exist first or the build will fail.
- [ ] T004A1 Initialize CDN-free `npm run build` pipeline in `InquirySpark.Web/package.json` referencing Bootstrap, DataTables, and WebSpark.Bootswatch dependencies.
- [ ] T004A2 Integrate `npm run build` automatic pre-build execution target into `InquirySpark.Web/InquirySpark.Web.csproj`. After this task completes, run `dotnet build InquirySpark.sln -warnaserror` to verify the full scaffold + npm pipeline produces a zero-warning build. Depends on: T004A, T004A1.
- [ ] T004B [P] Create RoleMappingItem configuration model in `InquirySpark.Common/Models/UnifiedWeb/RoleMappingItem.cs` for cross-app role/permission mapping per FR-004.
- [ ] T004C [P] Create DecisionSpark file-storage service contract in `InquirySpark.Repository/Services/UnifiedWeb/IDecisionSparkFileStorageService.cs` using patterns documented in T001B. Interface must expose typed read/write operations for each DecisionSpark data domain (conversations, decision specifications, etc.).
- [ ] T004D Implement `InquirySpark.Repository/Services/UnifiedWeb/DecisionSparkFileStorageService.cs`, adapting DecisionSpark's file-based data mechanisms for use in InquirySpark.Web per the integration contract in `contracts/decisionspark-file-storage-integration.md`. Depends on: T004C, T001B.
- [ ] T005 Create CapabilityDomain configuration model in `InquirySpark.Common/Models/UnifiedWeb/CapabilityDomainItem.cs` (No EF DB mapping).
- [ ] T006 [P] Create CapabilityItem configuration model in `InquirySpark.Common/Models/UnifiedWeb/CapabilityItem.cs`.
- [ ] T007 [P] Create CapabilityPhase configuration model in `InquirySpark.Common/Models/UnifiedWeb/CapabilityPhaseItem.cs` (renamed from MigrationPhaseItem per spec terminology: "capability completion" = building features; "migration" = data/identity transitions only).
- [ ] T008 [P] Create ParityValidationRecord configuration model in `InquirySpark.Common/Models/UnifiedWeb/ParityValidationRecordItem.cs`.
- [ ] T009 [P] Create CutoverDecisionRecord configuration model in `InquirySpark.Common/Models/UnifiedWeb/CutoverDecisionRecordItem.cs`.
- [ ] T010 Create unified capability-completion governance service contract in `InquirySpark.Repository/Services/UnifiedWeb/IUnifiedWebCapabilityService.cs`.
- [ ] T011 Implement baseline capability-completion service with response wrappers in `InquirySpark.Repository/Services/UnifiedWeb/UnifiedWebCapabilityService.cs` (Retrieves capability tracking state from IOptions/config, no DB queries).
- [ ] T012 [P] Create identity migration bridge service contract in `InquirySpark.Repository/Services/UnifiedWeb/IIdentityMigrationBridgeService.cs`.
- [ ] T013 Implement identity migration bridge service in `InquirySpark.Repository/Services/UnifiedWeb/IdentityMigrationBridgeService.cs`, explicitly wiring Canonical Identity semantics to the `ControlSparkUserContextConnection` identity infrastructure.
- [ ] T013A Implement role mapping and permission migration logic in `InquirySpark.Repository/Services/UnifiedWeb/IdentityMigrationBridgeService.cs` using RoleMappingItem model per FR-004. Depends on: T013.
- [ ] T014 [P] Create client UI framework options model in `InquirySpark.Web/Configuration/Unified/ClientUiOptions.cs`.
- [ ] T015 Implement unified client bootstrap module in `InquirySpark.Web/wwwroot/js/unified-app.js`.
- [ ] T016 Register unified services in `InquirySpark.Web/Program.cs`, explicitly wiring `ControlSparkUserContextConnection` (Identity SQLite), `InquirySparkContext` (SQLite Read/Only Data), and `IDecisionSparkFileStorageService` → `DecisionSparkFileStorageService`. Scope: data context and file-storage DI registrations only (auth/Identity wiring is T016A; authorization policies are T016B).
- [ ] T016A Reuse the existing `InquirySpark.Admin` authentication/sign-in configuration in `InquirySpark.Web/Program.cs`: add `AddDefaultIdentity`, role support, and Entity Framework identity stores. Scope: Identity/auth builder registrations only (data context wiring is T016; authorization policies are T016B). Depends on: T016.
- [ ] T016B Recreate the existing `InquirySpark.Admin` authorization policies and Identity UI endpoint mapping in `InquirySpark.Web/Program.cs` using controller/view routing plus Razor Pages for Identity UI only. Scope: `app.UseAuthorization()`, policy definitions, and Identity UI endpoint mapping only (all builder registrations are T016/T016A). Depends on: T016A.
- [ ] T017 Add unified app settings section for migration controls in `InquirySpark.Web/appsettings.json`.
- [ ] T018 Add development migration controls in `InquirySpark.Web/appsettings.Development.json`.
- [ ] T019 [P] Add performance instrumentation configuration for key user actions in `InquirySpark.Web/appsettings.Development.json`.
- [ ] T019A Create authentication and session continuity integration tests in `InquirySpark.Common.Tests/UnifiedWeb/US1AuthenticationFlowTests.cs` validating reuse of the InquirySpark.Admin sign-in experience.

**Checkpoint**: Unified capability-completion governance, reused Admin authentication/sign-in foundations, and client UI foundations are available for story implementation.

---

## Phase 3: User Story 1 - Unified Operations Workspace (Priority: P1) 🎯 MVP

**Goal**: Deliver one signed-in workspace in InquirySpark.Web that exposes the full inventoried capability set previously split across DecisionSpark and InquirySpark.Admin.

**Independent Test**: User signs in once using the reused InquirySpark.Admin authentication flow and completes representative workflows from every capability family defined in the parity traceability matrix without opening another app.

**Services available to all Phase 3 controllers**: `IDecisionSparkFileStorageService` (file-backed DecisionSpark data), `IIdentityMigrationBridgeService` (cross-identity context), `IUnifiedWebCapabilityService` (capability/cutover state). Inject as needed per domain.

### Implementation for User Story 1

- [ ] T020 [US1] Create unified operations home controller in `InquirySpark.Web/Areas/Unified/Controllers/OperationsController.cs`.
- [ ] T021 [P] [US1] Create unified operations home view model in `InquirySpark.Web/Areas/Unified/ViewModels/OperationsHomeViewModel.cs`.
- [ ] T022 [US1] Create unified operations home view in `InquirySpark.Web/Areas/Unified/Views/Operations/Index.cshtml`.
- [ ] T023 [P] [US1] Add shared navigation component model in `InquirySpark.Web/ViewModels/Navigation/UnifiedNavigationNodeViewModel.cs`.
- [ ] T024 [US1] Implement unified navigation builder service in `InquirySpark.Web/Services/Navigation/UnifiedNavigationBuilder.cs`.
- [ ] T025 [US1] Integrate unified navigation into layout in `InquirySpark.Web/Views/Shared/_Layout.cshtml`.
- [ ] T026 [US1] Add capability routing map for initial unified workflows in `InquirySpark.Web/Configuration/Unified/CapabilityRoutingMap.cs`.
- [ ] T027 [US1] Add canonical unified route policy in `InquirySpark.Web/Configuration/Unified/CanonicalRoutePolicy.cs`.
- [ ] T028 [US1] Implement DecisionSpark conversation workflow parity in `InquirySpark.Web/Areas/Unified/Controllers/DecisionConversationController.cs`. Inject `IDecisionSparkFileStorageService` (T004C/T004D) for all file-backed data access; inject `IIdentityMigrationBridgeService` where cross-identity context is needed. Depends on: T004D, T016.
- [ ] T029 [US1] Implement DecisionSpark decision-specification management parity in `InquirySpark.Web/Areas/Unified/Controllers/DecisionSpecificationController.cs`. Inject `IDecisionSparkFileStorageService` (T004C/T004D) for all file-backed data access. Depends on: T004D, T016.
- [ ] T029A [US1] Implement Inquiry administration capability-family parity for applications, users, roles, site roles, and site menus in `InquirySpark.Web/Areas/Unified/Controllers/InquiryAdministrationController.cs`.
- [ ] T029B [US1] Implement Inquiry authoring capability-family parity for surveys, questions, question groups, group members, answers, and email templates in `InquirySpark.Web/Areas/Unified/Controllers/InquiryAuthoringController.cs`.
- [ ] T029C [US1] Implement Inquiry operations capability-family parity for companies, import history, survey status, and review status workflows in `InquirySpark.Web/Areas/Unified/Controllers/InquiryOperationsController.cs`.
- [ ] T029D [US1] Implement charting, system health, conversation API parity, role management, and user preference capability-family parity in `InquirySpark.Web/Areas/Unified/Controllers/OperationsSupportController.cs`.
- [ ] T030 [US1] Validate single-session cross-domain workflow via quickstart steps in `.documentation/specs/001-unified-web-experience/quickstart.md`.
- [ ] T030A [US1] Create unit/integration tests for UnifiedNavigationBuilder, OperationsController, and capability-family routing coverage in `InquirySpark.Common.Tests/UnifiedWeb/US1NavigationTests.cs`.
- [ ] T030B [US1] Record parity completion evidence for every capability family delivered in Phase 3 in `.documentation/specs/001-unified-web-experience/contracts/us1-parity-evidence.md`.

**Checkpoint**: Unified workspace parity baseline is functional and independently demonstrable across all inventoried capability families.

---

## Phase 4: User Story 2 - Consistent User Experience and Navigation (Priority: P2)

**Goal**: Standardize visual and interaction patterns across merged capability areas.

**Independent Test**: Users perform common actions in migrated areas and observe consistent page structure, action placement, and terminology.

### Implementation for User Story 2

- [ ] T031 [P] [US2] Create unified UX conventions document for developers in `.documentation/specs/001-unified-web-experience/contracts/unified-ux-conventions.md`.
- [ ] T032 [US2] Implement shared page header/action partial in `InquirySpark.Web/Views/Shared/Unified/_PageHeaderActions.cshtml`.
- [ ] T033 [P] [US2] Implement shared table/action partial in `InquirySpark.Web/Views/Shared/Unified/_DataTableCard.cshtml`.
- [ ] T034 [US2] Apply unified action semantics to operations view in `InquirySpark.Web/Areas/Unified/Views/Operations/Index.cshtml`.
- [ ] T035 [US2] Add terminology normalization map in `InquirySpark.Web/Configuration/Unified/TerminologyMap.cs`.
- [ ] T036 [US2] Update unified navigation labels to canonical terminology in `InquirySpark.Web/Services/Navigation/UnifiedNavigationBuilder.cs`.
- [ ] T037 [US2] Add UX consistency validation checklist in `.documentation/specs/001-unified-web-experience/checklists/ux-consistency.md`.

**Checkpoint**: Unified areas present consistent UX patterns and language.

---

## Phase 5: User Story 3 - Controlled Capability Completion and Cutover (Priority: P3)

**Goal**: Provide phased capability-completion controls, parity tracking, and go/no-go governance for safe final decommissioning.

**Independent Test**: A domain can be moved through phase lifecycle with recorded parity evidence, readiness checks, and cutover decision outcome.

### Implementation for User Story 3

- [ ] T038 [US3] Create capability completion matrix controller in `InquirySpark.Web/Areas/Unified/Controllers/CapabilityCompletionMatrixController.cs`.
- [ ] T039 [P] [US3] Create capability completion matrix view model in `InquirySpark.Web/Areas/Unified/ViewModels/Completion/CapabilityCompletionMatrixViewModel.cs`.
- [ ] T040 [US3] Create capability completion matrix view in `InquirySpark.Web/Areas/Unified/Views/CapabilityCompletionMatrix/Index.cshtml`.
- [ ] T041 [US3] Implement capability inventory query path in `InquirySpark.Repository/Services/UnifiedWeb/UnifiedWebCapabilityService.cs`.
- [ ] T042 [US3] Implement parity validation submission path in `InquirySpark.Repository/Services/UnifiedWeb/UnifiedWebCapabilityService.cs`.
- [ ] T043 [US3] Implement capability completion phase status path in `InquirySpark.Repository/Services/UnifiedWeb/UnifiedWebCapabilityService.cs`.
- [ ] T044 [US3] Implement cutover decision recording path in `InquirySpark.Repository/Services/UnifiedWeb/UnifiedWebCapabilityService.cs`.
- [ ] T045 [US3] Add phased completion cutover policy configuration in `InquirySpark.Web/Configuration/Unified/CutoverPolicyOptions.cs`.
- [ ] T045A [US3] Define per-domain pre-cutover gate criteria — a testable pass/fail checklist for each capability domain — in `.documentation/specs/001-unified-web-experience/contracts/pre-cutover-gate-criteria.md` per FR-011. Criteria must cover: functional parity evidence, permission parity evidence, performance validation, zero-warning build, and post-cutover validation readiness. Depends on: T002A (parity traceability matrix). Must complete before T046.
- [ ] T046 [US3] Document cutover operation runbook in `.documentation/specs/001-unified-web-experience/contracts/cutover-runbook.md`. Reference `pre-cutover-gate-criteria.md` (T045A) as the gate-pass precondition for each runbook step. Include post-cutover monitoring directive: ops must track SC-004 and SC-006 incident metrics for 30 days after each domain cutover.
- [ ] T047 [US3] Create stakeholder communication pack for completion and decommission phases in `.documentation/specs/001-unified-web-experience/contracts/stakeholder-communication-pack.md`.
- [ ] T047A [US3] Implement code-level rollback guards in `InquirySpark.Repository/Services/UnifiedWeb/UnifiedWebCapabilityService.cs` to revert domain cutover status and re-enable legacy access per FR-006. Scope: cutover-status reversal and legacy-access re-enablement only (data-integrity verification is T047A1).
- [ ] T047A1 [US3] Create rollback data-integrity verification checklist covering: in-flight record state consistency after status reversal, DecisionSpark file-storage state validation, and configuration state coherence. Record checklist in `.documentation/specs/001-unified-web-experience/contracts/rollback-integrity-checklist.md`. Add covering integration test for rollback-with-staged-data scenario in `InquirySpark.Common.Tests/UnifiedWeb/US3CapabilityServiceTests.cs` per FR-006 ("restorable without data loss"). Depends on: T047A.
- [ ] T047B [US3] Create unit/integration tests for UnifiedWebCapabilityService (capability inventory, parity, cutover, rollback) in `InquirySpark.Common.Tests/UnifiedWeb/US3CapabilityServiceTests.cs`.

**Checkpoint**: Domain-level migration and cutover flow is operationally controlled and auditable.

---

## Phase 6: User Story 4 - Governance, Auditability, and Operational Readiness (Priority: P4)

**Goal**: Ensure compliance-grade traceability and support readiness for unified operations.

**Independent Test**: Operational review can verify required logs, evidence bundles, and incident-response runbooks without relying on legacy pathways.

### Implementation for User Story 4

- [ ] T047C [US4] Enumerate required audit event types from DecisionSpark and InquirySpark.Admin AND define required payload fields per event type (at minimum: EventType, UserId, Timestamp, CorrelationId, ResourceId, ActionDetails) in `.documentation/specs/001-unified-web-experience/contracts/audit-event-types.md` per FR-007. This document is the schema authority for T048 (UnifiedAuditEventItem model).
- [ ] T048 [US4] Create structured audit event payload model in `InquirySpark.Common/Models/UnifiedWeb/UnifiedAuditEventItem.cs`.
- [ ] T049 [US4] Add unified audit logging service contract in `InquirySpark.Repository/Services/UnifiedWeb/IUnifiedAuditService.cs`.
- [ ] T050 [US4] Implement unified audit logging service in `InquirySpark.Repository/Services/UnifiedWeb/UnifiedAuditService.cs` (Uses `ILogger` standard pipelines, NOT EF Database).
- [ ] T051 [US4] Integrate cutover and parity audit emission in `InquirySpark.Repository/Services/UnifiedWeb/UnifiedWebCapabilityService.cs`.
- [ ] T052 [US4] Create operational readiness dashboard controller in `InquirySpark.Web/Areas/Unified/Controllers/OperationalReadinessController.cs`.
- [ ] T053 [P] [US4] Create operational readiness view in `InquirySpark.Web/Areas/Unified/Views/OperationalReadiness/Index.cshtml`.
- [ ] T054 [US4] Add operational readiness checklist in `.documentation/specs/001-unified-web-experience/checklists/operational-readiness.md`.
- [ ] T055 [US4] Document incident-response path for unified app in `.documentation/specs/001-unified-web-experience/contracts/incident-response-runbook.md`.
- [ ] T055A [US4] Create unit/integration tests for UnifiedAuditService in `InquirySpark.Common.Tests/UnifiedWeb/US4AuditServiceTests.cs`.

**Checkpoint**: Governance, auditability, and operational support readiness are demonstrable in unified app context.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final cross-story hardening, documentation, and validation.

- [ ] T056 [P] Record implementation structure delta evidence in `.documentation/specs/001-unified-web-experience/contracts/implementation-structure-delta.md`.
- [ ] T057 Record route-policy and decommission-readiness evidence in `.documentation/specs/001-unified-web-experience/contracts/route-and-decommission-readiness.md`.
- [ ] T058 [P] Add XML documentation comments to new public contracts/models in `InquirySpark.Common/Models/UnifiedWeb/` and `InquirySpark.Repository/Services/UnifiedWeb/`.
- [ ] T058A [P] Add XML documentation comments to new public APIs in `InquirySpark.Web/Areas/Unified/Controllers/` and `InquirySpark.Web/Services/` per constitution requirement (XML doc comments required on all public APIs across all projects).
- [ ] T059 Add explicit performance measurement task definition and thresholds in `.documentation/specs/001-unified-web-experience/contracts/performance-validation-method.md`.
- [ ] T060 Execute performance measurement for key user actions and record pass/fail evidence in `.documentation/specs/001-unified-web-experience/contracts/performance-validation-evidence.md`.
- [ ] T061 Run quickstart end-to-end validation and record evidence in `.documentation/specs/001-unified-web-experience/quickstart.md`.
- [ ] T062 Run full build and tests (`dotnet build InquirySpark.sln -warnaserror`, `dotnet test`) and capture outcomes in `.documentation/specs/001-unified-web-experience/contracts/validation-evidence.md`.
- [ ] T062B Run `dotnet ef migrations list` across all projects in the solution and confirm zero new migrations were generated (spec constraint: no new database objects). Record pass/fail result in `.documentation/specs/001-unified-web-experience/contracts/validation-evidence.md`.
- [ ] T062A Execute post-cutover functional parity, **permission parity**, and critical workflow integrity validation: use capability matrix (T002A) as the functional checklist and role-mapping artifacts (T004B, T013A) as the expected-permissions authority. Record pass/fail evidence for each domain in `.documentation/specs/001-unified-web-experience/contracts/post-cutover-parity-evidence.md` per FR-014.
- [ ] T063 Remove `DecisionSpark` from active solution configuration in `InquirySpark.sln` after unified completion gates pass.
- [ ] T064 Remove `InquirySpark.Admin` from active solution configuration in `InquirySpark.sln` after unified completion gates pass.
- [ ] T065 Remove `DecisionSpark/` and `InquirySpark.Admin/` runtime deployment references in deployment/run documentation at `README.md`.
- [ ] T066 Remove `DecisionSpark` and `InquirySpark.Admin` from active deployment pipeline manifests in `.github/workflows/` and deployment configuration files.
- [ ] T067 Record post-deploy decommission verification evidence in `.documentation/specs/001-unified-web-experience/contracts/decommission-verification-evidence.md`.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies. T002A and T002B must complete before parity implementation work is considered ready.
- **Phase 2 (Foundational)**: Depends on Phase 1 and blocks all user stories.
- **Phase 3 (US1)**: Depends on Phase 2; this is MVP.
- **Phase 4 (US2)**: Depends on Phase 3 navigation base.
- **Phase 5 (US3)**: Depends on Phase 2 services and can run after US1.
- **Phase 6 (US4)**: Depends on Phase 5 cutover/parity flows.
- **Phase 7 (Polish)**: Depends on completion of desired story phases.

### User Story Dependencies

- **US1 (P1)**: Independent after foundational completion.
- **US2 (P2)**: Relies on US1 unified navigation baseline.
- **US3 (P3)**: Relies on foundational migration services; functionally independent from US2.
- **US4 (P4)**: Relies on US3 governance flow outputs.

### Parallel Opportunities

- Phase 1: T001A, T003, and T004 parallel after T001; T002A and T002B follow the inventory baseline.
- Phase 2: T004A blocks all; T004B, T006-T009 parallel; T012 and T014 parallel before implementations; T016A and T016B complete before US1 sign-in validation.
- US1: T021 and T023 parallel; T028, T029, T029A, T029B, T029C, and T029D parallel after routing map and parity traceability are established.
- US2: T031 and T033 parallel.
- US3: T039 parallel with T038 before service-wire tasks.
- US4: T053 parallel after controller shell exists.
- Polish: T056 and T058 parallel.

---

## Parallel Example: User Story 1

```text
Run in parallel after T019:
- T021 [US1] Create operations view model
- T023 [US1] Create unified navigation node view model

Run in parallel after T025:
- T028 [US1] Implement DecisionSpark conversation workflow parity
- T029 [US1] Implement DecisionSpark decision-specification management parity
- T029A [US1] Implement inquiry administration capability-family parity
- T029B [US1] Implement inquiry authoring capability-family parity
- T029C [US1] Implement inquiry operations capability-family parity
- T029D [US1] Implement operations support capability-family parity
```

---

## Implementation Strategy

### MVP First (User Story 1)

1. Complete Setup and Foundational phases.
2. Freeze the parity traceability matrix and capability-family delivery plan.
3. Complete US1 unified workspace tasks.
4. Validate single-session cross-domain workflow and reused Admin sign-in.
5. Demo MVP before broader migration controls.

### Incremental Delivery

1. Deliver US1 unified access and navigation.
2. Add US2 UX consistency hardening.
3. Add US3 migration matrix and cutover control.
4. Add US4 governance and readiness dashboards.
5. Finish with polish validation and evidence capture.

### Parallel Team Strategy

1. Team A: foundational services and identity/client infrastructure.
2. Team B: unified workspace UX and navigation.
3. Team C: migration matrix and governance controls.
4. Team D: audit/readiness and runbook evidence.

---

## Notes

- [P] tasks are file-isolated and dependency-safe.
- User-story labels preserve traceability from spec to implementation.
- Task list is designed for independent story validation, full parity traceability across BOTH legacy applications, and phased delivery.
- Complete legacy application decommission tasks (T063-T067) only after unified completion gates pass.
