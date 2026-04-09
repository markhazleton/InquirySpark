# API Contract: Conversation API

**Feature**: 002-hateoas-conversation-api | **Version**: v1 | **Date**: 2026-04-08

## Base URL

```
/api/v1/conversation
```

All endpoints are **anonymous** (`[AllowAnonymous]`). No cookies, sessions, or bearer tokens required.

---

## Endpoints

### POST `/api/v1/conversation/start`

Authenticate, optionally start/resume/restart a survey conversation.

#### Request

**Content-Type**: `application/json`

```json
{
  "account_name": "string (required)",
  "password": "string (required)",
  "application_id": 1,
  "survey_id": 5,
  "conversation_id": "7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8",
  "action": "resume"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `account_name` | string | Yes | `ApplicationUser.AccountNm` |
| `password` | string | Yes | Plaintext password to verify against `PasswordHash` |
| `application_id` | int | Yes | `Application.ApplicationId` — scopes survey discovery |
| `survey_id` | int | No | Which survey to start; omit to get survey list |
| `conversation_id` | GUID | No | Existing conversation to resume or restart |
| `action` | string | No | `"resume"` (default) or `"restart"` — only meaningful with `conversation_id` |

#### Response Scenarios

**200 — Survey list** (no `survey_id` provided):
```json
{
  "conversation_id": "00000000-0000-0000-0000-000000000000",
  "action": {
    "action_type": "survey_selection",
    "question": null,
    "surveys": [
      {
        "survey_id": 5,
        "survey_name": "Customer Satisfaction",
        "survey_short_name": "CSAT",
        "survey_description": "Rate your experience..."
      }
    ],
    "completion_message": null
  },
  "next_url": null,
  "prev_url": null,
  "conversation_ended": false,
  "updated_utc": "2026-04-08T12:00:00Z"
}
```

**200 — Conversation started** (new, `survey_id` provided):
```json
{
  "conversation_id": "7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8",
  "action": {
    "action_type": "question",
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
    "surveys": null,
    "completion_message": null
  },
  "next_url": "/api/v1/conversation/next/7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8/7",
  "prev_url": null,
  "conversation_ended": false,
  "updated_utc": "2026-04-08T12:00:00Z"
}
```

**200 — Conversation resumed** (`conversation_id` provided, skips answered questions):
```json
{
  "conversation_id": "7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8",
  "action": {
    "action_type": "question",
    "question": {
      "question_id": 9,
      "question_group_id": 3,
      "text": "Any additional feedback?",
      "options": [],
      "allow_free_text": true
    },
    "surveys": null,
    "completion_message": null
  },
  "next_url": "/api/v1/conversation/next/7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8/9",
  "prev_url": "/api/v1/conversation/next/7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8/8",
  "conversation_ended": false,
  "updated_utc": "2026-04-08T12:00:00Z"
}
```

**400 — Bad Request**:
```json
{
  "errors": ["application_id is required and must refer to a known application."]
}
```
Triggers: missing/unknown `application_id`, inactive survey, survey with no questions, `conversation_id` belongs to different user.

**401 — Unauthorized**:
```json
{
  "errors": ["Invalid credentials."]
}
```
Triggers: wrong password, unknown account, blank password in database.

**404 — Not Found**:
```json
{
  "errors": ["Conversation not found."]
}
```
Triggers: `conversation_id` does not exist.

---

### POST `/api/v1/conversation/next/{conversationId}/{questionId}`

Submit an answer and advance, or read the current question (no body).

#### Path Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `conversationId` | GUID | `SurveyResponse.ConversationId` — from `next_url` |
| `questionId` | int | `Question.QuestionId` — from `next_url` |

#### Request (optional)

**Content-Type**: `application/json`

```json
{
  "question_answer_id": 11,
  "user_input": "Additional comments here"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `question_answer_id` | int | No | Selected predefined answer option |
| `user_input` | string | No | Free-text answer |

If body is **empty or null**, the endpoint returns the current question without modifying state (read mode per FR-006).

#### Response Scenarios

**200 — Next question**:
```json
{
  "conversation_id": "7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8",
  "action": {
    "action_type": "question",
    "question": {
      "question_id": 8,
      "question_group_id": 3,
      "text": "Would you recommend us to a friend?",
      "options": [
        { "id": 15, "text": "Yes", "sort": 1 },
        { "id": 16, "text": "No", "sort": 2 }
      ],
      "allow_free_text": false
    },
    "surveys": null,
    "completion_message": null
  },
  "next_url": "/api/v1/conversation/next/7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8/8",
  "prev_url": "/api/v1/conversation/next/7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8/7",
  "conversation_ended": false,
  "updated_utc": "2026-04-08T12:01:00Z"
}
```

**200 — Conversation complete** (last question answered):
```json
{
  "conversation_id": "7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8",
  "action": {
    "action_type": "complete",
    "question": null,
    "surveys": null,
    "completion_message": "Thank you for completing the survey."
  },
  "next_url": null,
  "prev_url": "/api/v1/conversation/next/7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8/10",
  "conversation_ended": true,
  "updated_utc": "2026-04-08T12:02:00Z"
}
```

**400 — Invalid answer**:
```json
{
  "errors": ["question_answer_id 99 does not belong to question 7."]
}
```

**404 — Not Found**:
```json
{
  "errors": ["Conversation not found."]
}
```

---

## JSON Naming Convention

All JSON property names use **snake_case** to match the spec envelope schema. Configure `JsonSerializerOptions` with `JsonNamingPolicy.SnakeCaseLower` on the controller or globally for the API route.

## Error Response Format

All error responses use:
```json
{
  "errors": ["Human-readable error message."]
}
```

This aligns with the existing `BaseResponse<T>.Errors` pattern. The controller maps `BaseResponse.Errors` directly to the response body for non-200 cases.

## HATEOAS Link Construction

Links are **relative URLs** constructed by the service:

```
next_url = $"/api/v1/conversation/next/{conversationId}/{nextQuestionId}"
prev_url = $"/api/v1/conversation/next/{conversationId}/{prevQuestionId}"
```

- `next_url` is `null` when `conversation_ended` is `true`
- `prev_url` is `null` for the first question
- The client follows these URLs verbatim — no path construction needed

## Idempotency

- **`/start` with same `conversation_id`**: Always returns the current state (resume) or resets (restart). No duplicate `SurveyResponse` created.
- **`/next` with same answer**: Overwrites existing `SurveyResponseAnswer`. Subsequent answers deleted per step-back rule.
- **`/next` without body**: Read-only, no state change.
