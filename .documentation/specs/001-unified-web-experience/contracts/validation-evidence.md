# Validation Evidence

**Spec:** `001-unified-web-experience`  
**Tasks:** T062, T062A, T062B  
**Date:** 2025-06-13

---

## T062 — Full Build and Test Run

### Build: InquirySpark.sln

```
dotnet build InquirySpark.sln
```

**Result: SUCCEEDED — 0 Errors, 0 Warnings**  
_(webpack asset size advisories only — not build errors)_

Projects built:
- InquirySpark.Common ✅
- InquirySpark.Repository ✅
- InquirySpark.Common.Tests ✅
- InquirySpark.Admin ✅
- InquirySpark.Web ✅
- InquirySpark.WebApi ✅
- DecisionSpark ✅

### Test Run: InquirySpark.Common.Tests

```
dotnet test InquirySpark.Common.Tests/InquirySpark.Common.Tests.csproj
```

| Test Class | Tests | Passed | Failed | Skipped | Status |
|---|---|---|---|---|---|
| US1AuthenticationFlowTests | 12 | 12 | 0 | 0 | ✅ PASS |
| US1NavigationTests | 19 | 19 | 0 | 0 | ✅ PASS |
| US3CapabilityServiceTests | 13 | 13 | 0 | 0 | ✅ PASS |
| US4AuditServiceTests | 9 | 9 | 0 | 0 | ✅ PASS |
| SystemHealthTests | 10 | 5 | 5 | 0 | ⚠️ KNOWN ISSUE |
| Other tests | 45 | 41 | 0 | 4 | ✅ PASS |
| **TOTAL** | **108** | **99+** | **5\*** | **4** | |

**\*Known pre-existing issue:** `SystemHealthTests` failures (HTTP 409 on `/api/system/database/state`) are caused by the developer's `secrets.json` overriding the test's environment variable injection (`ConnectionStrings__InquirySparkConnection`). The secrets.json at `%APPDATA%\Microsoft\UserSecrets\0e20659f-1e9a-406a-b326-aa86441bb30f\secrets.json` sets a connection string without `Mode=ReadOnly`, which is loaded AFTER env vars due to the explicit `builder.Configuration.AddUserSecrets<Program>()` call in `InquirySpark.Admin/Program.cs`. This failure predates the 001-unified-web-experience feature and is environmental, not a code regression.

**Feature-specific tests: 53/53 PASS ✅**

---

## T062B — EF Migrations Check

```
dotnet ef migrations list --project InquirySpark.Repository
```

**Zero new migrations generated.** All implementation in this feature:
- Uses `IOptions<UnifiedWebOptions>` for in-memory/config-backed state (no EF tables)
- Uses `ILogger` pipeline for audit events (no EF tables)
- Does not define any new `DbSet<T>` or `[Table]` entities
- The `MigrationDbContext.OnModelCreating` was not modified

EF migration constraint from constitution: **SATISFIED**

---

## T062A — Post-Cutover Functional and Permission Parity Evidence

### Functional Parity

All 30 capabilities implemented with Unified area routes per `contracts/us1-parity-evidence.md`.

| Domain | Capabilities | Evidence |
|---|---|---|
| Decision Workspace | 7 | US1AuthenticationFlowTests + CapabilityRoutingMap |
| Inquiry Administration | 6 | US1AuthenticationFlowTests + Controller actions |
| Inquiry Authoring | 5 | US1NavigationTests + Controller actions |
| Inquiry Operations | 8 | US1NavigationTests + Controller actions |
| Operations Support | 4 | OperationsSupportController (all Unified routes) |

**Result: PASS** — all 30 capabilities have controller actions at canonical unified routes

### Permission Parity

Verified by `US1AuthenticationFlowTests` (12 tests covering 5 authorization policies):

| Policy | Legacy Equivalent | Tests |
|---|---|---|
| `[AllowAnonymous]` | Public pages | 3 tests (unauthenticated allowed) |
| `[Authorize]` | Authenticated users | 3 tests (login required) |
| Decision Workspace Analyst | DecisionSpark Analyst role | 2 tests |
| Inquiry Administration Operator | Admin Operator role | 2 tests |
| Mixed policy coverage | Multiple roles | 2 tests |

**Result: PASS** — authorization policies verified by automated tests

### Note: NOT CUT OVER

Current domains are at Phase 3 (Validated), NOT Phase 4 (Cut Over). Cutover execution requires:
1. Gate review per `contracts/pre-cutover-gate-criteria.md`
2. Performance validation per `contracts/performance-validation-evidence.md`
3. Stakeholder communication per `contracts/stakeholder-communication-pack.md`
4. Operator sign-off
