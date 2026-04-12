# Pull Request Review: Feature: HATEOAS Conversation API

```yaml
gate: pr-review
status: fail
blocking: true
severity: error
summary: "Blocking correctness issues found in HATEOAS navigation and free-text persistence; spec lifecycle is complete."
```

## Review Metadata

- **PR Number**: #4
- **Source Branch**: 002-hateoas-conversation-api
- **Target Branch**: main
- **Review Date**: 2026-04-12 00:37:12 UTC
- **Last Updated**: 2026-04-12 00:37:12 UTC
- **Reviewed Commit**: 49a63f475e48889656aaca51e4d462dda5fe6102
- **Reviewer**: devspark.pr-review
- **Constitution Version**: 1.1.0

## PR Summary

- **Author**: @markhazleton
- **Created**: 2026-04-12T00:32:16Z
- **Status**: OPEN
- **Files Changed**: 38
- **Commits**: 10
- **Lines**: +15501 -1

## Executive Summary

- ✅ **Constitution Compliance**: FAIL (3/5 principles checked as pass)
- 📋 **Spec Lifecycle**: Complete
- 📝 **Task Completion**: 54/54 tasks complete
- 🔒 **Security**: 1 issues found
- 📊 **Code Quality**: 2 recommendations
- 🧪 **Testing**: FAIL
- 📝 **Documentation**: PASS

**Overall Assessment**: The branch is in sync with main and lifecycle artifacts are complete (spec complete, tasks/checklist complete), but two blocking implementation defects break required conversation behavior and free-text answer persistence. Additional contract and consistency issues should be addressed before merge.

**Approval Recommendation**: ❌ REJECT
*Note: APPROVE is blocked if Spec Lifecycle is not Complete or tasks are incomplete for feature branches.*

## Critical Issues (Blocking)

| ID | Principle | File:Line | Issue | Recommendation |
|----|-----------|-----------|-------|----------------|
| C1 | Spec Contract (FR-012 / FR-004) | InquirySpark.Repository/Services/ConversationService.cs:342 | `next_url` is set to `null` on the last question envelope before the final answer is submitted. A thin client following links cannot submit the last answer. | Always emit a postable link for the current question step; only omit `next_url` on completion envelope (`conversation_ended: true`). |
| C2 | III. EF Core Context Stewardship | InquirySpark.Repository/Services/ConversationService.cs:109 | Free-text answer path writes `QuestionAnswerId = 0` for required FK `SurveyResponseAnswer.QuestionAnswerId`, risking FK violations and 500 errors instead of valid free-text persistence. | Persist free-text answers with a valid strategy compatible with schema constraints (e.g., nullable FK via migration or canonical free-text option row) and enforce validation accordingly. |

## High Priority Issues

| ID | Principle | File:Line | Issue | Recommendation |
|----|-----------|-----------|-------|----------------|
| H1 | API Contract Fidelity (Spec FR-004, request schema) | InquirySpark.Admin/Program.cs:79 | API models are PascalCase (`AccountName`, `ApplicationId`, etc.) and no snake_case serializer configuration is registered; this conflicts with the documented `account_name`, `application_id`, `conversation_id`, `next_url` contract. | Configure explicit JSON naming (or `[JsonPropertyName]` attributes) to match the published contract and Swagger examples. |
| H2 | I. Response Wrapper Consistency | InquirySpark.Admin/Controllers/Api/ConversationController.cs:76 | Controller introduces custom `MapResult` parsing instead of using the existing API response helper pattern, creating divergent error-shaping behavior. | Route `BaseResponse<T>` through shared API helper to keep consistent status/error mapping across controllers. |

## Medium Priority Suggestions

| ID | Principle | File:Line | Issue | Recommendation |
|----|-----------|-----------|-------|----------------|
| M1 | Development Workflow Quality Gate | InquirySpark.Common.Tests/Integration/ConversationApiTests.cs:58 | Conversation integration tests are all `[Ignore]`, so critical flow regressions (like C1/C2) are not gated. | Enable at least one CI-runnable integration test (or deterministic service-level tests) covering start -> next -> completion and free-text path. |

## Low Priority Improvements

None found.

## Constitution Alignment Details

| Principle | Status | Evidence | Notes |
|-----------|--------|----------|-------|
| I. Response Wrapper Consistency | ❌ Fail | InquirySpark.Admin/Controllers/Api/ConversationController.cs:76 | Custom controller mapping diverges from shared helper pattern. |
| II. Dependency Injection Discipline | ✅ Pass | InquirySpark.Admin/Program.cs | `IConversationService` registered via DI and injected into controller/service constructors. |
| III. EF Core Context Stewardship | ❌ Fail | InquirySpark.Repository/Services/ConversationService.cs:109 | Answer persistence path conflicts with required FK behavior for free-text cases. |
| IV. Admin UI Standardization | ⏭️ N/A | - | PR scope is API/service/data-layer, not Admin CRUD UI templating. |
| V. Documentation & Knowledge Flow | ✅ Pass | .documentation/specs/002-hateoas-conversation-api/* | Spec, tasks, and checklist artifacts are maintained under `/.documentation/`. |

## Security Checklist

- [x] No hardcoded secrets or credentials
- [ ] Input validation present where needed
- [x] Authentication/authorization checks appropriate
- [x] No SQL injection vulnerabilities
- [x] No XSS vulnerabilities
- [x] Dependencies reviewed for vulnerabilities

Validation notes:
- Input validation gap observed in answer persistence semantics for free-text path (C2).
- No direct secret leakage or unsafe dynamic SQL observed in reviewed scope.

## Code Quality Assessment

### Strengths
- Service methods are wrapped with `DbContextHelper.ExecuteAsync`, aligning with repository response patterns.
- Spec lifecycle artifacts are complete and traceable (spec/tasks/checklist all present and complete).

### Areas for Improvement
- HATEOAS link construction should be scenario-tested for last-step behavior.
- API contract naming should be explicitly enforced to avoid client incompatibility.

## Testing Coverage

**Status**: INADEQUATE

All conversation integration tests in `InquirySpark.Common.Tests/Integration/ConversationApiTests.cs` are currently ignored, leaving key interaction paths unguarded.

## Documentation Status

**Status**: ADEQUATE

Feature docs, plan, research, contracts, tasks, and checklist are present in `/.documentation/specs/002-hateoas-conversation-api/`. Spec status is `Complete` and checklist is complete.

## Changed Files Summary

| File | Changes | Type | Constitution Issues |
|------|---------|------|---------------------|
| InquirySpark.Admin/Controllers/Api/ConversationController.cs | +92 -0 | Added | 1 issue (H2) |
| InquirySpark.Admin/Program.cs | +47 -2 | Modified | 1 issue (H1) |
| InquirySpark.Repository/Services/ConversationService.cs | +381 -0 | Added | 2 issues (C1, C2) |
| InquirySpark.Repository/Services/DbContextHelper.cs | +16 -0 | Modified | None |
| InquirySpark.Repository/Database/InquirySparkContext.cs | +12 -0 | Modified | None |
| InquirySpark.Repository/Database/ApplicationUser.cs | +11 -0 | Modified | None |
| InquirySpark.Repository/Database/SurveyResponse.cs | +10 -0 | Modified | None |
| InquirySpark.Common.Tests/Integration/ConversationApiTests.cs | +236 -0 | Added | 1 issue (M1) |
| .documentation/specs/002-hateoas-conversation-api/spec.md | +411 -0 | Added | None |
| .documentation/specs/002-hateoas-conversation-api/tasks.md | +241 -0 | Added | None |

## Detailed Findings by File

### InquirySpark.Repository/Services/ConversationService.cs

**Lines 342-343**: Last-question envelope sets `next_url` to null before completion.

```csharp
var nextUrl = currentIndex + 1 < totalQuestions
    ? $"/api/v1/conversation/next/{conversationId}/{question.QuestionId}"
    : null;
```

- **Principle Violated**: Spec FR-012 / FR-004 (postable link contract)
- **Severity**: CRITICAL
- **Recommendation**: Keep question-step submission link available for every active question; only completion response should omit `next_url`.

**Lines 109 and 119**: Free-text branch writes `QuestionAnswerId = 0` for required FK.

```csharp
existingAnswer.QuestionAnswerId = request!.QuestionAnswerId ?? 0;
// ...
QuestionAnswerId = request!.QuestionAnswerId ?? 0,
```

- **Principle Violated**: III. EF Core Context Stewardship
- **Severity**: CRITICAL
- **Recommendation**: Use schema-compatible free-text persistence (nullable FK with migration or valid sentinel option row) and enforce explicit validation.

### InquirySpark.Admin/Program.cs and InquirySpark.Common/Models/ConversationStartRequest.cs

**Line 79 + DTO property definitions**: Contract uses snake_case, implementation uses default Pascal/camel behavior.

```csharp
builder.Services.AddControllersWithViews();
// DTO fields: AccountName, ApplicationId, SurveyId, ConversationId
```

- **Principle Violated**: API contract fidelity to spec FR-004 and request schema
- **Severity**: HIGH
- **Recommendation**: Add explicit JSON naming policy or per-property `JsonPropertyName` to enforce contract-safe wire format.

### InquirySpark.Admin/Controllers/Api/ConversationController.cs

**Line 76**: Local result mapper diverges from shared response helper pattern.

```csharp
private IActionResult MapResult<T>(BaseResponse<T> result)
```

- **Principle Violated**: I. Response Wrapper Consistency
- **Severity**: HIGH
- **Recommendation**: Reuse shared API response helper to keep behavior consistent across API controllers.

### InquirySpark.Common.Tests/Integration/ConversationApiTests.cs

**Lines 58-193**: All tests are ignored.

```csharp
[Ignore("Requires writable conversation-dev.db — run eng/apply-conversation-migration.ps1")]
```

- **Principle Violated**: Workflow quality gate intent
- **Severity**: MEDIUM
- **Recommendation**: Add at least one non-ignored automated test path for CI.

## Next Steps

### Immediate Actions (Required)

- [ ] Fix HATEOAS final-step link behavior in conversation envelopes (C1)
- [ ] Fix free-text answer persistence to satisfy FK and API behavior (C2)

### Recommended Improvements

- [ ] Align wire JSON contract to documented snake_case payload/response fields (H1)
- [ ] Standardize controller response mapping through shared helper (H2)
- [ ] Enable at least one runnable automated conversation flow test (M1)

### Future Considerations (Optional)

- [ ] Add contract tests validating OpenAPI schema vs runtime JSON payload shapes.
- [ ] Add regression tests around resume/restart and step-back semantics.

## Approval Decision

**Recommendation**: ❌ REJECT

**Reasoning**:
The PR is lifecycle-complete and branch-synced, but it contains blocking behavioral defects that violate required conversation flow semantics and persistence correctness. These issues can cause client dead-ends and server-side persistence failures, so merge should be blocked until corrected.

**Estimated Rework Time**: 1-2 days

---
