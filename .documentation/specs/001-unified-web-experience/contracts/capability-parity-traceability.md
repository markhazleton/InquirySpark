# Capability Parity Traceability Matrix

**Feature**: `001-unified-web-experience`
**Created**: 2026-04-12
**Task**: T002A
**Authority**: This document is the scenario enumeration authority referenced by SC-003 ("cross-domain tasks without opening a second application in 100% of scenarios enumerated here").
**Source**: `contracts/capability-inventory-seed.md` (T002)

---

## Purpose

Maps every inventoried legacy capability to its target unified implementation — controller/view family, test asset, and cutover evidence artifact. This matrix is the implementation checklist for FR-001 (full feature parity) and FR-010 (capability completion tracking).

---

## Status Legend

| Status | Meaning |
|---|---|
| `not-started` | No work begun |
| `in-progress` | Actively being implemented |
| `implemented` | Code complete, parity validation pending |
| `validated` | Parity evidence recorded and accepted |
| `cut-over` | Legacy source removed from active deployment |

---

## Parity Matrix

### DecisionSpark Domain

| Cap ID | Legacy Source | Legacy Route | Unified Controller | Unified Route | Parity Test | Evidence Artifact | Status |
|---|---|---|---|---|---|---|---|
| CAP-DS-001 | `ConversationController` | `/Conversation/*` | `DecisionConversationController` | `/Unified/DecisionConversation/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-DS-002 | `Admin/DecisionSpecsController` | `/Admin/DecisionSpecs/*` | `DecisionSpecificationController` | `/Unified/DecisionSpecification/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-DS-003 | `DecisionSpecsApiController` | `/api/decision-specs/*` | `DecisionSpecificationController` (MVC) | `/Unified/DecisionSpecification/*` (MVC, Identity auth) | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-DS-004 | `Admin/DecisionSpecsController` (_LlmAssistModal) | `/Admin/DecisionSpecs/Draft` | `DecisionSpecificationController` | `/Unified/DecisionSpecification/Draft` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-DS-005 | `DecisionSpecsApiController` (transition) | `/api/decision-specs/{id}/status` | `DecisionSpecificationController` | `/Unified/DecisionSpecification/{id}/Status` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-DS-006 | `DecisionSpecsApiController` (audit) | `/api/decision-specs/{id}/audit` | `DecisionSpecificationController` | `/Unified/DecisionSpecification/{id}/Audit` | `US4AuditServiceTests` | `post-cutover-parity-evidence.md` | not-started |
| CAP-DS-007 | `DecisionSpecsHealthCheck` | `/health` | `OperationsSupportController` | `/Unified/OperationsSupport/Health` | `US4AuditServiceTests` | `post-cutover-parity-evidence.md` | not-started |

### InquirySpark.Admin — Administration Domain

| Cap ID | Legacy Source | Legacy Route | Unified Controller | Unified Route | Parity Test | Evidence Artifact | Status |
|---|---|---|---|---|---|---|---|
| CAP-IA-001 | `ApplicationsController` | `/Inquiry/Applications/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/Applications/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-002 | `ApplicationUsersController` | `/Inquiry/ApplicationUsers/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/ApplicationUsers/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-003 | `ApplicationUserRolesController` | `/Inquiry/ApplicationUserRoles/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/ApplicationUserRoles/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-004 | `ApplicationSurveysController` | `/Inquiry/ApplicationSurveys/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/ApplicationSurveys/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-005 | `AppPropertiesController` | `/Inquiry/AppProperties/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/AppProperties/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-024 | `RolesController`, `RoleManagementController` | `/Inquiry/Roles/*`, `/RoleManagement/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/Roles/*` | `US1NavigationTests` | `post-cutover-parity-evidence.md` | not-started |

### InquirySpark.Admin — Authoring Domain

| Cap ID | Legacy Source | Legacy Route | Unified Controller | Unified Route | Parity Test | Evidence Artifact | Status |
|---|---|---|---|---|---|---|---|
| CAP-IA-006 | `SurveysController` | `/Inquiry/Surveys/*` | `InquiryAuthoringController` | `/Unified/InquiryAuthoring/Surveys/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-007 | `SurveyEmailTemplatesController` | `/Inquiry/SurveyEmailTemplates/*` | `InquiryAuthoringController` | `/Unified/InquiryAuthoring/SurveyEmailTemplates/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-008 | `QuestionsController` | `/Inquiry/Questions/*` | `InquiryAuthoringController` | `/Unified/InquiryAuthoring/Questions/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-009 | `QuestionGroupsController` | `/Inquiry/QuestionGroups/*` | `InquiryAuthoringController` | `/Unified/InquiryAuthoring/QuestionGroups/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-010 | `QuestionGroupMembersController` | `/Inquiry/QuestionGroupMembers/*` | `InquiryAuthoringController` | `/Unified/InquiryAuthoring/QuestionGroupMembers/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-011 | `QuestionAnswersController` | `/Inquiry/QuestionAnswers/*` | `InquiryAuthoringController` | `/Unified/InquiryAuthoring/QuestionAnswers/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |

### InquirySpark.Admin — Operations Domain

| Cap ID | Legacy Source | Legacy Route | Unified Controller | Unified Route | Parity Test | Evidence Artifact | Status |
|---|---|---|---|---|---|---|---|
| CAP-IA-012 | `CompaniesController` | `/Inquiry/Companies/*` | `InquiryOperationsController` | `/Unified/InquiryOperations/Companies/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-013 | `ImportHistoriesController` | `/Inquiry/ImportHistories/*` | `InquiryOperationsController` | `/Unified/InquiryOperations/ImportHistories/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-014 | `SurveyStatusController` | `/Inquiry/SurveyStatus/*` | `InquiryOperationsController` | `/Unified/InquiryOperations/SurveyStatus/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-015 | `SurveyReviewStatusController` | `/Inquiry/SurveyReviewStatus/*` | `InquiryOperationsController` | `/Unified/InquiryOperations/SurveyReviewStatus/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |

### InquirySpark.Admin — Site Configuration Domain

| Cap ID | Legacy Source | Legacy Route | Unified Controller | Unified Route | Parity Test | Evidence Artifact | Status |
|---|---|---|---|---|---|---|---|
| CAP-IA-016 | `SiteRolesController` | `/Inquiry/SiteRoles/*` | `InquiryOperationsController` | `/Unified/InquiryOperations/SiteRoles/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-017 | `SiteAppMenusController` | `/Inquiry/SiteAppMenus/*` | `InquiryOperationsController` | `/Unified/InquiryOperations/SiteAppMenus/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |

### InquirySpark.Admin — Lookup Domain

| Cap ID | Legacy Source | Legacy Route | Unified Controller | Unified Route | Parity Test | Evidence Artifact | Status |
|---|---|---|---|---|---|---|---|
| CAP-IA-018 | `LuApplicationTypesController` | `/Inquiry/LuApplicationTypes/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/LuApplicationTypes/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-019 | `LuQuestionTypesController` | `/Inquiry/LuQuestionTypes/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/LuQuestionTypes/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-020 | `LuReviewStatusController` | `/Inquiry/LuReviewStatus/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/LuReviewStatus/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-021 | `LuSurveyResponseStatusController` | `/Inquiry/LuSurveyResponseStatus/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/LuSurveyResponseStatus/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-022 | `LuSurveyTypesController` | `/Inquiry/LuSurveyTypes/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/LuSurveyTypes/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-023 | `LuUnitOfMeasuresController` | `/Inquiry/LuUnitOfMeasures/*` | `InquiryAdministrationController` | `/Unified/InquiryAdministration/LuUnitOfMeasures/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |

### InquirySpark.Admin — Analytics and Support Domain

| Cap ID | Legacy Source | Legacy Route | Unified Controller | Unified Route | Parity Test | Evidence Artifact | Status |
|---|---|---|---|---|---|---|---|
| CAP-IA-025 | `ChartSettingsController` | `/Inquiry/ChartSettings/*` | `OperationsSupportController` | `/Unified/OperationsSupport/ChartSettings/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-026 | `ChartBuilderController` | `/ChartBuilder/*` | `OperationsSupportController` | `/Unified/OperationsSupport/ChartBuilder/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-027 | `Api/ChartDefinitionsController` | `/api/ChartDefinitions/*` | `OperationsSupportController` | `/Unified/OperationsSupport/ChartDefinitions/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-028 | `SystemHealthController` | `/SystemHealth/*` | `OperationsSupportController` | `/Unified/OperationsSupport/SystemHealth/*` | `US4AuditServiceTests` | `post-cutover-parity-evidence.md` | not-started |
| CAP-IA-029 | `Api/UserPreferencesController` | `/api/UserPreferences/*` | `OperationsSupportController` | `/Unified/OperationsSupport/UserPreferences/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |
| CAP-IA-030 | `Api/ConversationController` | `/api/Conversation/*` | `DecisionConversationController` | `/Unified/DecisionConversation/Api/*` | `US1NavigationTests` | `us1-parity-evidence.md` | not-started |

---

## Cross-Domain Scenarios (SC-003 Authority)

The following are the scenarios referenced by SC-003. A user must be able to complete all of these without opening a second application:

| Scenario ID | Description | Capabilities Exercised |
|---|---|---|
| XD-001 | Sign in and navigate to a Decision Conversation, then navigate to Survey Authoring | CAP-DS-001, CAP-IA-006 |
| XD-002 | Create a Decision Specification, then assign a related Survey to an Application | CAP-DS-002, CAP-IA-004 |
| XD-003 | View Decision Spec audit history, then check System Health | CAP-DS-006, CAP-IA-028 |
| XD-004 | Manage Application Users and their roles, then start a Decision Conversation | CAP-IA-002, CAP-IA-003, CAP-DS-001 |
| XD-005 | Author a survey with questions and answers, then check import history | CAP-IA-006, CAP-IA-008, CAP-IA-011, CAP-IA-013 |
| XD-006 | Run LLM spec drafting, transition spec to Published, then view capability matrix | CAP-DS-004, CAP-DS-005, unified matrix |

---

## Completion Summary (updated as work progresses)

| Domain | Total Capabilities | Validated | % Complete |
|---|---|---|---|
| DecisionSpark | 7 | 0 | 0% |
| InquirySpark.Admin — Administration | 6 | 0 | 0% |
| InquirySpark.Admin — Authoring | 6 | 0 | 0% |
| InquirySpark.Admin — Operations | 4 | 0 | 0% |
| InquirySpark.Admin — Site Config | 2 | 0 | 0% |
| InquirySpark.Admin — Lookups | 6 | 0 | 0% |
| InquirySpark.Admin — Analytics/Support | 6 | 0 | 0% |
| **TOTAL** | **37** | **0** | **0%** |
