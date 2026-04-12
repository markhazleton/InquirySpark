# Implementation Plan: Unified InquirySpark Web Experience

**Branch**: `001-unified-web-experience` | **Date**: 2026-04-12 | **Spec**: [.documentation/specs/001-unified-web-experience/spec.md](.documentation/specs/001-unified-web-experience/spec.md)
**Input**: Feature specification from `.documentation/specs/001-unified-web-experience/spec.md`

## Summary

Consolidate overlapping admin capabilities from `DecisionSpark` and `InquirySpark.Admin` into a single unified application experience under `InquirySpark.Web` as new greenfield development, using canonical identity authority with completion bridging and completion-based cutover controls. The implementation prioritizes zero-loss functional parity, unified navigation/UX consistency, and final removal of legacy application runtime use.

## Technical Context

**Language/Version**: C# on .NET 10 (`net10.0`)  
**Primary Dependencies**: ASP.NET Core MVC/Razor, Entity Framework Core (SQLite provider), existing shared libraries in `InquirySpark.Common` and `InquirySpark.Repository`, existing UI stack (Bootstrap 5 + DataTables + Bootswatch pattern)  
**Client Framework/Library**: Server-rendered ASP.NET Core Razor views with Bootstrap 5, DataTables, and targeted TypeScript/JavaScript modules (no SPA framework baseline)  
**Storage**: Existing SQLite assets and current repository/context model. NO new database objects or EF models will be created. Migration orchestrations (capability matrix, cutover) are strictly managed via `appsettings.json` config structures. Audit events rely exclusively on the `ILogger` pipeline. DecisionSpark relies exclusively on file storage, not a database, and its file-based mechanisms will seamlessly deploy alongside the existing `InquirySpark.Repository` without schema conflicts, respecting the constitution's SQLite-only database constraint.  
**Testing**: `dotnet test` (MSTest projects), integration tests for cross-domain workflows and cutover/readiness paths  
**Target Platform**: ASP.NET Core web hosting (development on Windows; deployable on standard .NET-supported server environments)  
**Project Type**: Web application consolidation (single unified user experience over existing shared backend/services)  
**Performance Goals**: 95% of key user actions complete in <=2 seconds during validation and post-cutover  
**Constraints**: Capability-domain phased completion; rollback-safe cutover; preserve role semantics; canonical identity authority; documentation and artifacts remain under `.documentation/`  
**Scale/Scope**: Full functional parity across both current admin surfaces, including shared navigation, capability completion matrix, and final decommissioning of DecisionSpark and InquirySpark.Admin

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Pre-Design Gate Review:

1. Response wrapper consistency: PASS (plan maintains service-level response patterns through existing shared/repository conventions).
2. DI discipline: PASS (no direct service construction introduced by design intent).
3. EF context stewardship: PASS (existing `InquirySparkContext` remains source of truth; no ad-hoc persistence bypass proposed).
4. Admin UI standardization: PASS with migration requirement (unified UX must preserve Bootstrap/DataTables conventions where applicable).
5. Documentation governance: PASS (`plan.md`, `research.md`, `data-model.md`, `quickstart.md`, and contracts remain under `.documentation/specs/001-unified-web-experience/`).

Result: No blocking constitution violations detected for planning.

## Project Structure

### Documentation (this feature)

```text
.documentation/specs/001-unified-web-experience/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── unified-web-contracts.md
├── checklists/
│   └── requirements.md
└── tasks.md
```

### Source Code (repository root)

```text
InquirySpark.Web/
InquirySpark.Admin/
DecisionSpark/
InquirySpark.Common/
InquirySpark.Repository/
InquirySpark.Common.Tests/
```

**Structure Decision**: Use web-application consolidation structure centered on `InquirySpark.Web` as the unified experience layer, while extracting/reusing domain logic through `InquirySpark.Common` and `InquirySpark.Repository`. DecisionSpark and InquirySpark.Admin are source references during development and are removed from active runtime deployment when completion gates pass.

**Note**: `InquirySpark.Web` does not yet exist in the repository and must be scaffolded (csproj, Program.cs, Area registration, solution integration) as the first foundational task before any code tasks that reference it.

**Terminology Convention**: "Capability completion" = building features in InquirySpark.Web. "Migration" = technical data/identity transitions. "Decommission" = final removal of legacy apps. "Greenfield" = new UX layer over existing shared services (not a backend rewrite).

## Phase 0: Research Plan

Research objectives derived from spec ambiguity/risk profile:

1. Canonical identity completion-bridge strategy between overlapping admin surfaces.
2. Capability-domain completion slicing and parity verification strategy.
3. Canonical InquirySpark.Web route strategy with no dependency on legacy compatibility behavior.
4. Unified navigation and terminology governance pattern for merged workflows.
5. Operational readiness model: cutover gates, rollback procedure, and observability evidence.

## Phase 1: Design Plan

Design outputs to be produced in this command:

1. `research.md` with explicit decisions, rationale, and alternatives.
2. `data-model.md` documenting planning/control entities for migration orchestration and validation.
3. `contracts/unified-web-contracts.md` defining interface contracts for capability completion matrix, parity status, and cutover governance interactions.
4. `quickstart.md` describing how to validate phased migration and unified workflows in development.
5. Stakeholder communication pack and decommission execution evidence artifacts for cutover communication and runtime retirement.
5. Agent context update execution via `.devspark/scripts/powershell/update-agent-context.ps1 -AgentType copilot`.

## Post-Design Constitution Check

To be validated after Phase 1 artifacts:

1. No documentation generated outside `/.documentation/`: PASS.
2. Plan preserves existing architectural constraints (DI, EF, response wrappers): PASS.
3. UI standardization alignment explicitly captured in requirements and plan artifacts: PASS.

Result: Constitution gates remain satisfied after design planning artifacts.

## Complexity Tracking

No constitution exceptions requested at planning stage.
