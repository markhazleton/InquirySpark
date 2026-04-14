# US1 Parity Evidence — Phase 3 Capability Delivery

**Spec:** `001-unified-web-experience`  
**User Story:** US1 — Single-session cross-domain capability access  
**Task:** T030B  
**Date Recorded:** 2025-01-26  
**Status:** DELIVERED ✅

---

## Summary

All 30 inventoried capabilities from DecisionSpark and InquirySpark.Admin are addressable via unified `/Unified/` routes in `InquirySpark.Web`. Every capability has:

1. A controller action in the Unified area
2. A route entry in `CapabilityRoutingMap`
3. A navigation node in `UnifiedNavigationBuilder`
4. Test coverage validating route registration and navigation structure

---

## Capability Coverage by Family

### DecisionSpark Domain (CAP-DS-001 thru CAP-DS-007)

| Capability ID | Description | Unified Route | Controller |
|---|---|---|---|
| CAP-DS-001 | Conversation list | `/Unified/DecisionConversation` | `DecisionConversationController.Index` |
| CAP-DS-002 | Spec list | `/Unified/DecisionSpecification` | `DecisionSpecificationController.Index` |
| CAP-DS-003 | Spec details | `/Unified/DecisionSpecification` | `DecisionSpecificationController.Details` |
| CAP-DS-004 | Draft spec | `/Unified/DecisionSpecification/Draft` | `DecisionSpecificationController.Create` |
| CAP-DS-005 | Transition status | `/Unified/DecisionSpecification/{id}/Status` | `DecisionSpecificationController.TransitionStatus` |
| CAP-DS-006 | Spec audit trail | `/Unified/DecisionSpecification/{id}/Audit` | `DecisionSpecificationController.Audit` |
| CAP-DS-007 | System health | `/Unified/OperationsSupport/Health` | `OperationsSupportController.Health` |

### Inquiry Administration Domain (CAP-IA-001 thru CAP-IA-005, CAP-IA-024)

| Capability ID | Description | Unified Route | Controller |
|---|---|---|---|
| CAP-IA-001 | Applications | `/Unified/InquiryAdministration/Applications` | `InquiryAdministrationController.Applications` |
| CAP-IA-002 | Application users | `/Unified/InquiryAdministration/ApplicationUsers` | `InquiryAdministrationController.ApplicationUsers` |
| CAP-IA-003 | User roles | `/Unified/InquiryAdministration/ApplicationUserRoles` | `InquiryAdministrationController.ApplicationUserRoles` |
| CAP-IA-004 | Application surveys | `/Unified/InquiryAdministration/ApplicationSurveys` | `InquiryAdministrationController.ApplicationSurveys` |
| CAP-IA-005 | App properties | `/Unified/InquiryAdministration/AppProperties` | `InquiryAdministrationController.AppProperties` |
| CAP-IA-024 | Roles | `/Unified/InquiryAdministration/Roles` | `InquiryAdministrationController.Roles` |

### Inquiry Authoring Domain (CAP-IA-006 thru CAP-IA-011)

| Capability ID | Description | Unified Route | Controller |
|---|---|---|---|
| CAP-IA-006 | Surveys | `/Unified/InquiryAuthoring/Surveys` | `InquiryAuthoringController.Surveys` |
| CAP-IA-007 | Email templates | `/Unified/InquiryAuthoring/SurveyEmailTemplates` | `InquiryAuthoringController.SurveyEmailTemplates` |
| CAP-IA-008 | Questions | `/Unified/InquiryAuthoring/Questions` | `InquiryAuthoringController.Questions` |
| CAP-IA-009 | Question groups | `/Unified/InquiryAuthoring/QuestionGroups` | `InquiryAuthoringController.QuestionGroups` |
| CAP-IA-010 | Group members | `/Unified/InquiryAuthoring/QuestionGroupMembers` | `InquiryAuthoringController.QuestionGroupMembers` |
| CAP-IA-011 | Answers | `/Unified/InquiryAuthoring/QuestionAnswers` | `InquiryAuthoringController.QuestionAnswers` |

### Inquiry Operations Domain (CAP-IA-012 thru CAP-IA-017)

| Capability ID | Description | Unified Route | Controller |
|---|---|---|---|
| CAP-IA-012 | Companies | `/Unified/InquiryOperations/Companies` | `InquiryOperationsController.Companies` |
| CAP-IA-013 | Import history | `/Unified/InquiryOperations/ImportHistories` | `InquiryOperationsController.ImportHistories` |
| CAP-IA-014 | Survey status | `/Unified/InquiryOperations/SurveyStatus` | `InquiryOperationsController.SurveyStatus` |
| CAP-IA-015 | Review status | `/Unified/InquiryOperations/SurveyReviewStatus` | `InquiryOperationsController.SurveyReviewStatus` |
| CAP-IA-016 | Site roles | `/Unified/InquiryOperations/SiteRoles` | `InquiryOperationsController.SiteRoles` |
| CAP-IA-017 | Site menus | `/Unified/InquiryOperations/SiteAppMenus` | `InquiryOperationsController.SiteAppMenus` |

### Administration Lookup Domain (CAP-IA-018 thru CAP-IA-023)

| Capability ID | Description | Unified Route | Controller |
|---|---|---|---|
| CAP-IA-018 | Lookup: Application types | `/Unified/InquiryAdministration/LuApplicationTypes` | `InquiryAdministrationController.LuApplicationTypes` |
| CAP-IA-019 | Lookup: Question types | `/Unified/InquiryAdministration/LuQuestionTypes` | `InquiryAdministrationController.LuQuestionTypes` |
| CAP-IA-020 | Lookup: Review status | `/Unified/InquiryAdministration/LuReviewStatus` | `InquiryAdministrationController.LuReviewStatus` |
| CAP-IA-021 | Lookup: Survey response status | `/Unified/InquiryAdministration/LuSurveyResponseStatus` | `InquiryAdministrationController.LuSurveyResponseStatus` |
| CAP-IA-022 | Lookup: Survey types | `/Unified/InquiryAdministration/LuSurveyTypes` | `InquiryAdministrationController.LuSurveyTypes` |
| CAP-IA-023 | Lookup: Unit of measures | `/Unified/InquiryAdministration/LuUnitOfMeasures` | `InquiryAdministrationController.LuUnitOfMeasures` |

### Operations Support Domain (CAP-IA-025 thru CAP-IA-030)

| Capability ID | Description | Unified Route | Controller |
|---|---|---|---|
| CAP-IA-025 | Chart settings | `/Unified/OperationsSupport/ChartSettings` | `OperationsSupportController.ChartSettings` |
| CAP-IA-026 | Chart builder | `/Unified/OperationsSupport/ChartBuilder` | `OperationsSupportController.ChartBuilder` |
| CAP-IA-027 | Chart definitions | `/Unified/OperationsSupport/ChartDefinitions` | `OperationsSupportController.ChartSettings` (catalog) |
| CAP-IA-028 | System health | `/Unified/OperationsSupport/SystemHealth` | `OperationsSupportController.Health` |
| CAP-IA-029 | User preferences | `/Unified/OperationsSupport/UserPreferences` | `OperationsSupportController.UserPreferences` |
| CAP-IA-030 | Conversation API | `/Unified/DecisionConversation/Api` | `DecisionConversationController.ApiSpecs` |

---

## Test Coverage

Tests verifying parity are in `InquirySpark.Common.Tests/UnifiedWeb/`:

| Test File | Tests | Coverage |
|---|---|---|
| `US1AuthenticationFlowTests.cs` | 12 | Identity bridge, role mapping, parity validation |
| `US1NavigationTests.cs` | 19 | Navigation tree structure, capability routing map, route policy |

**Total: 31 tests, 31 passed**

---

## Architecture Notes

- Navigation types (`UnifiedNavigationNodeViewModel`, `UnifiedNavigationBuilder`) are defined in **both** `InquirySpark.Repository` (for test access) and `InquirySpark.Web` (for Web Assembly). Both are kept in sync.
- `CapabilityRoutingMap` and `CanonicalRoutePolicy` are similarly dual-homed in Repository and Web layers.
- All unified routes are under `/Unified/` per `CanonicalRoutePolicy.UrlPrefix` — no legacy URL aliases exist.
- Authorization policies applied per controller: `Analyst`, `Operator`, `Consultant`, `Executive` (some capabilities allow anonymous, e.g., `/Health`).
