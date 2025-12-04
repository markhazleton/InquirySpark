# Feature Specification: SQL Server Dependency Removal Baseline

**Feature Branch**: `[001-remove-sql-server]`  
**Created**: December 4, 2025  
**Status**: Draft  
**Input**: User description: "Plan to fully remove any SQL Server related code from all projects in the repository. General clean-up and optimization of projects in the repostiory, get a clean build with no warnings or errors to create a quality baseline for future development using industy best practices and standards ofr Net 10 devleopment"

## Clarifications

### Session 2025-12-04

- Q: Should this feature migrate legacy SQL Server data into SQLite? → A: No, operate on the clean SQLite databases that already exist; any remaining legacy migrations stay outside this scope.
- Q: Can this feature modify the existing SQLite `.db` files (schema or seeded data)? → A: No, those databases are already migrated and must remain unchanged; this effort only updates code and configuration to consume them as-is.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Repository owner eliminates SQL Server dependencies (Priority: P1)

The repository owner needs to remove every SQL Server–specific dependency, configuration, and asset so that all InquirySpark applications share a single, provider-agnostic persistence approach that can be configured without SQL Server anywhere in the solution.

**Why this priority**: Without eliminating SQL Server code paths first, any remaining clean-up or modernization effort is blocked because projects would still require SQL Server binaries, build assets, and configuration files.

**Independent Test**: Build the solution on a clean machine without SQL Server installed; verify that dependency restoration, compilation, and smoke tests succeed while all runtime configuration defaults to the new persistence provider.

**Acceptance Scenarios**:

1. **Given** a fresh clone of the repository, **When** a developer restores packages and builds the solution, **Then** no SQL Server NuGet packages, provider-specific code, or connection strings exist in any project.
2. **Given** all three applications (WebApi, Admin, Web), **When** they are started with the default settings, **Then** they use the agreed-upon non–SQL Server data provider and fail gracefully if that provider is unavailable.

---

### User Story 2 - Build engineer establishes a clean .NET 10 baseline (Priority: P2)

The build/release engineer needs a repository that builds without warnings, nullable reference issues, or analyzer violations so that future features can start from an industry-standard .NET 10 baseline.

**Why this priority**: Once SQL Server dependencies are removed, the immediate next value is ensuring the build is stable and warning-free; this provides measurable proof that the migration succeeded.

**Independent Test**: Run `dotnet build InquirySpark.sln -warnaserror` on CI; the command must finish successfully and produce zero warnings.

**Acceptance Scenarios**:

1. **Given** the automated build pipeline, **When** it runs with warnings treated as errors, **Then** the build completes successfully for all projects.
2. **Given** code analysis rulesets, **When** analyzers execute during the build, **Then** no violations remain unchecked into the main branch.

---

### User Story 3 - Quality lead validates parity and readiness (Priority: P3)

The QA/quality lead needs to validate that removing SQL Server did not regress runtime capabilities, deployment scripts, or documentation so stakeholders remain confident adopting the new baseline.

**Why this priority**: After code and build changes, parity validation ensures that all environments, deployment assets, and run-books continue to work and that documentation clearly communicates the new expectations.

**Independent Test**: Execute smoke/regression tests against each application, confirm deployment scripts, infrastructure-as-code templates, and run-books reflect the non–SQL Server provider, and capture sign-off.

**Acceptance Scenarios**:

1. **Given** existing integration/regression test suites, **When** they execute against the refactored applications, **Then** results match the pre-migration baseline (allowing for connection-string updates).
2. **Given** operational documentation and deployment scripts, **When** the quality lead reviews them, **Then** all references to SQL Server are removed or replaced with updated provider guidance.

---

### Edge Cases

- How does the system behave if a developer supplies a legacy SQL Server connection string via environment variables despite the removal? The applications must detect and reject unsupported providers with actionable errors.
- What happens if an EF Core migration or DbContext still references SQL Server provider APIs? Builds must fail with analyzers or CI gates before the change merges.
- How are seed data scripts or stored procedures handled if they relied on SQL Server T-SQL features? The plan must cover equivalent provider-neutral replacements or deprecations.
- How does the repository handle mixed environments where Admin already uses SQLite while other apps migrate simultaneously, ensuring shared migrations and context naming collisions are resolved?
- If stakeholders later need historic SQL Server data, the repository must document that migrations occur in a separate initiative so developers do not expect automated import steps in this baseline.
- What safeguards ensure the applications do not attempt to apply EF Core migrations or other schema changes against the read-only SQLite `.db` artifacts (e.g., migrations disabled, fail-fast checks)?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST remove all SQL Server–specific NuGet packages, provider references, and conditional code paths across InquirySpark.WebApi, InquirySpark.Admin, InquirySpark.Web, InquirySpark.Repository, and InquirySpark.Common.
- **FR-002**: System MUST introduce a single, provider-agnostic EF Core configuration pattern so each application resolves its DbContext options from shared settings and secrets, preventing provider drift.
- **FR-003**: System MUST replace SQL Server connection strings and configuration keys with SQLite equivalents so every application (development, automated testing, and production demos) uses SQLite as the default persistence provider.
- **FR-004**: System MUST update database projects, seed scripts, and EF Core migrations to eliminate T-SQL-specific constructs or obsolete artifacts, ensuring migrations apply cleanly on the new provider.
- **FR-005**: System MUST modernize project files to .NET 10 conventions (nullable annotations, implicit usings, analyzers, TreatWarningsAsErrors) and ensure `dotnet build InquirySpark.sln` produces zero warnings.
- **FR-006**: System MUST align CI/CD workflows, infrastructure scripts, and developer onboarding instructions with the SQL Server–free architecture, including environment variables, secrets, and health checks.
- **FR-007**: System MUST document troubleshooting guidance for local developers (e.g., how to install/configure the new provider, run migrations, and reset demo data) so onboarding remains frictionless.
- **FR-008**: System MUST provide automated verification (unit, integration, or smoke tests) that confirms critical repository functionality still works end-to-end using the new provider before merging to main.
- **FR-009**: System MUST treat the provided SQLite `.db` files as immutable assets for this baseline; no schema migrations or manual edits to those databases occur within this scope.

### Key Entities

- **Persistence Provider Configuration**: Represents the unified configuration model (connection strings, provider name, migration assembly) shared across applications; must support environment overrides without code changes.
- **InquirySparkContext**: EF Core DbContext that currently targets SQL Server; it must become provider-agnostic with migrations compatible with the new backend and consistent seeding routines.
- **Build & Release Pipeline**: The automation responsible for restoring packages, running analyzers/tests, and producing artifacts; must enforce warning-free builds and validate no SQL Server dependencies remain.
- **Operational Documentation Set**: Developer onboarding guides, deployment run-books, and README assets that must explain the new persistence story and any manual steps for provisioning the replacement database.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Following the documented build steps, a clean machine completes dependency restore and a full solution build without installing SQL Server or encountering warnings.
- **SC-002**: All automated test suites (unit + integration) execute using the new provider with pass rates equal to or greater than pre-migration baselines, confirming no functional regressions.
- **SC-003**: Repository-wide static analysis reports zero SQL Server references (packages, namespaces, connection strings) and fewer than five total warnings, each documented with justification if suppressed.
- **SC-004**: Updated documentation and onboarding checklists are reviewed by at least two stakeholders, and feedback indicates the migration path and new environment setup steps are clear (≥90% satisfaction in internal survey or sign-off checklist).

## Assumptions & Constraints

- Existing business logic does not rely on SQL Server–specific stored procedures or CLR functions; if any surface, they will be rewritten in EF/Core logic or equivalent provider-neutral scripts.
- CI agents and developer machines can install or access SQLite without additional licensing costs, and production demos also run on SQLite-provisioned storage.
- Historical SQL Server data that needed to live in SQLite has already been migrated; this feature assumes the provided SQLite databases (with seed/demo data) are the source of truth and does not perform additional data migrations.
- Removing SQL Server code should not change public API contracts; any behavioral changes must be explicitly reviewed with stakeholders.
- Historical data migrations (e.g., InquirySpark.Database.sqlproj) can be archived or reworked without impacting compliance requirements; otherwise, exceptions will be documented.
- The pre-migrated SQLite `.db` files remain read-only artifacts for this delivery; any schema evolution or content refresh will be scheduled as a separate feature.
