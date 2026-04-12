# Implementation Plan: HATEOAS Conversation API

**Branch**: `002-hateoas-conversation-api` | **Date**: 2026-04-08 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/002-hateoas-conversation-api/spec.md`

## Summary

Add a RESTful, HATEOAS-driven Conversation API to `InquirySpark.Admin` under `/api/v1/conversation/` that walks users through a survey one question at a time. Two endpoints — `POST /start` (authenticate, begin or resume a survey) and `POST /next/{conversation_id}/{question_id}` (submit answer, advance) — return a uniform JSON envelope containing the current action, HATEOAS links (`next_url`, `prev_url`), and conversation state. The implementation adds a GUID `ConversationId` column to `SurveyResponse`, introduces a `PasswordHash` field on `ApplicationUser` with ASP.NET Core Identity `PasswordHasher<T>` verification, and creates a new `IConversationService` in the Repository layer with a thin API controller in Admin.

## Technical Context

**Language/Version**: C# / .NET 10
**Primary Dependencies**: ASP.NET Core MVC, EF Core (SQLite provider), Microsoft.AspNetCore.Identity (PasswordHasher only — not full Identity framework), Swashbuckle.AspNetCore (for API documentation)
**Storage**: SQLite — `InquirySpark.db` (ReadWriteCreate in non-dev; ReadOnly in Development)
**Testing**: `dotnet test` — InquirySpark.Common.Tests (existing); new integration tests targeting the Conversation API
**Target Platform**: Linux/Windows server (Kestrel)
**Project Type**: Web — existing multi-project .NET solution
**Performance Goals**: Typical internal survey usage; no high-throughput requirements
**Constraints**: SQLite single-writer model; no concurrent multi-user write pressure expected. Development environment is read-only — tests and local dev require `ReadWriteCreate` mode for conversation writes
**Scale/Scope**: 2 new endpoints, 1 new service, 1 new controller, 2 schema columns added to existing tables, ~15 request/response DTOs, plus Swagger documentation enabled.

## Strategy: Thin Admin, Thick Common/Repository

To comply with the request to keep the `Admin` project strictly thin, the following architectural boundaries are strictly held:
- **InquirySpark.Admin**: Restricted entirely to standard configuration (DI, Swagger) and an ultra-thin controller (`ConversationController`). The controller contains zero business logic, performing only a direct pass-through query to the `IConversationService` mapping the response automatically via the existing `ApiResponseHelper.ExecuteAsync`.
- **InquirySpark.Common**: Houses all portable DTOs (`ConversationEnvelope`, arguments), Enums (`SurveyResponseStatus`), and domain-agnostic helpers. This ensures any future client or cross-cutting project can reference the API definitions seamlessly without pulling in EF Core or heavy business logic.
- **InquirySpark.Repository**: Houses the core domain logic, schema modifications, database context (`InquirySparkContext`), mapping translation between DB entities and portable DTOs, and the core `ConversationService`. 

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| # | Rule | Status | Notes |
|---|------|--------|-------|
| I | Response Wrapper Consistency | ✅ PASS | `IConversationService` will return `BaseResponse<ConversationEnvelope>` through `DbContextHelper.ExecuteAsync` |
| II | Dependency Injection Discipline | ✅ PASS | `ConversationService` registered via `AddScoped` in `Program.cs`; primary constructor injection |
| III | EF Core Context Stewardship | ⚠️ CONDITIONAL | Adding `ConversationId` (GUID) to `SurveyResponse` and `PasswordHash` to `ApplicationUser` requires schema change. See **Complexity Tracking** below |
| IV | Admin UI Standardization | ✅ N/A | Feature is API-only; no admin views |
| V | Documentation & Knowledge Flow | ✅ PASS | All docs under `/.documentation/specs/002-hateoas-conversation-api/` |
| — | No new tables | ✅ PASS | Spec FR-010: no new tables; only columns added to existing entities |
| — | SQLite exclusive | ✅ PASS | No SQL Server references |
| — | XML doc comments on public APIs | ✅ PASS | All new public classes/methods will have XML docs |
| — | Build with zero warnings | ✅ PASS | Will validate with `dotnet build -warnaserror` |

### Gate Resolution: Schema Change vs. Immutable `.db` Assets

The constitution states *"SQLite `.db` assets are immutable — `Database.Migrate()` is disabled."* However:

1. **`appsettings.json` (non-dev)** uses `Mode=ReadWriteCreate` — the production database IS writable.
2. **`appsettings.Development.json`** uses `Mode=ReadOnly` — development is read-only for safety.
3. The feature spec FR-010 explicitly requires EF Core Migrations for schema changes.

**Resolution**: The immutability rule protects the **committed seed `.db` files** in `data/sqlite/` from accidental corruption. Because the Conversation API requires write operations to test, we will use a separate, ephemeral writable copy of the database during development to adhere strictly to the constitution.

- Creating EF Core migrations for the two new columns
- Tests and local development will point a separate copied writable SQLite database (e.g. `conversation-dev.db`) instead of making the seed `.db` file writable.

This approach respects the constitution's intent (no accidental runtime schema drift) while enabling the spec-required schema evolution.

## Project Structure

### Documentation (this feature)

```text
.documentation/specs/002-hateoas-conversation-api/
├── plan.md              # This file
├── research.md          # Phase 0: Research findings
├── data-model.md        # Phase 1: Entity changes & DTOs
├── quickstart.md        # Phase 1: Developer quickstart guide
├── contracts/           # Phase 1: API contract definitions
│   └── conversation-api.md
└── tasks.md             # Phase 2: Task breakdown (generated by /devspark.tasks)
```

### Source Code (repository root)

```text
InquirySpark.Common/
├── Models/
│   ├── ConversationEnvelope.cs    # Response envelope DTO (new)
│   ├── ConversationStartRequest.cs # Start request DTO (new)
│   ├── ConversationNextRequest.cs  # Next request DTO (new)
│   ├── ConversationAction.cs       # Action sub-object DTO (new)
│   ├── ConversationQuestion.cs     # Question sub-object DTO (new)
│   ├── ConversationSurveyOption.cs # Survey list item DTO (new)
│   └── SurveyResponseStatus.cs     # Enum mapping to LuSurveyResponseStatus (new)

InquirySpark.Repository/
├── Database/
│   ├── SurveyResponse.cs          # +ConversationId (Guid) property
│   ├── ApplicationUser.cs         # +PasswordHash (string) property
│   └── InquirySparkContext.cs     # +OnModelCreating index for ConversationId
├── Services/
│   ├── IConversationService.cs    # Service interface (new)
│   ├── ConversationService.cs     # Service implementation (new)
│   └── ConversationService_Mappers.cs # Entity→DTO mappers (new)
├── Migrations/                    # EF Core migration files (new directory)

InquirySpark.Admin/
├── Controllers/
│   └── Api/
│       └── ConversationController.cs  # API controller (new)
├── Program.cs                          # +DI registration for IConversationService

InquirySpark.Common.Tests/
├── Integration/
│   └── ConversationApiTests.cs    # Integration tests (new)
```

**Structure Decision**: Follows the existing pattern — entity changes in `Repository/Database/`, DTOs in `Common/Models/`, service + interface in `Repository/Services/`, thin controller in `Admin/Controllers/Api/`. No new projects needed.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Schema change to immutable `.db` | Spec FR-010 requires `ConversationId` on `SurveyResponse` and `PasswordHash` on `ApplicationUser` | Cannot use a separate table (FR-010 prohibits new tables); cannot use in-memory-only tracking (conversation must survive server restarts) |
| EF Core Migrations directory | Two new columns require tracked migrations | Manual `ALTER TABLE` scripts are fragile and not EF-model-aware; migrations provide idempotent, versioned schema evolution |
| Development connection string change | Conversation API writes answers to the database | Read-only mode blocks all INSERT/UPDATE operations, so we will generate a temporary writable `.db` copy specifically for this feature. |
└── [platform-specific structure: feature modules, UI flows, platform tests]
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
