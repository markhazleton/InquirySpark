# Quickstart: HATEOAS Conversation API

**Feature**: 002-hateoas-conversation-api | **Date**: 2026-04-08

## Prerequisites

- .NET 10 SDK
- SQLite database files in `data/sqlite/` (migrated with new columns)
- InquirySpark.Admin project buildable

## Setup

### 1. Apply Database Migration

```powershell
# From repo root — apply the migration to the seed database
.\eng\apply-conversation-migration.ps1
```

This adds `ConversationId` to `SurveyResponse` and `PasswordHash` to `ApplicationUser`.

### 2. Update Connection String for Development

In `InquirySpark.Admin/appsettings.Development.json`, ensure `ReadWriteCreate` mode:

```json
{
  "ConnectionStrings": {
    "InquirySparkConnection": "Data Source=../../data/sqlite/InquirySpark.db;Mode=ReadWriteCreate"
  }
}
```

### 3. Build and Run

```powershell
dotnet build InquirySpark.Admin/InquirySpark.Admin.csproj
dotnet run --project InquirySpark.Admin
```

The API will be available at `https://localhost:7001/api/v1/conversation/`.

## API Walkthrough

### Step 1: Discover Surveys

```bash
curl -X POST https://localhost:7001/api/v1/conversation/start \
  -H "Content-Type: application/json" \
  -d '{
    "account_name": "testuser",
    "password": "testpassword",
    "application_id": 1
  }'
```

Response: list of available surveys (`action_type: "survey_selection"`).

### Step 2: Start a Survey

```bash
curl -X POST https://localhost:7001/api/v1/conversation/start \
  -H "Content-Type: application/json" \
  -d '{
    "account_name": "testuser",
    "password": "testpassword",
    "application_id": 1,
    "survey_id": 5
  }'
```

Response: first question with `next_url`.

### Step 3: Answer Questions (Follow HATEOAS Links)

```bash
# Use the next_url from the previous response
curl -X POST https://localhost:7001/api/v1/conversation/next/{conversation_id}/{question_id} \
  -H "Content-Type: application/json" \
  -d '{
    "question_answer_id": 11
  }'
```

Repeat until `conversation_ended: true`.

### Step 4: Resume an Interrupted Conversation

```bash
curl -X POST https://localhost:7001/api/v1/conversation/start \
  -H "Content-Type: application/json" \
  -d '{
    "account_name": "testuser",
    "password": "testpassword",
    "application_id": 1,
    "conversation_id": "7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8"
  }'
```

Returns the first unanswered question.

### Step 5: Restart a Conversation

```bash
curl -X POST https://localhost:7001/api/v1/conversation/start \
  -H "Content-Type: application/json" \
  -d '{
    "account_name": "testuser",
    "password": "testpassword",
    "application_id": 1,
    "conversation_id": "7f4f0f6f-8f9d-4ec4-9c77-48b574ee73e8",
    "action": "restart"
  }'
```

Deletes all previous answers, returns question 1.

## Testing

```powershell
# Run all tests
dotnet test

# Run conversation-specific integration tests
dotnet test --filter "FullyQualifiedName~Conversation"
```

## Key Files

| File | Purpose |
|------|---------|
| `InquirySpark.Repository/Services/IConversationService.cs` | Service interface |
| `InquirySpark.Repository/Services/ConversationService.cs` | Service implementation |
| `InquirySpark.Admin/Controllers/Api/ConversationController.cs` | API controller |
| `InquirySpark.Repository/Models/ConversationEnvelope.cs` | Response DTO |
| `InquirySpark.Repository/Database/SurveyResponse.cs` | +ConversationId |
| `InquirySpark.Repository/Database/ApplicationUser.cs` | +PasswordHash |

## Swagger

The endpoints will appear in Swagger UI at `https://localhost:7001/swagger` under the **Conversation** tag.
