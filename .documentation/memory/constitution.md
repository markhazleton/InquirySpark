# InquirySpark Constitution

## Core Principles

### I. Response Wrapper Consistency
Every service returns `BaseResponse<T>` or `BaseResponseCollection<T>` through `DbContextHelper.ExecuteAsync`/`ExecuteCollectionAsync`. The `IsSuccessful`, `Errors`, and `Data` contract is non-negotiable and controllers must surface responses through helpers such as `ApiResponseHelper` instead of direct EF interactions.

### II. Dependency Injection Discipline
All services use constructor injection with primary constructors. Registrations live in `Program.cs`, and no class bypasses the DI container. Logging is injected the same way and exposed through `_logger` provided by `BaseController`/controller constructors.

### III. EF Core Context Stewardship
`InquirySparkContext` is the single source of truth. Prefer existing views (e.g., `VwApplication`, `VwSurveyResponseDetail`) over ad-hoc joins, and honor database naming conventions (`{Entity}Id`, `{Name}Fl`, `{Name}Dt`, etc.).

### IV. Admin UI Standardization
All InquirySpark.Admin CRUD pages follow the Bootstrap 5 + DataTables card template: card header/actions, `.table` with auto-initialized DataTables, `.no-sort` action columns, Bootswatch-managed CSS, and Bootstrap Icons for affordances. No inline styles. All action columns use `.no-sort`. Buttons follow the standard details/edit/delete pattern with Bootstrap Icon glyphs.

### V. Documentation & Knowledge Flow
**All generated documentation MUST live under `/.documentation/`.** The only files permitted outside `/.documentation/` are `README.md` (project root) and governance files in `.github/`. The legacy `docs/` directory is retired and must not be used. Session documentation is placed in `/.documentation/copilot/session-YYYY-MM-DD/` and archived to `.archive/` when no longer active. XML documentation comments are required for all public APIs across Common, Repository, and WebApi projects.

**Enforcement**: Any AI agent or developer creating a `.md` file outside `/.documentation/` (except `README.md` and `.github/`) is in violation of this constitution and the file must be moved.

## Engineering Constraints & Standards

- **Tech Stack**: .NET 10 solution with WebApi (REST + Swagger), Admin MVC app, and Web MVC app using controllers and Razor views, with Razor Pages enabled where shared Identity UI requires it. Shared logic resides in Common/Repository.
- **Frontend Assets**: No CDN usage. JavaScript packages (jQuery 3.7.1, Bootstrap 5.3.8 JS, DataTables 2.3.5 + extensions, Bootstrap Icons, validation libs, JSZip/PDFMake) are sourced via npm, copied to `wwwroot/lib` by `npm run build`, and executed automatically on `dotnet build`. CSS originates from the WebSpark.Bootswatch theme system.
- **DataTables Behavior**: `.table` elements auto-initialize with pagination, search, and state saving. Use `data-datatable="false"` to opt out and `.datatable-export` to enable export buttons.
- **Global Usings**: Respect project-level `GlobalUsing.cs` files; add explicit `using` statements only when necessary.
- **Connection Strings**: All projects use SQLite exclusively (SQL Server removed in `specs/001-remove-sql-server`). Admin uses `InquirySparkConnection` (SQLite, read-only) and `ControlSparkUserContextConnection` (Identity SQLite). Connection strings use `Data Source=...;Mode=ReadOnly` format. SQLite `.db` assets are immutable — `Database.Migrate()` is disabled.

## Development Workflow & Quality Gates

- **Build Commands**: `dotnet build InquirySpark.sln` for routine local builds; `dotnet build InquirySpark.sln -warnaserror` is the required zero-warning validation gate before merge. Project-specific builds (e.g., `dotnet build InquirySpark.Admin/InquirySpark.Admin.csproj`) automatically trigger the npm pipeline.
- **Run Targets**: `dotnet run --project InquirySpark.WebApi` (https://localhost:5001), `InquirySpark.Admin` (https://localhost:7001), `InquirySpark.Web` (https://localhost:5002).
- **Testing**: Use `dotnet test` or target projects such as `InquirySpark.Common.Tests` explicitly.
- **UI Rules**: No inline styles; rely on Bootstrap utility classes. All action columns include `.no-sort`. Buttons follow the standard outlined in the table template (details/edit/delete with Bootstrap Icon glyphs).
- **Common Pitfalls Guardrail**: Never add Bootstrap CSS to npm, never bypass `DbContextHelper`, never query EF entities directly inside controllers, always check for existing DB views before inventing new joins.

## Governance

**This constitution is the primary source of truth** for engineering principles, conventions, and rules in this repository. `.github/copilot-instructions.md` is a practical agent-facing reference that must defer to and align with this constitution — not the other way around.

### Authority & Conflict Resolution
- When a rule appears in both this constitution and `copilot-instructions.md`, **this constitution governs**.
- When `copilot-instructions.md` conflicts with or omits a rule defined here, `copilot-instructions.md` must be updated to align.
- AI agents reading `copilot-instructions.md` are directed to consult this constitution for complete rules, validation criteria, and rationale.

### Compliance Checklist (all PRs and AI sessions)
- [ ] Response wrappers used in all service methods
- [ ] DI used exclusively (no `new` for services)
- [ ] EF queries routed through views or `DbContextHelper`
- [ ] Admin UI follows Bootstrap 5 + DataTables card template
- [ ] No SQL Server references remain
- [ ] **No documentation files created outside `/.documentation/`** (except `README.md` and `.github/`)
- [ ] XML doc comments present on all public APIs
- [ ] Build passes with `dotnet build -warnaserror` (zero warnings)

### Amendment Process
Amendments require: (1) documenting rationale in the change, (2) updating `copilot-instructions.md` to reflect the change, and (3) incrementing the version metadata below.

**Version**: 1.2.0 | **Ratified**: 2025-12-04 | **Last Amended**: 2026-04-12
