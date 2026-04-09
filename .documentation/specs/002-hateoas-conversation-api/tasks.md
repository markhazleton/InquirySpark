# Tasks: HATEOAS Conversation API

**Input**: Design documents from `/specs/002-hateoas-conversation-api/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅, quickstart.md ✅

**Tests**: Not explicitly requested in the feature specification. Test tasks are omitted. Integration tests are included as a polish-phase checkpoint only.

**Organization**: Tasks are grouped by user story (4 stories: P1-P4) to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3, US4)
- Include exact file paths in descriptions

## Path Conventions

All paths are relative to the repository root `C:\GitHub\markhazleton\InquirySpark\`.

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Schema changes, entity updates, DTO models, and project configuration needed before any service logic.

- [ ] T001 Add `ConversationId` (Guid) property to `SurveyResponse` entity in `InquirySpark.Repository/Database/SurveyResponse.cs`
- [ ] T002 Add `PasswordHash` (string?, nullable) property to `ApplicationUser` entity in `InquirySpark.Repository/Database/ApplicationUser.cs`
- [ ] T003 Add unique index on `SurveyResponse.ConversationId` and column config in `InquirySparkContext.OnModelCreating` in `InquirySpark.Repository/Database/InquirySparkContext.cs`
- [ ] T004 Create EF Core migration for `ConversationId` and `PasswordHash` columns in `InquirySpark.Repository/Migrations/` (run `dotnet ef migrations add AddConversationApi`)
- [ ] T005 Create offline script `eng/setup-conversation-db.ps1` to duplicate `InquirySpark.db` into an ephemeral `conversation-dev.db` and apply migrations
- [ ] T006 Update local development (`appsettings.Development.json`) to target `conversation-dev.db` with `Mode=ReadWriteCreate` while leaving `InquirySpark.db` `ReadOnly`
- [ ] T007 [P] Create `ConversationEnvelope` DTO in `InquirySpark.Common/Models/ConversationEnvelope.cs`
- [ ] T008 [P] Create `ConversationAction` DTO in `InquirySpark.Common/Models/ConversationAction.cs`
- [ ] T009 [P] Create `ConversationQuestion` DTO in `InquirySpark.Common/Models/ConversationQuestion.cs`
- [ ] T010 [P] Create `ConversationAnswerOption` DTO in `InquirySpark.Common/Models/ConversationAnswerOption.cs`
- [ ] T011 [P] Create `ConversationSurveyOption` DTO in `InquirySpark.Common/Models/ConversationSurveyOption.cs`
- [ ] T012 [P] Create `ConversationStartRequest` DTO in `InquirySpark.Common/Models/ConversationStartRequest.cs`
- [ ] T013 [P] Create `ConversationNextRequest` DTO in `InquirySpark.Common/Models/ConversationNextRequest.cs`

**Checkpoint**: All entity changes, migrations, DTOs, and config are in place. No service or controller logic yet.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Service interface, controller skeleton, DI registration, JSON config, and shared mapper logic that ALL user stories depend on.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [ ] T014 Create `IConversationService` interface with `StartConversationAsync` and `NextStepAsync` method signatures in `InquirySpark.Repository/Services/IConversationService.cs`
- [ ] T015 Create `ConversationService` class skeleton (primary constructor with `InquirySparkContext`, `ILogger<ConversationService>`) implementing `IConversationService` in `InquirySpark.Repository/Services/ConversationService.cs` — methods throw `NotImplementedException` initially
- [ ] T016 Create `ConversationService_Mappers` static mapper class with `ToConversationQuestion`, `ToConversationAnswerOption`, `ToConversationSurveyOption`, and `ToConversationEnvelope` methods in `InquirySpark.Repository/Services/ConversationService_Mappers.cs`
- [ ] T017 Create `ConversationController` with `[ApiController]`, `[AllowAnonymous]`, `[Route("api/v1/conversation")]`, snake_case JSON config, and `POST /start` + `POST /next/{conversationId}/{questionId}` action stubs in `InquirySpark.Admin/Controllers/Api/ConversationController.cs`
- [ ] T018 Register `IConversationService` / `ConversationService` as `AddScoped` in `InquirySpark.Admin/Program.cs`
- [ ] T019 Implement private `AuthenticateUserAsync` helper method in `ConversationService` — lookup `ApplicationUser` by `AccountNm`, verify password using `PasswordHasher<ApplicationUser>`, perform lazy hash migration from plaintext `Password` to `PasswordHash` in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T020 Implement private `GetOrderedQuestionsAsync` helper method in `ConversationService` — load survey with `QuestionGroups.OrderBy(GroupOrder).SelectMany(QuestionGroupMembers.OrderBy(DisplayOrder))` returning ordered `List<(QuestionGroupMember, Question)>` in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T021 Implement private `BuildEnvelope` helper method in `ConversationService` — construct `ConversationEnvelope` with HATEOAS `next_url`/`prev_url` links, `action_type`, and `conversation_ended` flag in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T022 Verify build passes with `dotnet build InquirySpark.sln` — fix any compilation errors from Phase 1–2

**Checkpoint**: Foundation ready — controller responds to requests (returns stubs), DI wired, helpers ready. User story implementation can now begin.

---

## Phase 3: User Story 1 — Complete a Survey End-to-End (Priority: P1) 🎯 MVP

**Goal**: A user authenticates, starts a survey, answers each question sequentially via HATEOAS links, and receives `conversation_ended: true` with a completion message after the last question.

**Independent Test**: Call `POST /start` with valid credentials and `survey_id`, follow `next_url` through every question submitting answers, verify final response has `conversation_ended: true` and `completion_message`. Confirm `SurveyResponse` exists with `ConversationId` and all `SurveyResponseAnswer` rows persisted.

### Implementation for User Story 1

- [ ] T023 [US1] Inspect `LuSurveyResponseStatus` seed data in `data/sqlite/InquirySpark.db` to determine the `StatusId` values for "in-progress" and "completed" statuses; define named constants (e.g., `StatusInProgress = 1`, `StatusCompleted = ?`) in `ConversationService` — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T024 [US1] Implement `StartConversationAsync` — new conversation path (no `conversation_id`): validate `application_id` exists, validate `survey_id` linked to application via `ApplicationSurvey`, validate survey active (`EndDt` null or in future, `StartDt` null or in past) and has questions, create `SurveyResponse` with `ConversationId = Guid.NewGuid()`, `DataSource = "ConversationAPI"`, `StatusId` set to in-progress constant, return first question envelope — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T025 [US1] Implement `NextStepAsync` — answer submission path: locate `SurveyResponse` by `ConversationId`, validate `questionId` is current step, upsert `SurveyResponseAnswer` (set `QuestionAnswerId` or `AnswerComment`), determine next question, return next question envelope or `conversation_ended: true` — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T026 [US1] Implement `NextStepAsync` — completion path: when last question answered, update `SurveyResponse.StatusId` to completed constant, include `Survey.CompletionMessage` in envelope, set `conversation_ended: true`, `next_url: null` — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T027 [US1] Implement `NextStepAsync` — read mode (FR-006): when request body is null/empty, return current question without modifying state — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T028 [US1] Wire `ConversationController.Start` action — call `_service.StartConversationAsync`, and return via `ApiResponseHelper.ExecuteAsync` to map HTTP status codes automatically — in `InquirySpark.Admin/Controllers/Api/ConversationController.cs`
- [ ] T029 [US1] Wire `ConversationController.Next` action — call `_service.NextStepAsync`, and return via `ApiResponseHelper.ExecuteAsync` to map HTTP status codes automatically — in `InquirySpark.Admin/Controllers/Api/ConversationController.cs`
- [ ] T030 [US1] Verify build passes with `dotnet build InquirySpark.sln`

**Checkpoint**: Full end-to-end survey conversation works. User authenticates, starts survey, walks through all questions, and sees completion message. This is the MVP.

---

## Phase 4: User Story 2 — Survey List at Start (Priority: P2)

**Goal**: When `/start` is called with valid credentials but no `survey_id`, the API returns a list of surveys available to the specified `application_id`.

**Independent Test**: Call `POST /start` with valid credentials, a valid `application_id`, and no `survey_id`. Verify response contains `action_type: "survey_selection"` and an array of surveys scoped to that application.

### Implementation for User Story 2

- [ ] T031 [US2] Implement `StartConversationAsync` — survey list path: when `SurveyId` is null, query `ApplicationSurvey` filtered by `ApplicationId`, join `Survey`, map to `ConversationSurveyOption` list, return envelope with `action_type: "survey_selection"` — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T032 [US2] Handle edge case: unknown `survey_id` returns `404 Not Found` with descriptive error — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T033 [US2] Verify build passes with `dotnet build InquirySpark.sln`

**Checkpoint**: Users can discover available surveys before starting one. Survey list is filtered by `application_id`.

---

## Phase 5: User Story 3 — Authentication Failure (Priority: P3)

**Goal**: Invalid credentials are rejected with `401 Unauthorized`, and no survey data is leaked. Unknown accounts return the same `401` to prevent user enumeration.

**Independent Test**: Call `POST /start` with a wrong password — expect `401`. Call with unknown `account_name` — expect `401` (same error, no hint about account existence).

### Implementation for User Story 3

- [ ] T034 [US3] Add credential validation to `StartConversationAsync` — return `401 Unauthorized` for: unknown `AccountNm`, wrong password, blank `PasswordHash` AND blank `Password` in database — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T035 [US3] Ensure `401` response uses generic error message (`"Invalid credentials."`) that does not reveal whether the account exists — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T036 [US3] Add `application_id` validation — return `400 Bad Request` when `ApplicationId` is 0 or does not exist in `Application` table — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T037 [US3] Ensure `ApiResponseHelper` correctly maps `BaseResponse` error states to proper HTTP status codes in controller: distinguish between 400, 401, 404 based on error metadata — in `InquirySpark.Admin/Controllers/Api/ConversationController.cs`
- [ ] T038 [US3] Verify build passes with `dotnet build InquirySpark.sln`

**Checkpoint**: Security boundary is solid. All invalid credential scenarios return the correct error without leaking information.

---

## Phase 6: User Story 4 — Resume or Restart an Existing Response (Priority: P4)

**Goal**: A user can resume an interrupted conversation from the last unanswered question, or restart from question 1 with all previous answers cleared.

**Independent Test**: Start a conversation, answer 2 questions, call `/start` with `conversation_id` (no `action`) — verify question 3 returned and earlier answers intact. Then call `/start` with same `conversation_id` and `action: "restart"` — verify all answers deleted and question 1 returned.

### Implementation for User Story 4

- [ ] T039 [US4] Implement `StartConversationAsync` — resume path: when `ConversationId` is provided (no `action` or `action: "resume"`), locate `SurveyResponse`, verify belongs to authenticated user (return `400` if not), find first unanswered question, return it in envelope — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T040 [US4] Implement `StartConversationAsync` — restart path: when `ConversationId` + `action: "restart"`, locate `SurveyResponse`, verify ownership, delete all `SurveyResponseAnswer` rows for that `SurveyResponseId`, reset `StatusId` to in-progress constant, return question 1 — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T041 [US4] Implement idempotent answer upsert in `NextStepAsync` — if answering a question that already has an answer, overwrite it and delete all subsequent answers per step-back rule (spec edge case: only 1 step forward or back) — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T042 [US4] Handle edge case: `conversation_id` not found returns `404`; `conversation_id` belongs to different user returns `400` — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T043 [US4] Verify build passes with `dotnet build InquirySpark.sln`

**Checkpoint**: All 4 user stories are independently functional. Resume and restart work correctly without data corruption.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Edge case hardening, XML documentation, build verification, and integration test scaffold.

- [ ] T044 [P] Add XML doc comments to all public classes and methods in `IConversationService`, `ConversationService`, `ConversationController`, and all DTO models — across `InquirySpark.Repository/Services/` and `InquirySpark.Common/Models/`
- [ ] T045 [P] Handle edge case: survey with zero questions returns `400 Bad Request` from `StartConversationAsync` — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T046 [P] Handle edge case: expired survey (`EndDt` in the past) or not-yet-started survey (`StartDt` in the future) returns `400 Bad Request` from `StartConversationAsync` — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T047 [P] Handle edge case: free-text question accepts `user_input` and stores in `AnswerComment`; option-only question rejects `user_input` without `question_answer_id` — in `InquirySpark.Repository/Services/ConversationService.cs`
- [ ] T048 Create integration test scaffold `ConversationApiTests.cs` with at least one end-to-end test that calls `/start` → `/next` through all questions → verifies `conversation_ended` in `InquirySpark.Common.Tests/Integration/ConversationApiTests.cs`
- [ ] T049 Run full build verification: `dotnet build InquirySpark.sln -warnaserror` and `dotnet test` — fix any warnings or failures
- [ ] T050 Run quickstart.md walkthrough validation — manually verify the curl examples from `quickstart.md` against a running instance

---

## Dependencies & Execution Order

### Phase Dependencies

```text
Phase 1 (Setup)          → No dependencies — start immediately
Phase 2 (Foundational)   → Depends on Phase 1 completion — BLOCKS all user stories
Phase 3 (US1 - P1) 🎯   → Depends on Phase 2 — core MVP
Phase 4 (US2 - P2)       → Depends on Phase 2 — can run in parallel with Phase 3
Phase 5 (US3 - P3)       → Depends on Phase 2 — can run in parallel with Phase 3/4
Phase 6 (US4 - P4)       → Depends on Phase 3 (needs working /start and /next to extend)
Phase 7 (Polish)          → Depends on Phases 3–6 completion
```

### User Story Dependencies

- **US1 (P1)**: Depends on Phase 2 only. No dependency on other stories. **This is the MVP.**
- **US2 (P2)**: Depends on Phase 2. Adds a branch to the same `StartConversationAsync` method as US1, but the survey-list path is independent from the start-survey path.
- **US3 (P3)**: Depends on Phase 2. Authentication hardening can be added to the existing `AuthenticateUserAsync` helper independently.
- **US4 (P4)**: Depends on US1 (Phase 3) — resume/restart logic requires a working conversation flow to extend. Cannot be implemented before US1 is complete.

### Within Each User Story

1. Service logic before controller wiring
2. Core path before edge cases
3. Build verification at end of each phase

### Parallel Opportunities

**Phase 1**: T007–T013 (all DTOs) can run in parallel — different files, no dependencies.

**Phase 2**: T014–T018 are sequential (interface → class → controller → DI). T019–T021 (helpers) can run in parallel after T015.

**Phase 3–5**: US1, US2, US3 can be developed in parallel after Phase 2 if team capacity allows (different code paths in the same service). However, single-developer sequential execution in priority order (P1→P2→P3) is recommended.

**Phase 6**: US4 must follow US1 completion.

**Phase 7**: T044–T047 can all run in parallel.

---

## Parallel Example: Phase 1 (Setup)

```text
Sequential:  T001 → T002 → T003 → T004 → T005 → T006
Parallel:    T007 ┐
             T008 ┤
             T009 ┤  (all DTOs — different files)
             T010 ┤
             T011 ┤
             T012 ┤
             T013 ┘
```

## Implementation Strategy

1. **MVP (Phase 1 + 2 + 3)**: Delivers core end-to-end survey flow — a user can authenticate, start a survey, answer all questions, and see the completion message. This is the minimum viable product.
2. **Enhanced Discovery (+ Phase 4)**: Adds survey list so clients don't need to hardcode survey IDs.
3. **Security Hardened (+ Phase 5)**: Adds auth failure handling and validation.
4. **Full Feature (+ Phase 6)**: Adds resume/restart for interrupted sessions.
5. **Production Ready (+ Phase 7)**: Documentation, edge cases, tests, and build verification.
