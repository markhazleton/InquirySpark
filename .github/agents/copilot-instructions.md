# InquirySpark Development Guidelines

Auto-generated from all feature plans. Last updated: 2025-12-04

## Active Technologies
- C# 13 on .NET 10 (WebApi + Admin + Repository) + ASP.NET Core MVC/WebApi, EF Core 10, Hangfire 1.8, Azure Service Bus, Azure Blob Storage SDK, Azure Cognitive Search SDK, Chart.js 4 + chartjs-node-canvas, DataTables 2.3.5, JSZip/PDFMake for exports (001-benchmark-insights)
- SQL Server 2022 (`InquirySparkContext`) for OLTP + Hangfire metadata, Azure Blob Storage for rendered assets/exports, Azure Cognitive Search index for chart discovery, Azure Service Bus queues/topics for batch fan-out (001-benchmark-insights)
- C# / .NET 10 + ASP.NET Core MVC, EF Core (SQLite provider), Microsoft.AspNetCore.Identity (PasswordHasher only — not full Identity framework) (002-hateoas-conversation-api)
- SQLite — `InquirySpark.db` (ReadWriteCreate in non-dev; ReadOnly in Development) (002-hateoas-conversation-api)
- C# on .NET 10 (`net10.0`) + ASP.NET Core MVC/Razor, Entity Framework Core (SQLite provider), existing shared libraries in `InquirySpark.Common` and `InquirySpark.Repository`, existing UI stack (Bootstrap 5 + DataTables + Bootswatch pattern) (001-unified-web-experience)
- Existing SQLite assets and current repository/context model; no new primary persistence engine introduced by this feature (001-unified-web-experience)

- C# 13 / .NET 10 (ASP.NET Core MVC, Razor Pages, Web API) + EF Core 10 (Sqlite provider), ASP.NET Core Identity, MSTest, WebSpark Bootswatch, DataTables, Microsoft.Extensions.Logging (001-remove-sql-server)

## Project Structure

```text
backend/
frontend/
tests/
```

## Commands

# Add commands for C# 13 / .NET 10 (ASP.NET Core MVC, Razor Pages, Web API)

## Code Style

C# 13 / .NET 10 (ASP.NET Core MVC, Razor Pages, Web API): Follow standard conventions

## Recent Changes
- 001-unified-web-experience: Added C# on .NET 10 (`net10.0`) + ASP.NET Core MVC/Razor, Entity Framework Core (SQLite provider), existing shared libraries in `InquirySpark.Common` and `InquirySpark.Repository`, existing UI stack (Bootstrap 5 + DataTables + Bootswatch pattern)
- 002-hateoas-conversation-api: Added C# / .NET 10 + ASP.NET Core MVC, EF Core (SQLite provider), Microsoft.AspNetCore.Identity (PasswordHasher only — not full Identity framework)
- 001-benchmark-insights: Added C# 13 on .NET 10 (WebApi + Admin + Repository) + ASP.NET Core MVC/WebApi, EF Core 10, Hangfire 1.8, Azure Service Bus, Azure Blob Storage SDK, Azure Cognitive Search SDK, Chart.js 4 + chartjs-node-canvas, DataTables 2.3.5, JSZip/PDFMake for exports


<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->

<!-- DEVSPARK SHARED CONTEXT:START -->
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
<!-- DEVSPARK SHARED CONTEXT:END -->

