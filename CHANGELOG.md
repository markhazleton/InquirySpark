# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### In Progress
- **001-remove-sql-server (US2/US3)**: Build engineer baseline (TreatWarningsAsErrors, CI workflow, BuildVerification.ps1) and quality lead validation (health endpoints, integration tests) are still open.

## [2026-04-12] Archive Run

### Archived
- `.documentation/copilot/harvest-2026-04-07.md` — superseded harvest report from an earlier archive run; key outcomes are already represented in the changelog and current guide.
- `.documentation/copilot/harvest-2026-04-11.md` — superseded harvest report from the previous run; retained only for historical traceability in `.archive/2026-04-12/`.

### Key decisions preserved
- Keep only the latest harvest report in active `.documentation/copilot/` to reduce noise in the living documentation surface.
- Keep `.documentation/copilot/harvest-2026-04-12.md` as the current operational baseline report.

## [2026-04-12] - HATEOAS Conversation API Completion

Feature spec: `.documentation/specs/002-hateoas-conversation-api/`  
Branch: `002-hateoas-conversation-api`  
Status: Implemented and merged to `main`

### Added
- Versioned anonymous conversation API endpoints under `api/v1/conversation` with HATEOAS navigation (`next_url`, `prev_url`, `conversation_ended`).
- Survey start flow supporting survey discovery by application and start-by-survey behavior.
- Resume and restart conversation behavior using persisted `SurveyResponse.ConversationId`.
- Swagger/OpenAPI documentation for start/next endpoints and response contracts.

### Changed
- Added GUID `ConversationId` support on survey response records and integrated it into conversation routing.
- Added hashed password support for conversation authentication and wired password verification into start flow.
- Standardized envelope and DTO usage in shared models for conversation start/next payloads.

### Fixed
- Enforced deterministic question progression and answer update behavior for resumed conversations.
- Improved endpoint status-code mapping for conversation validation and authentication failures.

## [2026-04-11] Archive Run

### Archive run (session notes archived)
- Archived `.documentation/copilot/session-2026-04-07/` SQLite verification notes to `.archive/.documentation/copilot/session-2026-04-07/` (5 files).
- Created `.documentation/copilot/harvest-2026-04-11.md` with the full harvest summary and dispositions.

### Key decisions preserved
- No changelog feature gaps were found for completed specs in this run.
- No spec-linked code comments required rewrite (0 found).

## [2026-04-07] Archive Run

### Archive run (no files moved — all candidates current)
- `harvest-2026-04-07.md` — kept; current harvest report from today's session
- **Created** `.documentation/Guide.md` — new living orientation document for `.documentation/`
- **Created** `.archive/README.md` — inventory of all archived batches

### Key decisions preserved
- `.archive/` is write-only from operational standpoint; `.devspark/scripts/` and `.documentation/scripts/` are never archived
- Constitution at `.documentation/memory/constitution.md` is never an archive candidate

## [2026-04-07] - DevSpark Framework Migration

### Changed
- Replaced speckit agent/prompt framework with **DevSpark** across all tooling entry points.
- Renamed all `.github/agents/speckit.*.agent.md` → `.github/agents/devspark.*.agent.md` (21 agent files).
- Renamed all `.github/prompts/speckit.*.prompt.md` → `.github/prompts/devspark.*.prompt.md` (21 prompt files).
- Added `.devspark/` directory containing stock default commands, memory constitution, PowerShell scripts, and Mustache templates.
- Removed `.documentation/SPECKIT_VERSION` file (replaced by `.devspark/` versioning).
- Updated `.gitignore` and `.vscode/settings.json` for devspark paths.
- Updated `InquirySpark.Admin`, `InquirySpark.Common`, `InquirySpark.Common.Tests`, and `InquirySpark.Repository` project files.
- Added `devspark.personalize` agent (new capability not present in speckit).

## [2026-03-29] - Benchmark Insights & Reporting Platform

Feature spec: `.documentation/specs/001-benchmark-insights/`  
Branch: `001-benchmark-insights`  
Status: Implementation complete (70/70 tasks across 8 phases)

### Added
- **Chart Builder (US1)**: Reusable chart definition CRUD with version history, dataset catalog, Chart.js template library (16 templates), formula parser (SUM, AVG, PERCENT_DIFF), client-side preview, and validation rules.
- **Batch Chart Rendering (US2)**: Batch job creation with parameter sweeps, Hangfire orchestration with Service Bus queue, progress tracking, chart asset library with approval workflow, and Azure Cognitive Search placeholder.
- **Data Validation & Export (US3)**: Server-side paged data explorer with dynamic filter builder, read-only enforcement with watermarking, multi-format export (XLSX, CSV, TSV, PDF), export request tracking with 7-day retention.
- **Deck Assembly (US4)**: Deck project management, slide ordering with drag-and-drop, chart asset staging from library, PowerPoint/Google Slides export, and deck telemetry for reuse tracking.
- **Gauge Dashboards (US5)**: Dashboard definition builder, gauge tile configuration with threshold coloring, metric group drill-down, snapshot scheduling, and executive-facing read-only views.
- **Audit Logging**: `AuditLog` table and `AuditLogService` tracking all mutation operations with actor, entity type, and timestamp indexes.
- **User Preferences**: Per-user JSON preference storage with optimistic concurrency for builder layouts, panel states, and filter drafts.
- **Policy-Based Authorization**: Role policies (Analyst, Operator, Consultant, Executive) applied to all new API controllers with Razor view integration.
- **13 new EF Core entities** under `InquirySpark.Repository/Database/Entities/Charting/` and `Security/`.
- **21 repository services**, **12 API controllers**, **6 MVC controllers**, **32 Razor views**, **5 background workers**.

### Fixed
- Resolved 71 compilation errors from initial implementation that referenced non-existent entity properties, missing Azure packages, and incorrect service signatures (see archived `BUILD-FIX-SUMMARY.md`).

## [2025-12-04] - SQL Server Removal & SQLite Baseline (Phases 1–3)

Feature spec: `.documentation/specs/001-remove-sql-server/`  
Branch: `001-benchmark-insights`  
Status: US1 complete; US2/US3 in progress

### Changed
- Migrated all persistence from SQL Server to immutable read-only SQLite databases.
- Pinned .NET SDK to 10.0.100 via `global.json`.
- Removed `Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.Data.SqlClient` from all projects.
- Unloaded `InquirySpark.Database.sqlproj` from the solution (archived as legacy).
- Updated `InquirySparkContext` to use `SqliteOptionsConfigurator` with read-only enforcement and migration guards.
- Hardened `DbContextHelper` with SQLite-specific provider diagnostics.

### Added
- `PersistenceProviderConfig` shared configuration record.
- `SqliteOptionsConfigurator` centralized SQLite connection setup.
- MSTest coverage for SQLite provider configuration.
- SQLite connection string patterns in all `appsettings.json` files.
- `data/sqlite/` directory with immutable `.db` assets marked as `Content`/`CopyIfNewer`.
- Updated README.md removing SQL Server prerequisites and documenting SQLite-only workflow.

### Added (Admin UI — prior session)
- Bootstrap 5 + DataTables card template standardization across all CRUD views.
- CDN-free frontend: npm-managed jQuery, Bootstrap JS, DataTables, Bootstrap Icons.
- Bootswatch theme integration via WebSpark.Bootswatch (28+ themes, cookie persistence).
- npm build pipeline (`npm run build` → `wwwroot/lib/`) integrated into `dotnet build`.
