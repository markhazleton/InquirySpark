# Implementation Plan: SQL Server Dependency Removal Baseline

**Branch**: `[001-remove-sql-server]` | **Date**: December 4, 2025 | **Spec**: [specs/001-remove-sql-server/spec.md](specs/001-remove-sql-server/spec.md)
**Input**: Feature specification from `/specs/001-remove-sql-server/spec.md`

## Summary

Retire every SQL Server dependency across InquirySpark’s .NET 10 solution, standardize EF Core configuration on the shipped read-only SQLite databases, and raise build quality to a warning-free baseline so future work starts from a clean, provider-agnostic foundation.

## Technical Context

**Language/Version**: C# 13 / .NET 10 (ASP.NET Core MVC, Razor Pages, Web API)  
**Primary Dependencies**: EF Core 10 (Sqlite provider), ASP.NET Core Identity, MSTest, WebSpark Bootswatch, DataTables, Microsoft.Extensions.Logging  
**Storage**: Immutable SQLite `.db` artifacts shared by Admin, Web, and WebApi; no schema or data mutations allowed  
**Testing**: MSTest test projects plus solution-wide integration/smoke tests executed via `dotnet test` pipelines  
**Target Platform**: Windows/Linux hosts running Kestrel behind reverse proxies; developer machines on Windows 11  
**Project Type**: Multi-project .NET solution (InquirySpark.WebApi, InquirySpark.Admin, InquirySpark.Web, InquirySpark.Repository, InquirySpark.Common, InquirySpark.Common.Tests, InquirySpark.Database)  
**Performance Goals**: Maintain current request throughput/latency while ensuring `dotnet build InquirySpark.sln -warnaserror` completes in <5 minutes on CI agents  
**Constraints**: No SQL Server packages or connection strings may remain; SQLite databases are consumed read-only; builds must be warning-free with TreatWarningsAsErrors enabled; documentation must describe the new provider story  
**Scale/Scope**: 3 runtime applications, 2 shared libraries, 1 database project, ~40 DbSets, and shared deployment collateral

## Constitution Check

*GATE STATUS: PASS (No ratified principles are defined in .specify/memory/constitution.md; document currently contains placeholders, so this feature proceeds under standard engineering guidelines. If principles are later ratified, re-run this gate.)*

## Project Structure

### Documentation (this feature)

```text
specs/001-remove-sql-server/
├── plan.md          # This implementation plan
├── research.md      # Phase 0 research outputs
├── data-model.md    # Phase 1 data model summary
├── quickstart.md    # Phase 1 onboarding guide
├── contracts/       # Phase 1 API/schema contracts
└── tasks.md         # Generated later via /speckit.tasks
```

### Source Code (repository root)

```text
InquirySpark.sln
├── InquirySpark.WebApi/        # REST API + Swagger, EF Core data access
├── InquirySpark.Admin/         # MVC admin interface (Bootstrap + DataTables)
├── InquirySpark.Web/           # Public Razor Pages app
├── InquirySpark.Repository/    # EF Core services + DbContext helper abstractions
├── InquirySpark.Common/        # Shared models, SDK objects, response wrappers
├── InquirySpark.Common.Tests/  # MSTest suite
├── InquirySpark.Database/      # SQL project (to be archived or converted)
└── docs/, specs/, .specify/    # Documentation and workflow assets
```

**Structure Decision**: Maintain the existing multi-project .NET solution. All persistence changes happen inside InquirySpark.Repository (DbContext + services) and propagate through WebApi/Admin/Web projects via dependency injection. Documentation and planning artifacts live under `specs/001-remove-sql-server/` per Copilot instructions.

## Complexity Tracking

No constitution violations or additional complexity justifications required at this time.
