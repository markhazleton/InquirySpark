# Legacy SQL Server Project Archive

**Project**: `InquirySpark.Database/InquirySpark.Database.sqlproj`
**Archived**: December 4, 2025
**Reason**: The SQL Server baseline has been superseded by the immutable SQLite provider introduced in the SQL Server dependency removal feature.

## What Changed?
- `InquirySpark.sln` now ships only the runtime/service projects; the SSDT `.sqlproj` stays in the repository for historical reference but is intentionally excluded from the active solution.
- EF Core configuration no longer targets SQL Server, so no code paths compile against the sqlproj-generated artifacts.
- The new persistence story is documented in `docs/copilot/session-2025-12-04/sqlite-data-assets.md` and backed by the `data/sqlite/*.db` files.

## When to Use the sqlproj
Use the sqlproj only when historical investigation is required (e.g., comparing schemas or exporting legacy scripts). Opening it in Visual Studio or Azure Data Studio is safe, but **do not** publish it to any environment tied to this baseline.

### Access Instructions
1. Launch Visual Studio 2022 with the SQL Server Data Tools workload installed.
2. Open `InquirySpark.Database/InquirySpark.Database.sqlproj` directly (it is no longer part of the main solution).
3. Treat the project as read-only. Any schema adjustments or migration scripts authored here must be staged in a future feature before being considered for inclusion in the SQLite artifacts.

## Operational Notes
- Keep the sqlproj in source control so auditors can trace historic schema design decisions.
- Flag any lingering SQL Server references in documentation or build assets and remove them as part of normal maintenance.
- If stakeholders ever approve a follow-up SQL Server export, create a dedicated feature spec; do **not** reintroduce the sqlproj into `InquirySpark.sln` without that approval.

## Change Log
- **2025-12-04**: Archived project, captured rationale, and confirmed it no longer loads as part of `InquirySpark.sln`.
