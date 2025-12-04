# Data Model — SQL Server Dependency Removal Baseline

## Overview
The InquirySpark solution already uses EF Core models defined in `InquirySpark.Repository`. This feature does not introduce new domain entities; it refactors how existing infrastructure entities are configured and consumed. The focus is on configuration, environment metadata, and build artifacts necessary to run against immutable SQLite databases.

## Entities

### PersistenceProviderConfig
- **Purpose**: Encapsulates provider-agnostic settings (connection string, provider name, migration assembly, read-only flags) shared across WebApi, Admin, and Web hosts.
- **Key Fields**:
  - `ProviderName` (string, required) — must equal `"Sqlite"` for this feature.
  - `ConnectionString` (string, required) — file path or `Data Source=` descriptor pointing to shipped `.db` asset.
  - `ReadOnly` (bool, default `true`) — enforces immutable access semantics.
  - `CommandTimeoutSeconds` (int?, optional) — allows tuning for long-running background tasks.
  - `MigrationAssembly` (string, optional) — remains populated for legacy documentation but is ignored while migrations are disabled.
- **Relationships**: Injected into `InquirySparkContext` factories; consumed by health checks and diagnostics endpoints.
- **Validation**:
  - Connection string must point to an existing file accessible by the hosting process.
  - `ReadOnly` cannot be disabled unless a future spec authorizes schema changes.

### DbContextFactory / SqliteOptionsConfigurator
- **Purpose**: Central place to apply `UseSqlite`, disable migrations, register interceptors, and ensure identical logging/resiliency options for every DbContext.
- **Key Fields**:
  - `IOptions<PersistenceProviderConfig>` or configuration snapshot.
  - `ILoggerFactory` for consistent logging.
- **Constraints**: Must throw a descriptive exception if provider name != SQLite or if the file is missing.

### BuildQualityBaseline
- **Purpose**: Represents the aggregated MSBuild/Directory.Build.props settings that enforce nullable reference types, analyzers, and TreatWarningsAsErrors across all projects.
- **Key Fields**:
  - `TreatWarningsAsErrors` (bool, true)
  - `AnalysisLevel` (string, e.g., `latest`)
  - `Nullable` (string, `enable`)
  - `ImplicitUsings` (bool, true)
- **Relationships**: Imported by every csproj; verified during CI builds.

### DocumentationAsset
- **Purpose**: Captures onboarding/run-book artifacts that must be updated to describe the SQLite-only baseline.
- **Key Fields**:
  - `Path` (string) — e.g., `README.md`, `docs/.../NPM-BUILD.md`.
  - `Audience` (enum) — Developer, QA, DevOps.
  - `Status` (enum) — Pending Update, Updated, Reviewed.
- **Relationships**: Tied to FR-006/FR-007 success criteria; tracked via future tasks.md.

## State Transitions & Workflows
- **Provider Configuration Lifecycle**: `Unconfigured → Configured (SQLite) → Validated` once health checks pass without SQL Server dependencies.
- **Build Quality Lifecycle**: `Warnings-present → TreatWarningsAsErrors enforced → CI green`; regressions reset the state to `Warnings-present` and block merges.
- **Documentation Lifecycle**: `Outdated → Drafted → Reviewed → Published`. This ensures clarity for developers adopting the new baseline.

## Notes
- Domain entities such as `Survey`, `Question`, and `Application` remain unchanged. Any schema updates would violate FR-009 and must be deferred to a future feature.
- The `InquirySpark.Database` sqlproj transitions from “active source” to “archived reference” status; no runtime dependency remains.
