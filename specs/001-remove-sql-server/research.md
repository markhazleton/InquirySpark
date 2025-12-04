# Research Log — 2025-12-04

## Decision: Standardize on Microsoft.Data.Sqlite with EF Core 10 context configuration
- **Rationale**: Aligns with FR-003/FR-009 and keeps all apps on the same lightweight provider that already powers InquirySpark.Admin’s demo experience. EF Core’s Sqlite provider is fully supported in .NET 10, removes the SQL Server dependency tree, and works in developer/CI environments without extra services.
- **Alternatives considered**: Continue supporting SQL Server as a secondary provider (would double the test/build matrix and violates spec), switch to PostgreSQL now (requires new infrastructure plus data migration that stakeholders explicitly deferred).

## Decision: Treat SQLite `.db` artifacts as immutable and disable runtime migrations
- **Rationale**: Spec clarifies no schema/data changes are allowed in this effort. Disabling `Database.Migrate()` calls and guarding any write attempts ensures EF Core does not try to evolve the immutable files. Developers can rely on the checked-in databases for deterministic test data.
- **Alternatives considered**: Generate new migrations targeting SQLite (conflicts with “no .db changes” directive), recreate schema on first run (risks drift and increases onboarding friction).

## Decision: Centralize DbContext options through a shared `SqliteOptionsConfigurator`
- **Rationale**: InquirySpark.Repository already exposes services consumed by the Web/WebApi/Admin apps. A shared configurator (extension method or factory) ensures connection strings, command behavior, and health-check policies stay consistent and makes it trivial to enforce read-only flags or WAL settings across all hosts.
- **Alternatives considered**: Configure each app manually (prone to drift), introduce a new configuration microservice (overkill for current scope).

## Decision: Enforce warning-free builds via solution-wide Directory.Build.props
- **Rationale**: FR-005 demands `dotnet build -warnaserror` succeeds. Pushing `TreatWarningsAsErrors`, nullable context, analyzers, and code-style settings into shared props prevents regression and ensures every project inherits the same rules without duplication.
- **Alternatives considered**: Configure each csproj individually (higher maintenance), rely on CI-only rules (developers wouldn’t see issues locally).

## Decision: Archive or transform SQL Server–specific artifacts in `InquirySpark.Database`
- **Rationale**: The sqlproj contains SQL Server scripts/migrations that would reintroduce forbidden dependencies if kept active. Archiving (or converting to documentation describing legacy assets) keeps history without confusing the new SQLite-only workflow.
- **Alternatives considered**: Keep sqlproj in build (causes build failures when SQL tooling missing), delete entirely (could lose institutional knowledge; archiving preserves it).
