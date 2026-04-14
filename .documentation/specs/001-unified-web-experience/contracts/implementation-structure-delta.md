# Implementation Structure Delta

**Spec:** `001-unified-web-experience`  
**Task:** T056  
**Status:** Evidence Document

This document captures the structural delta introduced by the unified web experience feature vs. the baseline state of `InquirySpark.Web`.

---

## New Projects / Assemblies

No new projects added. All work is additive within existing projects:

| Project | Change Type |
|---|---|
| `InquirySpark.Web` | Added: Areas/Unified/, Models/Unified/, Configuration/Unified/ |
| `InquirySpark.Repository` | Added: Services/UnifiedWeb/, Configuration/Unified/, Models/Navigation/ |
| `InquirySpark.Common` | Added: Models/UnifiedWeb/ (CapabilityItem, UnifiedAuditEventItem, etc.) |
| `InquirySpark.Common.Tests` | Added: UnifiedWeb/ test classes |

---

## New Files Inventory

### InquirySpark.Web — Added

**Areas/Unified/Controllers/**
- `DecisionWorkspaceController.cs` — 7 actions (conversation, specs, library, export, import, archive, audit)
- `InquiryAdministrationController.cs` — 6 actions (applications, companies, contacts, LU tables, users, roles)
- `InquiryAuthoringController.cs` — 5 actions (surveys, questions, groups, templates, library)
- `InquiryOperationsController.cs` — 8 actions (deployments, responses, report, export, notifications, assignments, scoring, analytics)
- `OperationsSupportController.cs` — 5 actions (health, chart builder, chart settings, user prefs, api docs)
- `CapabilityCompletionMatrixController.cs` — Index action (unified capability dashboard)
- `OperationalReadinessController.cs` — Index action (go/no-go domain readiness)

**Areas/Unified/ViewModels/Completion/**
- `CapabilityCompletionMatrixViewModel.cs` — CapabilityCompletionMatrixViewModel, CapabilityDomainGroup, CapabilityMatrixRow, CapabilityMatrixSummary, PhaseInfo

**Areas/Unified/Views/**
- `CapabilityCompletionMatrix/Index.cshtml`
- `OperationalReadiness/Index.cshtml`
- (All other Unified area views from US1 controllers)

**Configuration/Unified/**
- `CanonicalRoutePolicy.cs` — AreaName, UrlPrefix constants, IsUnifiedRoute helper
- `CapabilityRoutingMap.cs` — 30 CAP-* → route mappings
- `TerminologyMap.cs` — legacy → canonical label resolution
- `CutoverPolicyOptions.cs` — phased cutover configuration model

**Models/Unified/**
- `PageHeaderActionsModel.cs` — reusable page header model
- `DataTableCardModel.cs` — reusable DataTable card model

**Services/Navigation/**
- `UnifiedNavigationBuilder.cs` — 7-node unified nav tree

**Views/Shared/Unified/**
- `_PageHeaderActions.cshtml` — page header partial
- `_DataTableCard.cshtml` — DataTable card partial

### InquirySpark.Repository — Added

**Configuration/Unified/**
- `CanonicalRoutePolicy.cs` (test-accessible mirror)
- `CapabilityRoutingMap.cs` (test-accessible mirror)

**Models/Navigation/**
- `UnifiedNavigationNodeViewModel.cs` (test-accessible navigation model)

**Services/Navigation/**
- `UnifiedNavigationBuilder.cs` (test-accessible navigation builder)

**Services/UnifiedWeb/**
- `IUnifiedWebCapabilityService.cs` — capability governance contract
- `UnifiedWebCapabilityService.cs` — implementation (IOptions + in-memory state)
- `IUnifiedAuditService.cs` — audit pipeline contract
- `UnifiedAuditService.cs` — ILogger-only implementation
- `NullUnifiedAuditService.cs` — null-object (test helper)

### InquirySpark.Common — Added

**Models/UnifiedWeb/**
- `CapabilityItem.cs`, `CapabilityPhaseItem.cs`
- `CutoverDecisionRecordItem.cs`, `ParityValidationRecordItem.cs`
- `UnifiedAuditEventItem.cs`

### InquirySpark.Common.Tests — Added

- `UnifiedWeb/US1AuthenticationFlowTests.cs` — 12 tests
- `UnifiedWeb/US1NavigationTests.cs` — 19 tests
- `UnifiedWeb/US3CapabilityServiceTests.cs` — 13 tests
- `UnifiedWeb/US4AuditServiceTests.cs` — 9 tests

---

## No Files Removed (Additive Approach)

No legacy files in `InquirySpark.Web`, `InquirySpark.Admin`, or `DecisionSpark` were deleted. The unified web experience is entirely additive — legacy routes remain functional until cutover gates are passed (T063/T064).
