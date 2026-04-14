# InquirySpark
Spark Your Inquiry, Ignite Insights.

## Overview
InquirySpark is a .NET 10 unified survey/inquiry and decision-management system delivered through a single MVC web application backed by read-only SQLite databases. The solution contains:

- **InquirySpark.Web**: Unified operations workspace (Bootstrap 5 + DataTables, Razor Views, ASP.NET Core Areas)
- **InquirySpark.Repository**: EF Core 10 services and DbContext abstractions
- **InquirySpark.Common**: Shared models, SDK objects, response wrappers
- **InquirySpark.Common.Tests**: MSTest unit and integration test suite

> **Legacy applications removed**: `DecisionSpark` and `InquirySpark.Admin` have been decommissioned and are no longer part of the active solution. Their capabilities are fully delivered by `InquirySpark.Web`.

## Prerequisites
- **.NET SDK 10.0.100** (enforced via global.json)
- **Node.js 20.x** for the Web asset pipeline (npm build runs automatically on `dotnet build`)
- **No SQL Server required** — all persistence uses immutable SQLite databases

## Getting Started

### 1. Clone and Restore
```bash
git clone https://github.com/MarkHazleton/InquirySpark.git
cd InquirySpark
dotnet restore InquirySpark.sln
```

### 2. Build with Warning-Free Enforcement
```bash
dotnet build InquirySpark.sln -warnaserror
```
- All projects enforce nullable reference types and XML documentation
- npm build runs automatically for `InquirySpark.Web`
- SQLite databases are copied to output directories via `CopyToOutputDirectory`

### 3. Run the Unified Web Application
```bash
dotnet run --project InquirySpark.Web
```
- Default URL: `https://localhost:5002`
- Landing page redirects to `/Unified/Operations` (the unified operations dashboard)
- SQLite databases loaded from `data/sqlite/` in **read-only mode**
- No migrations or schema changes allowed

### 4. Run Tests
```bash
dotnet test InquirySpark.sln
```
- MSTest suite covers shared libraries, navigation, capability services, and audit services
- 99 of 108 tests pass; 5 known `SystemHealthTests` failures are environmental (developer `secrets.json` override)
- 4 tests are skipped (integration scenarios requiring live infrastructure)

## SQLite Database Assets
Immutable SQLite databases are stored in `data/sqlite/`:
- **ControlSparkUser.db**: ASP.NET Core Identity users and roles (read-write for Identity operations)
- **InquirySpark.db**: Survey/inquiry domain data (read-only)

Connection strings in `appsettings.json` use `Mode=ReadOnly` for the inquiry database and `Mode=ReadWriteCreate` for the Identity store. `Database.Migrate()` is disabled — schema changes are applied manually via `sqlite3`.

## Architecture
- **Persistence**: EF Core 10 with Microsoft.Data.Sqlite provider
- **UI**: Bootstrap 5 + DataTables 2.3.5 via npm (CDN-free), Bootswatch theme system, Bootstrap Icons
- **Unified Area**: All user-facing capabilities live under `InquirySpark.Web/Areas/Unified/` — one authenticated session, one navigation model
- **Dependency Injection**: Primary constructors, all services registered in `Program.cs`
- **Audit**: Structured logging via `IUnifiedAuditService` → standard `ILogger` pipeline (no EF audit tables)
- **Response pattern**: All service methods return `BaseResponse<T>` / `BaseResponseCollection<T>` via `DbContextHelper`

## Key Routes
| Path | Description |
|---|---|
| `/` | Redirects to `/Unified/Operations` |
| `/Unified/Operations` | Unified operations dashboard |
| `/Unified/CapabilityCompletionMatrix` | Capability completion/cutover matrix |
| `/Unified/OperationalReadiness` | Operational readiness dashboard |
| `/Unified/OperationsSupport/Health` | System health check (anonymous) |
| `/Identity/Account/Login` | ASP.NET Core Identity sign-in |

## Documentation
- [Project Constitution](.documentation/memory/constitution.md) — primary governance, rules, and validation criteria
- [Unified Web Experience Spec](.documentation/specs/001-unified-web-experience/spec.md)
- [Capability Parity Traceability](.documentation/specs/001-unified-web-experience/contracts/capability-parity-traceability.md)
- [Repo Story (2026-04-12)](.documentation/repo-story/repo-story-2026-04-12.md)

## Troubleshooting
- **Missing .db file errors**: Verify `data/sqlite/*.db` assets exist and connection strings point to correct relative paths
- **SQLite write attempts**: Ensure `Mode=ReadOnly` is present in all connection strings for `InquirySparkConnection`
- **npm build failures**: Run `npm install` manually in the `InquirySpark.Web/` directory, then `npm run build`
- **5 SystemHealthTests failures**: Pre-existing environmental issue — `secrets.json` overrides test env vars; not a code regression

## Contributing
- Read [`.documentation/memory/constitution.md`](.documentation/memory/constitution.md) before making changes
- Active feature specs live under [`.documentation/specs/`](.documentation/specs/)
- Current branch: `001-unified-web-experience`

## License
See [LICENSE](LICENSE) file.
