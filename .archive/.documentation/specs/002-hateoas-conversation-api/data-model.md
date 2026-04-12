# Data Model: HATEOAS Conversation API

**Feature**: 002-hateoas-conversation-api | **Date**: 2026-04-08

## Entity Changes

### SurveyResponse — Add `ConversationId`

| Column | Type | Nullable | Default | Index |
|--------|------|----------|---------|-------|
| `ConversationId` | `Guid` | No | `Guid.NewGuid()` | Unique (`IX_SurveyResponse_ConversationId`) |

**EF Core configuration** (in `OnModelCreating`):
```csharp
entity.Property(e => e.ConversationId)
    .HasDefaultValueSql("(hex(randomblob(16)))");  // SQLite GUID generation
entity.HasIndex(e => e.ConversationId)
    .IsUnique()
    .HasDatabaseName("IX_SurveyResponse_ConversationId");
```

**Migration SQL** (SQLite):
```sql
ALTER TABLE SurveyResponse ADD COLUMN ConversationId TEXT NOT NULL DEFAULT (hex(randomblob(16)));
CREATE UNIQUE INDEX IX_SurveyResponse_ConversationId ON SurveyResponse (ConversationId);
```

### ApplicationUser — Add `PasswordHash`

| Column | Type | Nullable | Default | Index |
|--------|------|----------|---------|-------|
| `PasswordHash` | `string` | Yes (nullable) | `null` | None |

**Migration SQL** (SQLite):
```sql
ALTER TABLE ApplicationUser ADD COLUMN PasswordHash TEXT NULL;
```

**Lazy migration logic**: When `PasswordHash` is null and plaintext `Password` matches, hash the password into `PasswordHash` and clear `Password`.

---

## New DTO Models (InquirySpark.Repository/Models/)

### ConversationEnvelope

The universal response envelope for all `/start` and `/next` responses.

```csharp
/// <summary>
/// HATEOAS response envelope for all conversation API responses.
/// </summary>
public class ConversationEnvelope
{
    public Guid ConversationId { get; set; }
    public ConversationAction Action { get; set; } = new();
    public string? NextUrl { get; set; }
    public string? PrevUrl { get; set; }
    public bool ConversationEnded { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
```

### ConversationAction

```csharp
/// <summary>
/// Describes the current action the client should render.
/// </summary>
public class ConversationAction
{
    /// <summary>
    /// One of: "question", "survey_selection", "complete"
    /// </summary>
    public string ActionType { get; set; } = string.Empty;
    public ConversationQuestion? Question { get; set; }
    public List<ConversationSurveyOption>? Surveys { get; set; }
    public string? CompletionMessage { get; set; }
}
```

### ConversationQuestion

```csharp
/// <summary>
/// A single question presented to the user during a conversation step.
/// </summary>
public class ConversationQuestion
{
    public int QuestionId { get; set; }
    public int QuestionGroupId { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<ConversationAnswerOption> Options { get; set; } = [];
    public bool AllowFreeText { get; set; }
}
```

### ConversationAnswerOption

```csharp
/// <summary>
/// A predefined answer option for a question.
/// </summary>
public class ConversationAnswerOption
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Sort { get; set; }
}
```

### ConversationSurveyOption

```csharp
/// <summary>
/// A survey available for selection when starting a conversation.
/// </summary>
public class ConversationSurveyOption
{
    public int SurveyId { get; set; }
    public string SurveyName { get; set; } = string.Empty;
    public string SurveyShortName { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
}
```

### ConversationStartRequest

```csharp
/// <summary>
/// Request body for POST /api/v1/conversation/start.
/// </summary>
public class ConversationStartRequest
{
    public string AccountName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int ApplicationId { get; set; }
    public int? SurveyId { get; set; }
    public Guid? ConversationId { get; set; }
    public string? Action { get; set; }  // "resume" (default) | "restart"
}
```

### ConversationNextRequest

```csharp
/// <summary>
/// Request body for POST /api/v1/conversation/next/{conversationId}/{questionId}.
/// </summary>
public class ConversationNextRequest
{
    public int? QuestionAnswerId { get; set; }
    public string? UserInput { get; set; }
}
```

---

## Service Interface

### IConversationService

```csharp
/// <summary>
/// Service for managing HATEOAS-driven survey conversations.
/// </summary>
public interface IConversationService
{
    /// <summary>
    /// Authenticates the user and starts, resumes, or restarts a conversation.
    /// Returns survey list if no survey_id is provided.
    /// </summary>
    Task<BaseResponse<ConversationEnvelope>> StartConversationAsync(ConversationStartRequest request);

    /// <summary>
    /// Submits an answer for the current question and returns the next step.
    /// If no answer body is provided, returns the current question without modification.
    /// </summary>
    Task<BaseResponse<ConversationEnvelope>> NextStepAsync(
        Guid conversationId,
        int questionId,
        ConversationNextRequest? request);
}
```

---

## Entity Relationships (Existing — Used by Feature)

```text
Application ──1:N──> ApplicationSurvey ──N:1──> Survey
                                                  │
Survey ──1:N──> QuestionGroup ──1:N──> QuestionGroupMember ──N:1──> Question
                                                                       │
Question ──1:N──> QuestionAnswer                                       │
                                                                       │
SurveyResponse ──1:N──> SurveyResponseAnswer ──N:1──> Question         │
     │                       └──N:1──> QuestionAnswer                  │
     └──N:1──> ApplicationUser                                         │
     └──N:1──> Application
     └──N:1──> Survey
```

### Key Query Paths

1. **Survey list for application**: `ApplicationSurvey.Where(as => as.ApplicationId == appId).Select(as => as.Survey)`
2. **Ordered questions for survey**: `Survey.QuestionGroups.OrderBy(GroupOrder).SelectMany(g => g.QuestionGroupMembers.OrderBy(DisplayOrder)).Select(m => m.Question)`
3. **Answers for response**: `SurveyResponseAnswers.Where(a => a.SurveyResponseId == responseId)`
4. **Response by conversation ID**: `SurveyResponses.FirstOrDefault(r => r.ConversationId == conversationId)`

---

## Validation Rules

| Field | Rule | HTTP Status |
|-------|------|-------------|
| `AccountName` | Required, non-empty | 400 |
| `Password` | Required, non-empty | 400 |
| `ApplicationId` | Required, must exist in `Application` table | 400 |
| `SurveyId` (when provided) | Must exist and be linked to `ApplicationId` via `ApplicationSurvey` | 404 |
| `SurveyId` (when provided) | Survey must not have `EndDt` in the past | 400 |
| `SurveyId` (when provided) | Survey must have at least one question | 400 |
| `ConversationId` (on start) | If provided, must exist and belong to authenticated user | 400 |
| `ConversationId` (on next) | Must exist | 404 |
| `QuestionId` (on next) | Must be current or previous question in sequence | 400 |
| `QuestionAnswerId` (when provided) | Must belong to the specified question | 400 |

## State Transitions

```text
[No Response] ──POST /start (new)──> SurveyResponse (StatusId=1, ConversationId=GUID)
                                         │
                                    POST /next (answer)
                                         │
                                    ┌─ More questions? ──> Return next question
                                    │
                                    └─ Last question? ──> StatusId=Completed, conversation_ended=true

[Existing Response] ──POST /start (resume)──> Return first unanswered question
                   ──POST /start (restart)──> Delete all answers, StatusId=1, return Q1
```
