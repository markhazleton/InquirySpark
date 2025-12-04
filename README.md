# InquirySpark
Spark Your Inquiry, Ignite Insights.

## Overview
InquirySpark is a .NET 10 survey/inquiry management system with an MVC admin interface, powered by read-only SQLite databases. The solution includes:

- **InquirySpark.Admin**: Bootstrap 5 + DataTables admin interface
- **InquirySpark.Repository**: EF Core 10 services and DbContext abstractions
- **InquirySpark.Common**: Shared models, SDK objects, response wrappers
- **InquirySpark.Common.Tests**: MSTest unit test suite

## Prerequisites
- **.NET SDK 10.0.100** (enforced via global.json)
- **Node.js 20.x** for Admin asset pipeline (npm build runs automatically)
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
dotnet build InquirySpark.sln
```
- All projects use nullable reference types and XML documentation
- npm build runs automatically for InquirySpark.Admin
- SQLite databases are copied to output directories

### 3. Run the Admin Application
```bash
dotnet run --project InquirySpark.Admin
```
- Default URL: `https://localhost:7001`
- SQLite databases loaded from `data/sqlite/` in **read-only mode**
- No migrations or schema changes allowed

### 4. Run Tests
```bash
dotnet test InquirySpark.sln
```
- MSTest suite covers shared libraries and SQLite provider configuration
- Integration tests ensure read-only enforcement

## SQLite Database Assets
Immutable SQLite databases are stored in `data/sqlite/`:
- **ControlSparkUser.db**: ASP.NET Core Identity users/roles
- **InquirySpark.db**: Survey/inquiry domain data

Connection strings in appsettings.json use `Mode=ReadOnly` to prevent schema/data mutations. See [docs/copilot/session-2025-12-04/sqlite-data-assets.md](docs/copilot/session-2025-12-04/sqlite-data-assets.md) for checksums and distribution policy.

## Architecture
- **Persistence**: EF Core 10 with Microsoft.Data.Sqlite provider
- **Configuration**: Centralized via `SqliteOptionsConfigurator` and `PersistenceProviderConfig`
- **UI Patterns**: Bootstrap 5 utilities, DataTables auto-initialization, Bootswatch theme switching
- **Dependency Injection**: Primary constructors, service registration in Program.cs

## Documentation
- [Bootstrap 5 Table Template](docs/copilot/session-2025-12-04/BOOTSTRAP5-TABLE-TEMPLATE.md)
- [NPM Build Process](docs/copilot/session-2025-12-04/NPM-BUILD.md)
- [CDN-Free Implementation](docs/copilot/session-2025-12-04/CDN-FREE-IMPLEMENTATION.md)
- [DataTables Reference](docs/copilot/session-2025-12-04/DATATABLES-REFERENCE.md)

## Troubleshooting
- **Missing .db file errors**: Verify `data/sqlite/*.db` assets exist and connection strings point to correct relative paths
- **SQLite write attempts**: Ensure `Mode=ReadOnly` is present in all connection strings
- **npm build failures**: Run `npm install` manually in InquirySpark.Admin directory
- **XML doc warnings**: These are informational; Phase 4 tasks will address documentation coverage

## Contributing
See [specs/001-remove-sql-server/](specs/001-remove-sql-server/) for current feature specifications and implementation tasks.

## License
See [LICENSE](LICENSE) file.
