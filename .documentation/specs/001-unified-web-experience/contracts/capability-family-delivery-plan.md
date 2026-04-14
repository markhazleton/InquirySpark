# Capability Family Delivery Plan

**Feature**: `001-unified-web-experience`
**Created**: 2026-04-12
**Task**: T002B
**Source**: `contracts/capability-inventory-seed.md` (T002), `contracts/capability-parity-traceability.md` (T002A)

---

## Purpose

Groups all 37 inventoried capabilities into delivery families aligned to legacy source application surfaces. Each family maps to a Phase 3 implementation task and a unified controller. Families are sequenced by priority within Phase 3.

---

## Delivery Families

### Family 1: Decision Conversations (DS)
**Phase 3 Task**: T028
**Unified Controller**: `DecisionConversationController`
**Route Prefix**: `/Unified/DecisionConversation/`
**Capabilities**: CAP-DS-001, CAP-IA-030
**Priority**: P1 (core workflow — required for MVP)
**Dependencies**: `IDecisionSparkFileStorageService` (T004C/T004D), `ISessionStore`, `IDecisionSpecLoader`, `IRoutingEvaluator`, `IResponseMapper`, `ITraitParser`
**Legacy sources**: `DecisionSpark/Controllers/ConversationController.cs`, `InquirySpark.Admin/Controllers/Api/ConversationController.cs`

---

### Family 2: Decision Specification Management (DS)
**Phase 3 Task**: T029
**Unified Controller**: `DecisionSpecificationController`
**Route Prefix**: `/Unified/DecisionSpecification/`
**Capabilities**: CAP-DS-002, CAP-DS-003, CAP-DS-004, CAP-DS-005, CAP-DS-006
**Priority**: P1
**Dependencies**: `IDecisionSparkFileStorageService` (T004C/T004D), `DecisionSpecDraftService`
**Legacy sources**: `DecisionSpark/Areas/Admin/Controllers/DecisionSpecsController.cs`, `DecisionSpark/Controllers/DecisionSpecsApiController.cs`
**Note**: The REST API surface (CAP-DS-003) is re-implemented as an MVC controller with Identity auth — the API-key middleware is not carried forward.

---

### Family 3: Inquiry Administration (IA)
**Phase 3 Task**: T029A
**Unified Controller**: `InquiryAdministrationController`
**Route Prefix**: `/Unified/InquiryAdministration/`
**Capabilities**: CAP-IA-001, CAP-IA-002, CAP-IA-003, CAP-IA-004, CAP-IA-005, CAP-IA-024, CAP-IA-018, CAP-IA-019, CAP-IA-020, CAP-IA-021, CAP-IA-022, CAP-IA-023
**Priority**: P1 (core admin — applications, users, roles required for MVP)
**Dependencies**: `InquirySparkContext`, `ControlSparkUserContextConnection` (Identity), `IIdentityMigrationBridgeService` (T013)
**Legacy sources**: `InquirySpark.Admin/Areas/Inquiry/Controllers/Applications*.cs`, `ApplicationUsers*.cs`, `Roles*.cs`, `Lu*.cs`

---

### Family 4: Inquiry Authoring (IA)
**Phase 3 Task**: T029B
**Unified Controller**: `InquiryAuthoringController`
**Route Prefix**: `/Unified/InquiryAuthoring/`
**Capabilities**: CAP-IA-006, CAP-IA-007, CAP-IA-008, CAP-IA-009, CAP-IA-010, CAP-IA-011
**Priority**: P1
**Dependencies**: `InquirySparkContext`
**Legacy sources**: `InquirySpark.Admin/Areas/Inquiry/Controllers/Surveys*.cs`, `Questions*.cs`, `QuestionGroup*.cs`, `QuestionAnswers*.cs`

---

### Family 5: Inquiry Operations (IA)
**Phase 3 Task**: T029C
**Unified Controller**: `InquiryOperationsController`
**Route Prefix**: `/Unified/InquiryOperations/`
**Capabilities**: CAP-IA-012, CAP-IA-013, CAP-IA-014, CAP-IA-015, CAP-IA-016, CAP-IA-017
**Priority**: P2 (operational management — not MVP blocking)
**Dependencies**: `InquirySparkContext`
**Legacy sources**: `InquirySpark.Admin/Areas/Inquiry/Controllers/Companies*.cs`, `ImportHistories*.cs`, `SurveyStatus*.cs`, `SurveyReviewStatus*.cs`, `SiteRoles*.cs`, `SiteAppMenus*.cs`

---

### Family 6: Operations Support / Analytics / Health (IA + DS)
**Phase 3 Task**: T029D
**Unified Controller**: `OperationsSupportController`
**Route Prefix**: `/Unified/OperationsSupport/`
**Capabilities**: CAP-DS-007, CAP-IA-025, CAP-IA-026, CAP-IA-027, CAP-IA-028, CAP-IA-029
**Priority**: P3
**Dependencies**: `InquirySparkContext`, `IDecisionSparkFileStorageService` (health check), `ILogger`
**Legacy sources**: `InquirySpark.Admin/Controllers/SystemHealthController.cs`, `ChartBuilderController.cs`, `Api/ChartDefinitionsController.cs`, `Api/UserPreferencesController.cs`, `DecisionSpark/Health/DecisionSpecsHealthCheck.cs`

---

## Delivery Sequence (Phase 3)

```
Phase 2 complete → Foundations ready
  ↓
T028 Family 1: Decision Conversations         [parallel after T027 routing map]
T029 Family 2: Decision Specifications        [parallel with T028]
T029A Family 3: Inquiry Administration        [parallel with T028/T029]
T029B Family 4: Inquiry Authoring             [parallel with T028/T029]
  ↓
T029C Family 5: Inquiry Operations            [after P1 families stable]
T029D Family 6: Operations Support            [after P1 families stable]
  ↓
T030  Single-session cross-domain validation
T030A Unit/integration tests
T030B Parity evidence recording
```

---

## MVP Gate

Phase 3 MVP is achieved when Families 1–4 are `validated` in the parity matrix and a user can complete cross-domain scenario XD-001 (Decision Conversation → Survey Authoring) in a single session.
