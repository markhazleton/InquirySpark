# Pull Request Review: Feature: HATEOAS Conversation API

```yaml
gate: pr-review
status: fail
blocking: true
severity: error
summary: "Branch sync and prior API blockers are fixed, but migration scope still violates feature schema constraints."
```

## Review Metadata

- **PR Number**: #4
- **Source Branch**: 002-hateoas-conversation-api
- **Target Branch**: main
- **Review Date**: 2026-04-12 00:52:00 UTC
- **Last Updated**: 2026-04-12 00:52:00 UTC
- **Reviewed Commit**: fef5eaf70b9462ac3dcf26590b0c53b7546a136c
- **Reviewer**: devspark.pr-review
- **Constitution Version**: 1.1.0

## PR Summary

- **Author**: @markhazleton
- **Created**: 2026-04-12T00:32:16Z
- **Status**: OPEN
- **Files Changed**: 43
- **Commits**: 12
- **Lines**: +16001 -1

## Executive Summary

- ✅ **Constitution Compliance**: FAIL (4/5 principles checked as pass)
- 📋 **Spec Lifecycle**: Complete
- 📝 **Task Completion**: 54/54 tasks complete
- 🔒 **Security**: 0 issues found
- 📊 **Code Quality**: 2 recommendations
- 🧪 **Testing**: FAIL
- 📝 **Documentation**: PASS

**Overall Assessment**: The branch is in sync with `main`, and previously reported runtime blockers in HATEOAS navigation and free-text persistence are fixed in this head commit. One blocking issue remains: the migration scope introduces broad schema creation that conflicts with the feature requirement to reuse existing schema with only targeted column additions.

**Approval Recommendation**: ⚠️ REQUEST CHANGES
*Note: APPROVE is blocked while critical issues are unresolved.*

## Critical Issues (Blocking)

| ID | Principle | File:Line | Issue | Recommendation |
|----|-----------|-----------|-------|----------------|
| C1 | Spec Lifecycle Contract (FR-010 MUST) | InquirySpark.Repository/Migrations/20260409043650_AddConversationApi.cs:14 | The feature migration creates a full schema (`CreateTable(...)` repeated across many entities) instead of limiting the change to conversation-specific schema deltas. This violates FR-010: reuse existing schema and avoid new tables for this feature. | Replace this migration with a targeted migration that only applies required conversation changes (e.g., `SurveyResponse.ConversationId`, `ApplicationUser.PasswordHash`, and any explicitly approved column updates). Keep unrelated table creation out of this PR. |

## High Priority Issues

| ID | Principle | File:Line | Issue | Recommendation |
|----|-----------|-----------|-------|----------------|
| H1 | Development Workflow & Quality Gates | InquirySpark.Common.Tests/Integration/ConversationApiTests.cs:65 | All integration tests remain `[Ignore]`, so conversation regressions are not CI-gated. | Enable at least one deterministic non-ignored test path for start -> next -> completion semantics. |
| H2 | Engineering Constraints (SQLite Asset Immutability) | data/sqlite/conversation-dev.db:1 | A mutable SQLite asset is included in PR changes, which is high-risk for reproducibility and conflicts with immutable DB asset guidance. | Remove mutable DB artifact changes from the PR and keep migration/seed flow reproducible through scripts only. |

## Medium Priority Suggestions

| ID | Principle | File:Line | Issue | Recommendation |
|----|-----------|-----------|-------|----------------|
| M1 | Testing Accuracy | InquirySpark.Common.Tests/Integration/ConversationApiTests.cs:83 | Test assertions mix camelCase and snake_case JSON field names (for example `actionType`, `conversationId`, `nextUrl`), which does not align with the API contract DTO annotations and can hide contract drift. | Normalize test JSON assertions to the published contract field names and add one contract assertion set that fails on casing regressions. |

## Low Priority Improvements

None found.

## Constitution Alignment Details

| Principle | Status | Evidence | Notes |
|-----------|--------|----------|-------|
| I. Response Wrapper Consistency | ✅ Pass | InquirySpark.Admin/Controllers/Api/ConversationController.cs:41 | Controller now routes `BaseResponse<T>` through `ApiResponseHelper`. |
| II. Dependency Injection Discipline | ✅ Pass | InquirySpark.Admin/Program.cs:73 | `IConversationService` is registered in DI and injected via constructor. |
| III. EF Core Context Stewardship | ❌ Fail | InquirySpark.Repository/Migrations/20260409043650_AddConversationApi.cs:14 | Migration scope is broader than feature-intended schema evolution. |
| IV. Admin UI Standardization | ⏭️ N/A | - | PR is API/service/data-focused, not Admin CRUD UI templating. |
| V. Documentation & Knowledge Flow | ✅ Pass | .documentation/specs/002-hateoas-conversation-api/spec.md:4 | Spec is under `/.documentation/` and marked `Complete`. |

## Security Checklist

- [x] No hardcoded secrets or credentials
- [x] Input validation present where needed
- [x] Authentication/authorization checks appropriate
- [x] No SQL injection vulnerabilities
- [x] No XSS vulnerabilities
- [x] Dependencies reviewed for vulnerabilities

Validation notes:
- No credential leakage or dynamic SQL risks observed in reviewed Conversation API scope.

## Code Quality Assessment

### Strengths
- Prior blocking defects from the earlier review are resolved in current head commit:
  - `next_url` is now consistently emitted for question steps via `BuildEnvelope(...)`.
  - Free-text persistence now uses a schema-compatible answer option strategy via `GetOrCreateFreeTextAnswerIdAsync(...)`.
- DTO wire-format annotations now align with snake_case contract fields in Conversation models.

### Areas for Improvement
- Narrow migration scope to avoid broad schema churn in a feature-specific PR.
- Activate at least one runnable integration or service-level regression test in CI.

## Testing Coverage

**Status**: INADEQUATE

Conversation integration tests remain ignored, so contract/flow regressions are not automatically detected during CI.

## Documentation Status

**Status**: ADEQUATE

Feature documentation is present and organized correctly under `/.documentation/specs/002-hateoas-conversation-api/`.

## Changed Files Summary

| File | Changes | Type | Constitution Issues |
|------|---------|------|---------------------|
| InquirySpark.Repository/Migrations/20260409043650_AddConversationApi.cs | +1866 -0 | Added | 1 issue (C1) |
| InquirySpark.Repository/Services/ConversationService.cs | +381 -0 | Added | None (prior blockers fixed) |
| InquirySpark.Admin/Controllers/Api/ConversationController.cs | +74 -0 | Added | None |
| InquirySpark.Common/Models/Conversation*.cs | +200+ | Added | None |
| InquirySpark.Common.Tests/Integration/ConversationApiTests.cs | +236 -0 | Added | 2 issues (H1, M1) |
| data/sqlite/conversation-dev.db | binary | Modified | 1 issue (H2) |

## Detailed Findings by File

### InquirySpark.Repository/Migrations/20260409043650_AddConversationApi.cs

**Lines 14+**: Migration performs broad schema creation.

```csharp
migrationBuilder.CreateTable(
    name: "AuditLog",
    columns: table => new
```

- **Principle Violated**: Feature requirement FR-010 (`MUST` reuse existing schema, no new tables for this feature)
- **Severity**: CRITICAL
- **Recommendation**: Replace with focused schema delta migration for conversation requirements only.

### InquirySpark.Common.Tests/Integration/ConversationApiTests.cs

**Lines 65, 89, 110, 143**: Tests are disabled.

```csharp
[Ignore("Requires writable conversation-dev.db — run eng/apply-conversation-migration.ps1")]
```

- **Principle Violated**: Development workflow quality gate intent
- **Severity**: HIGH
- **Recommendation**: Enable at least one deterministic test in CI.

**Lines 83, 123, 162**: Assertions use non-contract property casing in places.

```csharp
.GetProperty("actionType")
.GetProperty("conversationId")
```

- **Principle Violated**: Contract verification rigor
- **Severity**: MEDIUM
- **Recommendation**: Assert against the documented snake_case response fields consistently.

## Next Steps

### Immediate Actions (Required)

- [ ] Replace broad migration with targeted conversation-only schema migration (C1)

### Recommended Improvements

- [ ] Un-ignore at least one deterministic conversation integration test (H1)
- [ ] Remove mutable SQLite artifact from PR changes (H2)
- [ ] Align JSON assertion casing in tests to contract fields (M1)

### Future Considerations (Optional)

- [ ] Add contract-level tests that compare OpenAPI schema names to runtime JSON payload names.

## Approval Decision

**Recommendation**: ⚠️ REQUEST CHANGES

**Reasoning**:
Core Conversation API correctness defects identified in the previous review are fixed, and branch sync is valid. However, the migration still exceeds allowed feature scope by creating broad schema tables, which is a blocking requirement mismatch for this spec.

**Estimated Rework Time**: 4-8 hours

---

*Review generated by devspark.pr-review v1.0*
*Constitution-driven code review for InquirySpark*
*To update this review after changes: /devspark.pr-review #4*

---

## Previous Review History

### Review 1: 2026-04-12 00:37:12 UTC

**Commit**: 49a63f475e48889656aaca51e4d462dda5fe6102

Gate summary:

```yaml
gate: pr-review
status: fail
blocking: true
severity: error
summary: "Blocking correctness issues found in HATEOAS navigation and free-text persistence; spec lifecycle is complete."
```

Prior blocking findings:
- C1: `next_url` became null at last question step.
- C2: Free-text persistence used invalid `QuestionAnswerId = 0` path.

Current status of prior blockers:
- ✅ Fixed in reviewed commit `fef5eaf70b9462ac3dcf26590b0c53b7546a136c`.
