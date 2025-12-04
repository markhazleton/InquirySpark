## Tasks: SQL Server Dependency Removal Baseline

**Input**: Design documents from `/specs/001-remove-sql-server/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

## Phase 1: Setup (Shared Infrastructure)

Purpose: establish repository-wide tooling and documentation required before touching application code.

- [ ] T001 Pin .NET SDK 10.0.100 for all contributors at global.json
- [ ] T002 [P] Capture immutable SQLite asset locations, checksums, and distribution policy in docs/copilot/session-2025-12-04/sqlite-data-assets.md
- [ ] T003 [P] Publish provider environment template (connection strings, read-only flags) at eng/sqlite.env.example

---

## Phase 2: Foundational (Blocking Prerequisites)

Purpose: remove SQL Server from shared infrastructure and stand up the SQLite configuration plumbing. No user story work can start until this phase is finished.

- [ ] T004 Create the shared persistence configuration record and validation helpers at InquirySpark.Common/Models/PersistenceProviderConfig.cs
- [ ] T005 [P] Implement the centralized SqliteOptionsConfigurator with read-only enforcement at InquirySpark.Repository/Configuration/SqliteOptionsConfigurator.cs
- [ ] T006 Refactor InquirySpark.Repository/Database/InquirySparkContext.cs to consume the configurator, disable `Database.Migrate()`, and guard against schema writes
- [ ] T007 [P] Harden InquirySpark.Repository/Services/DbContextHelper.cs so SQLite exceptions surface actionable provider diagnostics
- [ ] T008 Remove Microsoft.EntityFrameworkCore.SqlServer, Microsoft.Data.SqlClient, and related imports from InquirySpark.Repository/InquirySpark.Repository.csproj, InquirySpark.WebApi/InquirySpark.WebApi.csproj, InquirySpark.Web/InquirySpark.Web.csproj, InquirySpark.Admin/InquirySpark.Admin.csproj, and InquirySpark.Common/InquirySpark.Common.csproj
- [ ] T009 Archive legacy SQL Server artifacts by unloading InquirySpark.Database/InquirySpark.Database.sqlproj from InquirySpark.sln and documenting the history in docs/copilot/session-2025-12-04/legacy-sqlserver-project.md

---

## Phase 3: User Story 1 – Repository owner eliminates SQL Server dependencies (Priority: P1) 🎯 MVP

**Goal**: Every project runs exclusively on the immutable SQLite provider with no SQL Server packages, configs, or runtime hooks remaining.

**Independent Test**: `dotnet restore` + `dotnet build InquirySpark.sln` succeeds on a clean machine without SQL Server installed, and each application starts using SQLite providers without attempting migrations.

### Tests for User Story 1

- [ ] T010 [P] [US1] Add MSTest coverage proving SqliteOptionsConfigurator builds Microsoft.Data.Sqlite connections at InquirySpark.Common.Tests/Providers/SqliteProviderTests.cs

### Implementation for User Story 1

- [ ] T011 [US1] Update InquirySpark.WebApi/Program.cs to register PersistenceProviderConfig, call `UseSqlite`, and remove SQL Server builder logic
- [ ] T012 [P] [US1] Update InquirySpark.Admin/Program.cs to share the configurator and delete SQL Server scaffolding
- [ ] T013 [P] [US1] Update InquirySpark.Web/Program.cs to wire the shared SQLite configuration and fail fast when the `.db` file is missing
- [ ] T014 [US1] Replace SQL Server connection strings with `Data Source=...;Mode=ReadOnly` entries inside InquirySpark.WebApi/appsettings.json and InquirySpark.WebApi/appsettings.Development.json
- [ ] T015 [P] [US1] Apply the SQLite connection string pattern to InquirySpark.Admin/appsettings.json and InquirySpark.Admin/appsettings.Development.json
- [ ] T016 [P] [US1] Apply the SQLite connection string pattern to InquirySpark.Web/appsettings.json and InquirySpark.Web/appsettings.Development.json
- [ ] T017 [US1] Mark immutable `.db` assets as `Content` with `CopyIfNewer` metadata in InquirySpark.Admin/InquirySpark.Admin.csproj (and any other project bundling the database)
- [ ] T018 [US1] Update README.md “Getting Started” guidance to remove SQL Server prerequisites and highlight the SQLite-only workflow

**Checkpoint**: US1 delivers an independently testable MVP once the above tasks pass.

---

## Phase 4: User Story 2 – Build engineer establishes a clean .NET 10 baseline (Priority: P2)

**Goal**: Solution builds warning-free with analyzers enforced locally and in CI, proving the SQLite migration did not degrade build quality.

**Independent Test**: GitHub Actions (or equivalent) workflow runs `dotnet build InquirySpark.sln -warnaserror` and `dotnet test InquirySpark.sln` successfully on the first attempt.

### Implementation for User Story 2

- [ ] T019 [US2] Author Directory.Build.props to turn on TreatWarningsAsErrors, Nullable, ImplicitUsings, and `AnalysisLevel=latest` for all projects at the repository root
- [ ] T020 [P] [US2] Tighten .editorconfig (repo root) to elevate nullable, style, and analyzer diagnostics aligned with the SQLite baseline
- [ ] T021 [US2] Create `.github/workflows/sqlite-baseline.yml` that runs `dotnet build -warnaserror` and `dotnet test` across Windows and Linux agents
- [ ] T022 [P] [US2] Implement eng/BuildVerification.ps1 so developers replicate the CI commands locally and fail if warnings appear
- [ ] T023 [US2] Publish the build verification checklist covering restore/build/test expectations at docs/copilot/session-2025-12-04/sqlite-build-checklist.md

**Checkpoint**: US2 is complete when CI and local scripts enforce the warning-free baseline.

---

## Phase 5: User Story 3 – Quality lead validates parity and readiness (Priority: P3)

**Goal**: Operational stakeholders can confirm the new persistence baseline via health endpoints, smoke tests, and updated run-books without touching SQL Server.

**Independent Test**: Hitting `/api/system/health` and `/api/system/database/state` on each host returns SQLite metadata, QA smoke tests pass, and deployment docs describe the new verification steps.

### Tests for User Story 3

- [ ] T024 [P] [US3] Add integration tests targeting the health endpoints with Microsoft.AspNetCore.Mvc.Testing at InquirySpark.Common.Tests/Integration/SystemHealthTests.cs

### Implementation for User Story 3

- [ ] T025 [US3] Implement SystemHealthController that matches contracts/system-health.openapi.yaml at InquirySpark.WebApi/Controllers/SystemHealthController.cs
- [ ] T026 [P] [US3] Expose `/api/system/database/state` metadata within the same controller, enforcing read-only detection logic
- [ ] T027 [US3] Add Admin diagnostics UI partial at InquirySpark.Admin/Views/Shared/_SystemHealthPartial.cshtml showing provider status from the API
- [ ] T028 [P] [US3] Add Web diagnostics UI partial at InquirySpark.Web/Pages/Shared/_SystemHealthPartial.cshtml mirroring the Admin status output
- [ ] T029 [US3] Update deployment run-books with SQLite health verification steps at docs/copilot/session-2025-12-04/sqlite-operational-readiness.md
- [ ] T030 [US3] Refresh specs/001-remove-sql-server/quickstart.md to include the health-endpoint smoke test procedure and troubleshooting guidance

**Checkpoint**: US3 finishes when operational docs and automated tests confirm parity with the new baseline.

---

## Final Phase: Polish & Cross-Cutting Concerns

Purpose: ensure the repository meets documentation, cleanliness, and verification expectations once all user stories land.

- [ ] T031 Run `dotnet build InquirySpark.sln -warnaserror` and `dotnet test InquirySpark.sln`, capturing logs in docs/copilot/session-2025-12-04/sqlite-baseline-validation.md
- [ ] T032 [P] Verify no SQLite `.db` artifacts changed by checking `git status` and documenting the check in docs/copilot/session-2025-12-04/sqlite-data-assets.md
- [ ] T033 Record the final terminology and documentation sweep in docs/copilot/session-2025-12-04/sqlite-terminology-review.md

---

## Dependencies & Execution Order

1. **Setup (Phase 1)** → must finish before any repository changes, delivering SDK pinning and environment documentation.
2. **Foundational (Phase 2)** → depends on Setup, unblocks every user story by removing SQL Server foundations and adding the shared SQLite configurator.
3. **User Story 1 (P1)** → depends on Foundational; once complete, the MVP is shippable.
4. **User Story 2 (P2)** → depends on Foundational (and benefits from US1 to ensure provider ready) but remains independently testable.
5. **User Story 3 (P3)** → depends on Foundational and the health contract; may execute in parallel with US2 if capacity allows.
6. **Polish Phase** → runs after the desired set of user stories (at least US1) land.

## Parallel Execution Examples

- **US1**: T012 and T013 can proceed concurrently because they modify different Program.cs files; T010 runs in parallel once T005–T007 land.
- **US2**: T020 and T022 are parallelizable since one edits .editorconfig while the other creates a PowerShell script.
- **US3**: T025 and T027 run simultaneously (API vs. Admin UI), and T024 can begin as soon as T025 exposes the endpoints.

## Implementation Strategy

1. Deliver the MVP by completing Phases 1–3, then pause for validation (run quickstart, smoke tests).
2. Layer on build-quality enforcement (Phase 4) to keep the baseline healthy.
3. Finish with parity validation (Phase 5) and the polish phase, ensuring documentation and verification artifacts are consistent.
4. Each story is independently testable, so the team can stop after any completed phase and still have a working increment.
