# Tasks: SQL Server Dependency Removal Baseline

**Input**: Design documents from `/specs/001-remove-sql-server/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

## Phase 1: Setup (Shared Infrastructure)

Purpose: establish repository-wide tooling and documentation required before touching application code.

- [ ] T001 Create `global.json` to pin .NET SDK 10.0.100 at global.json
- [ ] T002 Document immutable SQLite asset locations and checksum policy in docs/copilot/session-2025-12-04/sqlite-data-assets.md
- [ ] T003 Add shared environment template describing provider paths and read-only flags at eng/sqlite.env.example

---

## Phase 2: Foundational (Blocking Prerequisites)

Purpose: remove SQL Server from shared infrastructure and stand up the SQLite configuration plumbing. No user story work can start until this phase is finished.

- [ ] T004 Create Directory.Build.props with TreatWarningsAsErrors, Nullable, ImplicitUsings, and AnalysisLevel settings at Directory.Build.props
- [ ] T005 Remove `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.Data.SqlClient`, and related references from InquirySpark.Repository/InquirySpark.Repository.csproj, InquirySpark.WebApi/InquirySpark.WebApi.csproj, InquirySpark.Web/InquirySpark.Web.csproj, InquirySpark.Admin/InquirySpark.Admin.csproj, and InquirySpark.Common/InquirySpark.Common.csproj
- [ ] T006 Archive legacy SQL Server scripts by unloading InquirySpark.Database/InquirySpark.Database.sqlproj from InquirySpark.sln and capturing its purpose in docs/copilot/session-2025-12-04/legacy-sqlserver-project.md
- [ ] T007 Add centralized `SqliteOptionsConfigurator` with read-only enforcement at InquirySpark.Repository/Configuration/SqliteOptionsConfigurator.cs
- [ ] T008 Update InquirySpark.Repository/Database/InquirySparkContext.cs to consume `SqliteOptionsConfigurator`, disable `Database.Migrate()`, and guard against schema writes
- [ ] T009 Harden InquirySpark.Repository/Services/DbContextHelper.cs to surface SQLite-specific exceptions with actionable error messages

---

## Phase 3: User Story 1 – Repository owner eliminates SQL Server dependencies (Priority: P1) 🎯 MVP

**Goal**: Every project runs exclusively on the immutable SQLite provider with no SQL Server packages, configs, or runtime hooks remaining.

**Independent Test**: `dotnet restore` + `dotnet build InquirySpark.sln` succeeds on a clean machine without SQL Server installed, and each application starts using SQLite providers without attempting migrations.

- [ ] T010 [P] [US1] Add MSTest coverage that validates `InquirySparkContext` resolves `Microsoft.Data.Sqlite` connections using the configurator at InquirySpark.Common.Tests/Providers/SqliteProviderTests.cs
- [ ] T011 [US1] Update InquirySpark.WebApi/Program.cs to register `PersistenceProviderConfig`, call `UseSqlite`, and remove SQL Server–specific builder logic
- [ ] T012 [P] [US1] Update InquirySpark.Admin/Program.cs to rely on `SqliteOptionsConfigurator` and delete SQL Server connection scaffolding
- [ ] T013 [P] [US1] Update InquirySpark.Web/Program.cs to wire the shared SQLite configuration and fail fast if the `.db` file is missing
- [ ] T014 [US1] Replace SQL Server `ConnectionStrings` sections with `Data Source=...;Mode=ReadOnly` entries inside InquirySpark.WebApi/appsettings.Development.json, InquirySpark.Admin/appsettings.Development.json, and InquirySpark.Web/appsettings.Development.json
- [ ] T015 [US1] Mark immutable `.db` assets as `Content` with `CopyIfNewer` metadata in InquirySpark.Admin/InquirySpark.Admin.csproj (and any other project that bundles the database)
- [ ] T016 [US1] Update root README.md “Getting Started” guidance to remove SQL Server prerequisites and document the SQLite-only workflow

**Checkpoint**: US1 delivers an independently testable MVP once the above tasks pass.

---

## Phase 4: User Story 2 – Build engineer establishes a clean .NET 10 baseline (Priority: P2)

**Goal**: Solution builds warning-free with analyzers enforced locally and in CI, proving the SQLite migration did not degrade build quality.

**Independent Test**: GitHub Actions (or equivalent) workflow runs `dotnet build InquirySpark.sln -warnaserror` and `dotnet test InquirySpark.sln` successfully on the first attempt.

- [ ] T017 [P] [US2] Extend Directory.Build.props with analyzer packages, `AnalysisLevel=latest`, and deterministic build flags for every project
- [ ] T018 [US2] Tighten `.editorconfig` (repo root) to elevate nullable, style, and analyzer diagnostics that guard against regression
- [ ] T019 [US2] Author `.github/workflows/sqlite-baseline.yml` to run `dotnet build -warnaserror` and `dotnet test` on Windows and Linux agents
- [ ] T020 [P] [US2] Create eng/BuildVerification.ps1 that developers run locally to mimic the CI commands and fail if warnings appear
- [ ] T021 [US2] Publish a build verification checklist covering restore/build/test steps at docs/copilot/session-2025-12-04/sqlite-build-checklist.md

**Checkpoint**: US2 is complete when CI and local scripts enforce the warning-free baseline.

---

## Phase 5: User Story 3 – Quality lead validates parity and readiness (Priority: P3)

**Goal**: Operational stakeholders can confirm the new persistence baseline via health endpoints, smoke tests, and updated run-books without touching SQL Server.

**Independent Test**: Hitting `/api/system/health` and `/api/system/database/state` on each host returns SQLite metadata, QA smoke tests pass, and deployment docs describe the new verification steps.

- [ ] T022 [P] [US3] Implement the System Health controller that matches `contracts/system-health.openapi.yaml` at InquirySpark.WebApi/Controllers/SystemHealthController.cs
- [ ] T023 [US3] Add Admin and Web diagnostics UI snippets (e.g., InquirySpark.Admin/Views/Shared/_SystemHealthPartial.cshtml and InquirySpark.Web/Pages/Shared/_SystemHealthPartial.cshtml) that surface provider status
- [ ] T024 [P] [US3] Add integration tests hitting the new health endpoints using Microsoft.AspNetCore.Mvc.Testing at InquirySpark.Common.Tests/Integration/SystemHealthTests.cs
- [ ] T025 [US3] Update deployment run-books with SQLite health verification steps at docs/copilot/session-2025-12-04/sqlite-operational-readiness.md
- [ ] T026 [US3] Refresh specs/001-remove-sql-server/quickstart.md to include the health-endpoint smoke test procedure

**Checkpoint**: US3 finishes when operational docs and automated tests confirm parity with the new baseline.

---

## Final Phase: Polish & Cross-Cutting Concerns

Purpose: ensure the repository meets documentation, cleanliness, and verification expectations once all user stories land.

- [ ] T027 Run `dotnet build InquirySpark.sln -warnaserror` and `dotnet test InquirySpark.sln` (capturing logs) to prove the final baseline, and store the transcript in docs/copilot/session-2025-12-04/sqlite-baseline-validation.md
- [ ] T028 Verify no SQLite `.db` artifacts changed by checking `git status` and documenting the check inside docs/copilot/session-2025-12-04/sqlite-data-assets.md
- [ ] T029 Conduct a documentation sweep (README.md, docs/copilot references, Quickstart) to ensure consistent terminology and link coverage

---

## Dependencies & Execution Order

1. **Setup (Phase 1)** → must finish before any repository changes, delivering SDK pinning and environment documentation.
2. **Foundational (Phase 2)** → depends on Setup, unblocks every user story by removing SQL Server foundations and adding the shared SQLite configurator.
3. **User Story 1 (P1)** → depends on Foundational; once complete, the MVP is shippable.
4. **User Story 2 (P2)** → depends on Foundational (and benefits from US1 to ensure provider ready) but remains independently testable.
5. **User Story 3 (P3)** → depends on Foundational and the health contract; may execute in parallel with US2 if capacity allows.
6. **Polish Phase** → runs after the desired set of user stories (at least US1) land.

## Parallel Execution Examples

- **US1**: T012 and T013 can proceed concurrently because they touch different Program.cs files; T010 can run in parallel once T007–T008 ship.
- **US2**: T017 and T020 are parallelizable since one edits Directory.Build.props while the other adds a script under `eng/`.
- **US3**: T022 and T023 can run simultaneously (API vs. UI), and T024 can begin once T022 exposes the endpoints.

## Implementation Strategy

1. Deliver the MVP by completing Phases 1–3, then pause for validation (run quickstart, smoke tests).
2. Layer on build-quality enforcement (Phase 4) to keep the baseline healthy.
3. Finish with parity validation (Phase 5) and the polish phase, ensuring documentation and verification artifacts are consistent.
4. Each story is independently testable, so the team can stop after any completed phase and still have a working increment.
