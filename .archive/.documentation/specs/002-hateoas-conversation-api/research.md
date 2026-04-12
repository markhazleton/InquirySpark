# Research: HATEOAS Conversation API

**Feature**: 002-hateoas-conversation-api | **Date**: 2026-04-08

## R1: Schema Evolution Strategy (SQLite Immutable `.db` Conflict)

**Decision**: Use EF Core Migrations to add columns, applied via a one-time offline script — not `Database.Migrate()` at runtime.

**Rationale**: The constitution declares SQLite `.db` assets immutable and disables `Database.Migrate()`. However, the feature spec FR-010 mandates EF Core Migrations for the two new columns. The conflict is resolved by:

1. Adding EF Core migration files to the Repository project (code artifacts, committed to git)
2. Providing a PowerShell migration script (`eng/apply-conversation-migration.ps1`) that:
   - Copies the seed `.db` to a working copy
   - Runs `dotnet ef database update` against the copy
   - Replaces the seed file after validation
3. Never calling `Database.Migrate()` in application startup code

**Alternatives considered**:
- Manual `ALTER TABLE` SQL scripts: Rejected — not EF-model-aware; risk of drift between code model and schema
- Separate writable database for conversations: Rejected — violates FR-010 (no new tables/databases) and fragmenting data across databases complicates queries

## R2: Password Hashing with ASP.NET Core Identity PasswordHasher

**Decision**: Add a `PasswordHash` column to `ApplicationUser`. Use `PasswordHasher<ApplicationUser>` from `Microsoft.AspNetCore.Identity` for hashing and verification. Do NOT adopt full ASP.NET Core Identity framework.

**Rationale**: The spec (FR-001) requires `PasswordHasher` specifically. The existing `Password` field stores plaintext. The approach is:

1. Add `PasswordHash` (nullable string) to `ApplicationUser` entity
2. At authentication time in `ConversationService`:
   - If `PasswordHash` is non-null: verify using `PasswordHasher<ApplicationUser>.VerifyHashedPassword()`
   - If `PasswordHash` is null but `Password` matches plaintext: hash the password, store in `PasswordHash`, clear `Password` (lazy migration)
3. `Microsoft.AspNetCore.Identity` is already a transitive dependency (Admin uses Identity for cookie auth). Only `PasswordHasher<T>` is needed — no `UserManager`, `SignInManager`, or Identity tables.

**Alternatives considered**:
- Full Identity migration for ApplicationUser: Rejected — massive scope creep, requires Identity tables, disrupts existing admin auth
- BCrypt or custom hasher: Rejected — spec explicitly names PasswordHasher; using ASP.NET Core's built-in is idiomatic and auditable
- Separate auth table: Rejected — FR-010 prohibits new tables

## R3: SurveyResponseAnswer Access Pattern (No Direct Navigation)

**Decision**: Query `SurveyResponseAnswer` directly by `SurveyResponseId` using `_context.SurveyResponseAnswers.Where(a => a.SurveyResponseId == responseId)` rather than navigating through `SurveyResponseSequence`.

**Rationale**: The existing data model routes answers through `SurveyResponseSequence` (composite FK: `SurveyResponseId + SequenceNumber`). However:

1. `SurveyResponseAnswer` has a direct `SurveyResponseId` column — EF Core can query it directly
2. The Conversation API needs to: (a) check if a question is already answered, (b) upsert an answer, (c) delete all answers for a restart
3. Going through `SurveyResponseSequence` adds unnecessary complexity for these operations
4. A direct `SurveyResponse.SurveyResponseAnswers` navigation property is NOT required; LINQ queries on the `SurveyResponseAnswers` DbSet suffice

**Implementation**: Use explicit LINQ queries — no entity model changes for navigation.

## R4: Question Ordering — Deterministic Sequence

**Decision**: Order questions by `QuestionGroup.GroupOrder ASC`, then `QuestionGroupMember.DisplayOrder ASC` within each group.

**Rationale**: The spec (FR-013, SC-005) requires deterministic ordering. The existing entities provide:

- `QuestionGroup.GroupOrder` — int, group-level ordering
- `QuestionGroupMember.DisplayOrder` — int, question-within-group ordering
- `QuestionGroupMember` links `QuestionGroupId` → `QuestionId`

The full ordering query:
```csharp
survey.QuestionGroups
    .OrderBy(g => g.GroupOrder)
    .SelectMany(g => g.QuestionGroupMembers.OrderBy(m => m.DisplayOrder))
    .Select(m => m.Question)
```

**Alternatives considered**: Using `Question.QuestionSort` — rejected because it's question-global, not survey-scoped. The group/member ordering is survey-specific and explicitly referenced by the spec.

## R5: StatusId Values for In-Progress and Completed

**Decision**: Use `StatusId = 1` for in-progress; determine completed status from `LuSurveyResponseStatus` table at runtime by following the `NextStatusId` chain, or hardcode if seed data confirms a specific value.

**Rationale**: From stored procedures and reset logic, `StatusId = 1` is definitively the in-progress status. The completed status requires inspection of the `LuSurveyResponseStatus` seed data:

- The entity has `PreviousStatusId` and `NextStatusId` fields, forming a status flow chain
- Stored procedures reference statuses 1, 2, 3, and 5
- The implementation should query `LuSurveyResponseStatus` by name (e.g., "Complete" or "Completed") at service startup, or define constants once the seed data is verified

**Action**: During implementation, inspect the seed `.db` to confirm the exact `StatusId` for "Completed". If not determinable, add a `StatusName` lookup in the service constructor.

## R6: ConversationId GUID Generation and Indexing

**Decision**: Generate `ConversationId` as `Guid.NewGuid()` when creating a new `SurveyResponse`. Add a unique index on `SurveyResponse.ConversationId` in `OnModelCreating`.

**Rationale**: `ConversationId` is the public-facing identifier in all API URLs. It must be:

- Globally unique (GUID satisfies this)
- Indexed for fast lookups (every `/next` call queries by it)
- Non-guessable (GUIDs are not sequential integers)

SQLite stores GUIDs as TEXT (16-byte BLOB is also possible but TEXT is EF Core's default for SQLite). The unique index ensures no collisions and enables efficient `WHERE ConversationId = @id` queries.

## R7: Anonymous Endpoint Configuration

**Decision**: Decorate the `ConversationController` with `[AllowAnonymous]` at the controller level. Do not apply `[Authorize]` policy.

**Rationale**: The spec (FR-015) explicitly states:
- Endpoints MUST be accessible without ASP.NET Core Identity session/cookie
- `/start` authenticates via request body (`account_name` + `password`)
- `/next` is authorized by `conversation_id` possession only

The Admin app uses ASP.NET Core Identity with cookie auth and role-based policies. The Conversation API must bypass this entirely. `[AllowAnonymous]` on the controller prevents the global authentication middleware from rejecting unauthenticated requests.

## R8: Idempotent Answer Upsert and Step-Back Logic

**Decision**: When answering a question, check for existing `SurveyResponseAnswer` by `(SurveyResponseId, QuestionId)`. If found, update it. If answering a previous question, delete all answers for subsequent questions.

**Rationale**: The spec edge case says "the second submission overwrites the first (idempotent update)" and "any existing answers for subsequent questions MUST be deleted." The implementation:

1. Find existing answer: `_context.SurveyResponseAnswers.FirstOrDefault(a => a.SurveyResponseId == id && a.QuestionId == qId)`
2. If exists: update `QuestionAnswerId`/`AnswerComment`; delete answers for all questions that come after this one in the ordered sequence
3. If not exists: insert new `SurveyResponseAnswer`
4. Return the next question in sequence

The "1 step forward or 1 step back" constraint means the API validates that `question_id` is either the current unanswered question or the immediately preceding answered question.

## R9: Development Environment Write Access

**Decision**: The feature branch will require a `ReadWriteCreate` connection string for local development and testing. Update `appsettings.Development.json` or provide a dedicated `appsettings.Conversation.json` profile.

**Rationale**: The current Development config uses `Mode=ReadOnly`, which blocks all INSERT/UPDATE operations. The Conversation API fundamentally requires writes. Options:

1. Change `appsettings.Development.json` to `ReadWriteCreate` — simple but changes behavior for all developers
2. Add a `ASPNETCORE_ENVIRONMENT=ConversationDev` profile — isolated but adds complexity
3. Use `eng/sqlite.env.example` pattern with environment-specific overrides

**Recommendation**: Option 1 is simplest and aligns with the non-dev config already being `ReadWriteCreate`. The read-only restriction was a safety measure, not a hard requirement. Document the change in the feature branch PR.
