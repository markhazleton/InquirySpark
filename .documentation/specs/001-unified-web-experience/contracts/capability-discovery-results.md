# Capability Discovery Results

**Feature**: `001-unified-web-experience`
**Created**: 2026-04-12
**Task**: T001A — Enumerate capabilities from DecisionSpark and InquirySpark.Admin
**Feeds into**: T002 (capability-inventory-seed.md), T002A (capability-parity-traceability.md)

---

## Summary

| Source App | Capability Families | Total Controllers |
|---|---|---|
| DecisionSpark | 3 | 4 (3 MVC + 1 API) |
| InquirySpark.Admin | 9 | 35 (26 area + 9 non-area) |
| **Total** | **11** | **39** |

All capabilities listed below are **in scope** for InquirySpark.Web unless explicitly marked decommission-exempt.

---

## DecisionSpark Capabilities

### DS-F01 — Decision Conversations
- **Controller**: `DecisionSpark/Controllers/ConversationController.cs`
- **Route**: `/Conversation/...`
- **Description**: Guided question-answer sessions that collect user trait answers and route to a recommended decision outcome. Supports in-memory session state, OpenAI-powered question generation, and multi-step conversation flow.
- **Data storage**: File-based via `IConversationPersistence` / `FileConversationPersistence` (JSON per session under `ConversationStorage:Path`).
- **Dependencies**: `ISessionStore`, `IDecisionSpecLoader`, `IRoutingEvaluator`, `IResponseMapper`, `ITraitParser`, `IUserSelectionService`, `IQuestionGenerator`, `IOptionIdGenerator`, `IQuestionPresentationDecider`

### DS-F02 — Decision Specification Administration (UI)
- **Controller**: `DecisionSpark/Areas/Admin/Controllers/DecisionSpecsController.cs`
- **Route**: `/Admin/DecisionSpecs/...`
- **Actions**: Index (list), Create, Edit, Details, ViewJson, LLM Draft modal
- **Description**: Admin CRUD UI for managing DecisionSpec JSON documents. Includes an LLM-assisted draft workflow that generates a spec from an instruction prompt.
- **Data storage**: File-based via `IDecisionSpecRepository` (backed by `DecisionSpecFileStore` + `DecisionSpecMetadataStore`)
- **Dependencies**: `IDecisionSpecRepository`, `DecisionSpecDraftService`

### DS-F03 — Decision Specification API
- **Controller**: `DecisionSpark/Controllers/DecisionSpecsApiController.cs`
- **Route**: `/api/decision-specs/...` (REST, API-key protected)
- **Actions**: List, Get, Create, Update (full), Patch (trait), Delete, Restore, Status-Transition, LLM Draft initiate/retrieve, Audit log, Full JSON, Health
- **Description**: REST API surface for programmatic access to decision specifications with optimistic concurrency (ETag), soft-delete/restore, and LLM draft management.
- **Data storage**: File-based via `IDecisionSpecRepository`
- **Dependencies**: `IDecisionSpecRepository`, `DecisionSpecDraftService`, `IValidator<DecisionSpecDocument>`
- **Security**: `X-API-KEY` header authentication (non-Identity, custom middleware)

### DS-F04 — DecisionSpark Home/About
- **Controller**: `DecisionSpark/Controllers/HomeController.cs`
- **Route**: `/Home/Index`, `/Home/About`
- **Description**: Public landing and about pages. No data dependency.

---

## InquirySpark.Admin Capabilities

### IA-F01 — Application Administration
**Source**: `InquirySpark.Admin/Areas/Inquiry/Controllers/ApplicationsController.cs`
- List, Create, Edit, Delete, Details for `Application` entities
- Dependencies: `InquirySparkContext` (read-only SQLite)

### IA-F02 — Application User Management
**Sources**: `ApplicationUsersController.cs`, `ApplicationUserRolesController.cs`
- View, manage, and assign users to applications; assign roles per application user
- Dependencies: `InquirySparkContext`, `ControlSparkUserContextConnection` (Identity)

### IA-F03 — Application Properties
**Source**: `AppPropertiesController.cs`
- CRUD for per-application property key-value pairs

### IA-F04 — Application Surveys
**Source**: `ApplicationSurveysController.cs`
- Assign surveys to applications; manage application-survey association

### IA-F05 — Survey Authoring
**Sources**: `SurveysController.cs`, `SurveyEmailTemplatesController.cs`
- Full survey lifecycle: List, Create, Edit, Delete, Details
- Email template management per survey

### IA-F06 — Question Authoring
**Sources**: `QuestionsController.cs`, `QuestionGroupsController.cs`, `QuestionGroupMembersController.cs`, `QuestionAnswersController.cs`
- Manage questions, group questions into groups, manage group membership, manage answer options

### IA-F07 — Inquiry Operations
**Sources**: `CompaniesController.cs`, `ImportHistoriesController.cs`, `SurveyStatusController.cs`, `SurveyReviewStatusController.cs`
- Company management; import history tracking; survey and review status workflows

### IA-F08 — Site Configuration
**Sources**: `SiteRolesController.cs`, `SiteAppMenusController.cs`
- Site-wide role definitions and menu/navigation configuration

### IA-F09 — Lookup Table Management
**Sources**: `LuApplicationTypesController.cs`, `LuQuestionTypesController.cs`, `LuReviewStatusController.cs`, `LuSurveyResponseStatusController.cs`, `LuSurveyTypesController.cs`, `LuUnitOfMeasuresController.cs`
- Manage all lookup/reference tables used across the application

### IA-F10 — Role and Access Management
**Sources**: `RolesController.cs` (Area/Inquiry), `RoleManagementController.cs` (non-area)
- Area-scoped role CRUD; non-area role management UI (broader scope)

### IA-F11 — Charts and Analytics
**Sources**: `ChartSettingsController.cs` (area), `ChartBuilderController.cs` (non-area), `ChartDefinitionsController.cs` (API)
- Chart setting configuration per application; chart builder UI; chart definition API

### IA-F12 — System Health and Observability
**Sources**: `SystemHealthController.cs` (non-area), `SystemHealthController.cs` (API: `Api/SystemHealthController.cs`)
- System health dashboard view; health API endpoint

### IA-F13 — User Preferences
**Source**: `Api/UserPreferencesController.cs`
- API for reading/writing per-user preferences

### IA-F14 — Conversation Integration
**Source**: `Api/ConversationController.cs`
- API endpoint for InquirySpark.Admin to interact with conversation/decision state (integration point with DecisionSpark domain)

### IA-F15 — Home / Landing
**Source**: `HomeController.cs`
- Admin home page and navigation hub

---

## Cross-App Integration Points

| Point | DecisionSpark Side | InquirySpark.Admin Side |
|---|---|---|
| Conversation API | `ConversationController` (session-driven) | `Api/ConversationController` (client) |
| Identity | API-key auth (no Identity) | `ControlSparkUserContextConnection` (Identity) |
| Data storage | File system (JSON) | SQLite (read-only via `InquirySparkContext`) |

---

## Capabilities Requiring Special Handling

| Capability | Reason |
|---|---|
| DS-F03 (Spec API, API-key auth) | Must be re-secured with Identity-based authorization in InquirySpark.Web; API-key middleware is not reused |
| DS-F01 (Session state) | In-memory `ISessionStore` must be preserved or replaced; no DB persistence |
| IA-F02 (User/Role mgmt) | Requires writable `ControlSparkUserContextConnection` Identity store |
| IA-F09 (Lookup tables) | Read-only SQLite; verify all tables are accessible via `InquirySparkContext` |
