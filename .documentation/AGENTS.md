# InquirySpark Shared Agent Context

This file provides shared repository context that DevSpark can hydrate into agent-specific context files.

## Governance

- Read `.documentation/memory/constitution.md` first for project rules.
- Treat `.documentation/` as team-owned work product and `.devspark/` as framework-managed stock assets.
- Keep documentation markdown under `.documentation/` except for `README.md` and approved `.github/` governance files.

## Repository Focus

- InquirySpark is a .NET 10 solution with `InquirySpark.WebApi`, `InquirySpark.Admin`, `InquirySpark.Web`, `InquirySpark.Common`, and `InquirySpark.Repository`.
- SQLite is the only supported data store in this repository.
- Admin UI work follows Bootstrap 5 + DataTables conventions.

## Build And Test

- Build: `dotnet build InquirySpark.sln`
- Test: `dotnet test`
- Admin app: `dotnet build InquirySpark.Admin/InquirySpark.Admin.csproj` also runs the npm asset pipeline.

## DevSpark Usage

- Use stock commands from `.devspark/defaults/commands/` unless a team override is intentionally added.
- Use stock PowerShell scripts from `.devspark/scripts/powershell/` unless a team override is intentionally added.
- Prefer full-spec workflow for larger features and `devspark.quickfix` for contained fixes.