# Feature Specification: HATEOAS Conversation API

**Feature Branch**: `002-hateoas-conversation-api`  
**Created**: 2026-04-08  
**Status**: Complete  
**Scope**: InquirySpark.Admin — new API area  

## Overview

Add a RESTful, HATEOAS-driven Conversation API to `InquirySpark.Admin` that guides an application user through a structured survey experience in discrete steps. The API exposes two primary endpoints — `/start` and `/next` — and uses hypermedia links so a thin client never needs to know workflow structure. The implementation persists a new GUID `ConversationId` field on the existing `SurveyResponse` table and otherwise relies on the existing `ApplicationUser`, `Survey`, `QuestionGroup`, `QuestionGroupMember`, `Question`, `QuestionAnswer`, `SurveyResponse`, and `SurveyResponseAnswer` entities.

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 — Complete a Survey End-to-End (Priority: P1)

A user authenticates with their account name and password, receives a list of available surveys, selects one, and is walked through each question one at a time until all questions are answered. Each answer is persisted immediately. At the end the user sees the survey's completion message.

**Why this priority**: This is the core value of the entire feature. All other stories depend on this flow existing.

**Independent Test**: Can be fully tested with a REST client (e.g., Swagger UI or curl) by calling `/start`, following `next_url` through each question step, and verifying a `SurveyResponse` with a persisted GUID `ConversationId` plus `SurveyResponseAnswer` rows exists in the database after the final step.

**Acceptance Scenarios**:

1. **Given** valid `account_name` and `password`, **When** `POST /api/v1/conversation/start` is called with a valid `survey_id`, **Then** a `SurveyResponse` record is created, the first question is returned in the response envelope, and `next_url` points to the first question step.
2. **Given** an active conversation at step N, **When** `POST /api/v1/conversation/next/{conversation_id}/{question_id}` is called with a valid answer, **Then** the answer is persisted as a `SurveyResponseAnswer`, and the response envelope returns the next question with an updated `next_url` (or `conversation_ended: true` if no more questions).
3. **Given** the final question is answered, **When** the last `/next` call is made, **Then** `conversation_ended` is `true`, `next_url` is absent, and the `Survey.CompletionMessage` is included in the response.

---

### User Story 2 — Survey List at Start (Priority: P2)

Before a survey is selected, the user can discover which surveys are available to take via the start endpoint, so they can make an informed choice.

**Why this priority**: Needed for a self-service thin client that does not hardcode survey IDs.

**Independent Test**: Call `POST /api/v1/conversation/start` with valid credentials but no `survey_id`; response must include a list of available surveys with their IDs and names.

**Acceptance Scenarios**:

1. **Given** valid credentials and no `survey_id`, **When** `POST /api/v1/conversation/start` is called, **Then** the response contains `action_type: "survey_selection"` and an array of available surveys.
2. **Given** an invalid or unknown `survey_id`, **When** `POST /api/v1/conversation/start` is called, **Then** the API returns `404 Not Found` with a descriptive error.
3. **Given** valid credentials and a valid `application_id`, **When** `POST /api/v1/conversation/start` is called without `survey_id`, **Then** only surveys available to that application are returned.

---

### User Story 3 — Authentication Failure (Priority: P3)

Attempts to start a conversation with incorrect credentials are rejected with a clear error, preventing unauthorized survey submissions.

**Why this priority**: Security boundary — must exist before any other story can be considered safe.

**Independent Test**: Call `POST /api/v1/conversation/start` with a wrong password; expect `401 Unauthorized`.

**Acceptance Scenarios**:

1. **Given** an invalid password, **When** `POST /api/v1/conversation/start` is called, **Then** `401 Unauthorized` is returned and no `SurveyResponse` is created.
2. **Given** an unknown `account_name`, **When** `POST /api/v1/conversation/start` is called, **Then** `401 Unauthorized` is returned (account existence is not revealed).

---

### User Story 4 — Resume or Restart an Existing Response (Priority: P4)

A user who started a survey but did not finish can resume from their last unanswered question, or deliberately restart the survey from the beginning with all prior answers cleared, by passing the existing `conversation_id` to `/start`.

**Why this priority**: Prevents accidental duplicate `SurveyResponse` records when a session is interrupted and gives users explicit control to redo a survey when needed.

**Independent Test**: Start a conversation, answer two questions, then call `POST /api/v1/conversation/start` with the same `conversation_id` and no `action`; verify the third question (first unanswered) is returned and the two earlier `SurveyResponseAnswer` rows are intact.

**Acceptance Scenarios**:

1. **Given** an in-progress `SurveyResponse` with two questions answered, **When** `POST /api/v1/conversation/start` is called with `conversation_id` and no `action`, **Then** the first unanswered question is returned and no existing answers are modified.
2. **Given** a `SurveyResponse` with answers, **When** `POST /api/v1/conversation/start` is called with `conversation_id` and `action: "restart"`, **Then** all `SurveyResponseAnswer` rows for that response are deleted, `StatusId` is reset to in-progress, and question 1 is returned.
3. **Given** a `conversation_id` that belongs to a different user, **When** `POST /api/v1/conversation/start` is called, **Then** `400 Bad Request` is returned and no data is modified.

---

### Edge Cases

- What happens when a question has no defined answer options (free-text only)? The API must accept `user_input` and store it in `SurveyResponseAnswer.AnswerComment`.
- What happens if the same question is answered twice on the same response? The second submission overwrites the first (idempotent update). Furthermore, any existing answers for subsequent questions MUST be deleted, forcing the user back into sequential generation based on this new answer. The API MUST only allow 1 step forward or 1 step back.
- What if a `survey_id` belongs to a survey whose `EndDt` is in the past? Return `400 Bad Request` — survey is no longer active.
- What if an `ApplicationUser` record exists but the password field is blank? Treat as unauthenticated — `401 Unauthorized`.
- What if a survey has zero questions? Return `400 Bad Request` — no conversation can be started.

---

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The API MUST authenticate callers by matching `account_name` to `ApplicationUser.AccountNm` and verifying the supplied password using the ASP.NET Core Identity `PasswordHasher`. A new hashed password field must be added to `ApplicationUser`, and existing plaintext passwords iteratively converted or migrated.
- **FR-002**: `POST /api/v1/conversation/start` with valid credentials and no `survey_id` MUST return a list of available surveys for the supplied `application_id` (`SurveyNm`, `SurveyShortNm`, `SurveyDs`, `SurveyId`) plus `action_type: "survey_selection"`.
- **FR-003**: `POST /api/v1/conversation/start` behavior depends on whether `conversation_id` is supplied:
  - **No `conversation_id`**: Always creates a new `SurveyResponse` linked to the authenticated `ApplicationUser` and scoped to the specified `Application`. A new GUID `ConversationId` is generated and persisted on `SurveyResponse`. `survey_id` is required in this case and MUST refer to a survey available to that application.
  - **`conversation_id` + default or `action: "resume"`**: Locates the existing `SurveyResponse` by `ConversationId`, verifies it belongs to the authenticated user, and returns the next unanswered question (the first `Question` in ordered sequence that has no corresponding `SurveyResponseAnswer`).
  - **`conversation_id` + `action: "restart"`**: Locates the existing `SurveyResponse` by `ConversationId`, deletes all `SurveyResponseAnswer` rows for that `SurveyResponseId`, resets `SurveyResponse.StatusId` to in-progress, and returns question 1.
  - If `application_id` is absent or unknown, the API MUST return `400 Bad Request`. If a supplied `conversation_id` does not belong to the authenticated user, the API MUST return `400 Bad Request` (not `404`, to avoid user enumeration).
- **FR-004**: Every response from `/start` and `/next` MUST follow the same envelope schema: `conversation_id`, `action` (containing `action_type` and `question` or `completion`), `next_url`, `prev_url` (when applicable), and `conversation_ended`.
- **FR-005**: `POST /api/v1/conversation/next/{conversation_id}/{question_id}` with an answer body MUST authorize the request by locating the matching `SurveyResponse.ConversationId`, persist the answer as a `SurveyResponseAnswer`, and return the next unanswered question, or set `conversation_ended: true` when no more questions remain.
- **FR-006**: `POST /api/v1/conversation/next/{conversation_id}/{question_id}` without an answer body MUST authorize the request by locating the matching `SurveyResponse.ConversationId` and return the current question for that step without modifying state (read mode).
- **FR-007**: When the last question is answered, the API MUST update `SurveyResponse.StatusId` to the completed status and include `Survey.CompletionMessage` in the final response.
- **FR-008**: Questions with predefined `QuestionAnswer` options MUST be returned with the full list of options (id, display text, sort order); the caller submits the chosen `question_answer_id`.
- **FR-009**: Questions without predefined answers MUST accept a free-text `user_input` string stored in `SurveyResponseAnswer.AnswerComment`.
- **FR-010**: The API MUST reuse the existing schema except for adding a new GUID `ConversationId` column to `SurveyResponse` and the new hashed password field. It MUST NOT introduce any new tables. Schema modifications and `.db` file updates MUST be applied using EF Core Migrations.
- **FR-011**: All endpoints MUST return standard HTTP status codes: `200 OK`, `400 Bad Request`, `401 Unauthorized`, `404 Not Found`, `500 Internal Server Error`.
- **FR-012**: The `next_url` in every response MUST be a fully-qualified relative URL the client can POST to without constructing its own path. The `{conversation_id}` segment in `next_url` MUST be the persisted `SurveyResponse.ConversationId` GUID and the `{question_id}` segment MUST be the `Question.QuestionId` primary key value as returned by the server.
- **FR-013**: Questions MUST be ordered by `QuestionGroup.GroupOrder` then `QuestionGroupMember` sequence within the survey, so navigation is deterministic.
- **FR-014**: The API MUST be hosted under a versioned route prefix (e.g., `/api/v1/conversation/`) so future versions can be added without breaking existing clients.
- **FR-015**: The Conversation API endpoints MUST be accessible without an ASP.NET Core Identity session or cookie. They MUST be decorated as anonymous endpoints. `POST /start` authenticates with `account_name` and `password` in the request body. `POST /next` is authorized solely by possession of a valid `conversation_id` that resolves to an existing `SurveyResponse.ConversationId`; no additional authorization layer is required.
- **FR-016**: The API MUST be self-documenting via Swagger/OpenAPI. The Admin project MUST expose a Swagger UI endpoint (e.g., `/swagger`) documenting the Conversation API routes, HTTP status codes, and request/response schemas.

### Key Entities

- **ApplicationUser**: Represents the person taking the survey. Identified by `AccountNm`, authenticated via `Password`. `ApplicationUserId` is stored on the created `SurveyResponse` as `AssignedUserId`.
- **Application**: Defines the application context supplied by `application_id`. It scopes survey discovery and determines which surveys may be started for a conversation.
- **Survey**: The survey template. Defines the ordered set of questions via `QuestionGroup` → `QuestionGroupMember` → `Question`. `SurveyNm`, `SurveyDs`, and `CompletionMessage` are surfaced in API responses.
- **SurveyResponse**: The conversation session. One record per user–survey run. `ConversationId` is a persisted GUID used in public API routes and HATEOAS links. `StatusId` tracks `in_progress` vs `complete`. Linked to `ApplicationUser` (`AssignedUserId`) and `Survey` (`SurveyId`).
- **SurveyResponseAnswer**: One row per answered question within a session. References `SurveyResponseId`, `QuestionId`, `QuestionAnswerId` (for option-based answers), and `AnswerComment` (for free-text answers).
- **Question / QuestionAnswer**: Define the questions and their predefined answer options. `QuestionAnswerNm` is the display text; `QuestionAnswerId` is submitted by the client.
- **QuestionGroup / QuestionGroupMember**: Provide ordering. Questions are navigated in `GroupOrder` + member sequence order.

---

## Response Envelope Schema

Every `/start` and `/next` response uses the same structure:

```json
{
  "conversation_id": "7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8",
  "action": {
    "action_type": "question | survey_selection | complete",
    "question": {
      "question_id": 7,
      "question_group_id": 3,
      "text": "How satisfied are you with the process?",
      "options": [
        { "id": 11, "text": "Very Satisfied", "sort": 1 },
        { "id": 12, "text": "Satisfied", "sort": 2 }
      ],
      "allow_free_text": false
    },
    "surveys": [ ],
    "completion_message": "Thank you for completing the survey."
  },
  "next_url": "/api/v1/conversation/next/7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8/7",
  "prev_url": "/api/v1/conversation/next/7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8/6",
  "conversation_ended": false,
  "updated_utc": "2026-04-08T12:00:00Z"
}
```

| `action_type`      | What the client renders                                  |
|--------------------|----------------------------------------------------------|
| `survey_selection` | A list of surveys to choose from                         |
| `question`         | A single question with optional predefined answer options|
| `complete`         | The survey is done; display `completion_message`         |

---

## API Endpoints Summary

| Method | Route | Purpose |
|--------|-------|---------|
| `POST` | `/api/v1/conversation/start` | Authenticate user; optionally start a survey |
| `POST` | `/api/v1/conversation/next/{conversation_id}/{question_id}` | Submit answer and advance; or read current step |

### POST `/api/v1/conversation/start`

**Request body:**

| Field | Required | Description |
|-------|----------|-------------|
| `account_name` | Yes | `ApplicationUser.AccountNm` |
| `password` | Yes | `ApplicationUser.Password` |
| `survey_id` | No | Which survey to start; omit to receive survey list |
| `application_id` | **Yes** | `Application.ApplicationId` — scopes the `SurveyResponse` to a specific application; returns `400` if omitted or unknown |
| `conversation_id` | No | Existing `SurveyResponse.ConversationId` GUID to resume or restart. If omitted, a new response is always created. |
| `action` | No | `"resume"` (default) or `"restart"`. Only meaningful when `conversation_id` is supplied. `"restart"` deletes all `SurveyResponseAnswer` rows for the response and returns question 1. |

**Responses:**
- `200` — action_type `survey_selection` (no survey_id; surveys filtered by `application_id`), `question` (survey started/resumed/restarted)
- `400` — `application_id` missing/unknown, survey inactive, survey has no questions, or `conversation_id` does not belong to the authenticated user
- `401` — credentials invalid
- `404` — supplied `conversation_id` not found

### POST `/api/v1/conversation/next/{conversation_id}/{question_id}`

`conversation_id` is the persisted `SurveyResponse.ConversationId` GUID returned by `/start` and embedded in each `next_url`. Clients use it verbatim.

`question_id` is the `Question.QuestionId` primary key value from the database. It is returned by the server in each `question` action and must be used verbatim — clients never construct or compute it.

**Request body (optional):**

| Field | Description |
|-------|-------------|
| `question_answer_id` | ID of the selected predefined answer (`QuestionAnswer.QuestionAnswerId`) |
| `user_input` | Free-text answer |

**Responses:**
- `200` — next question, or `conversation_ended: true` with action_type `complete`
- `400` — invalid answer (e.g., free text on an options-only question)
- `404` — conversation_id or question_id not found

---

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A user with valid credentials can call `/start` + follow `next_url` through all questions and receive `conversation_ended: true` without any additional client-side logic.
- **SC-002**: Every answered question produces exactly one `SurveyResponseAnswer` row; duplicate submissions for the same question overwrite the existing row rather than creating duplicates, and every conversation is addressable by a persisted GUID `ConversationId`.
- **SC-003**: Invalid credentials on `/start` return `401 Unauthorized` in 100% of cases with no survey data leaked.
- **SC-004**: The completed `SurveyResponse.StatusId` reflects the completed status after the last question is answered, verifiable by querying the database directly.
- **SC-005**: Question ordering is deterministic and consistent across multiple calls to the same survey: questions always appear in `QuestionGroup.GroupOrder` + member sequence order.
- **SC-006**: A thin client (JavaScript fetch loop) can complete an entire survey using only URLs returned in `next_url` without hardcoding any route patterns.

---

## Clarifications

### Session 2026-04-08

- Q: What hashing algorithm must be used to verify the legacy `ApplicationUser.Password` field? → A: Use ASP.NET Core Identity `PasswordHasher`. A new hashed password field will be created and existing plaintext passwords converted.
- Q: If a user updates an answer to a previously answered question, how should the API determine the next step? → A: Delete existing answers for subsequent questions and return the immediate next question. The system ONLY allows 1 step forward, 1 step back.
- Q: How will the new `ConversationId` column be added to the `SurveyResponse` table? → A: EF Core Migrations is the preferred way to modify the schema and update the `.db` files.
- Q: How should `application_id` affect survey discovery and start behavior? → A: Filter available surveys by `application_id`, and reject starting any survey not available for that application.
- Q: Should the public conversation identifier remain `response_id`, or switch to a GUID-backed `conversation_id`? → A: Add `SurveyResponse.ConversationId` as a GUID, and use it in `next_url` instead of `response_id`.
- Q: How should `/next` be authorized after `/start` succeeds? → A: `/next` is authorized by `conversation_id` alone; no credentials are sent after `/start`.
- Q: What does `{question_id}` represent in the URL path `/next/{conversation_id}/{question_id}`? → A: The database `Question.QuestionId` primary key. Clients never construct or compute this value; they read it from `next_url` returned by the server.
- Q: How is `ApplicationUser.Password` stored in the existing database? → A: Plaintext — the API must use `PasswordHasher<ApplicationUser>` for verification, with lazy migration from the plaintext `Password` field to the new `PasswordHash` field on first successful login.
- Q: Can the same user take the same survey more than once, and can an existing response be updated? → A: Yes to both. If `conversation_id` is omitted on `/start`, a new `SurveyResponse` is always created. If `conversation_id` is supplied, the default behavior is **resume** (return the next unanswered question). Supplying `action: "restart"` alongside `conversation_id` clears all existing `SurveyResponseAnswer` rows for that response and restarts from question 1.
- Q: Must the Conversation API be protected by ASP.NET Core Identity session/cookie auth in addition to request-body credentials? → A: No — the endpoints are anonymous (no `[Authorize]` middleware). `/start` uses `account_name` + `password`; `/next` uses the returned `conversation_id`.
- Q: What `Application.ApplicationId` should be used when the caller omits `application_id` on `/start`? → A: `application_id` is mandatory. The API MUST return `400 Bad Request` if it is omitted or does not refer to a known `Application`.

---

## Assumptions

1. `ApplicationUser` requires a new hashed password field using ASP.NET Core Identity. Plaintext passwords must be migrated or converted.
2. `application_id` is a required field on `/start`. No default is applied by the server; callers must supply a valid `Application.ApplicationId`.
8. `application_id` is a hard scoping boundary: survey discovery returns only surveys available to that application, and `/start` rejects any `survey_id` outside that application scope.
9. `SurveyResponse` will gain a new persisted GUID `ConversationId` column, and all public conversation URLs and request payloads will use `conversation_id` instead of the numeric `SurveyResponseId`.
10. After `/start` succeeds, the client follows HATEOAS links using `conversation_id` alone; `/next` does not require `account_name` or `password` in its request body.
7. When `conversation_id` is supplied on `/start` with `action: "restart"`, the deletion of `SurveyResponseAnswer` rows is the only destructive operation — the `SurveyResponse` record itself is preserved and reused.
3. The `SurveyResponse.DataSource` field will be set to `"ConversationAPI"` to identify responses created through this endpoint.
4. Only surveys with an `EndDt` that is null or in the future (and a `StartDt` that is null or in the past) are returned in the list and eligible to start.
5. No new tables are introduced; the only schema changes are adding `SurveyResponse.ConversationId` and `ApplicationUser.PasswordHash`.
6. No CRM handoff, no external triage engine (clearstep), and no multilingual translation are in scope.
