# InquirySpark Constitution

## Core Principles

### I. Response Wrapper Consistency
Every service returns `BaseResponse<T>` or `BaseResponseCollection<T>` through `DbContextHelper.ExecuteAsync`/`ExecuteCollectionAsync`. The `IsSuccessful`, `Errors`, and `Data` contract is non-negotiable and controllers must surface responses through helpers such as `ApiResponseHelper` instead of direct EF interactions.

### II. Dependency Injection Discipline
All services use constructor injection with primary constructors. Registrations live in `Program.cs`, and no class bypasses the DI container. Logging is injected the same way and exposed through `_logger` provided by `BaseController`/controller constructors.

### III. EF Core Context Stewardship
`InquirySparkContext` is the single source of truth. Prefer existing views (e.g., `VwApplication`, `VwSurveyResponseDetail`) over ad-hoc joins, and honor database naming conventions (`{Entity}Id`, `{Name}Fl`, `{Name}Dt`, etc.).

### IV. Admin UI Standardization
All InquirySpark.Admin CRUD pages follow the Bootstrap 5 + DataTables card template defined in `docs/copilot/session-2025-12-04/BOOTSTRAP5-TABLE-TEMPLATE.md`: card header/actions, `.table` with auto-initialized DataTables, `.no-sort` action columns, Bootswatch-managed CSS, and Bootstrap Icons for affordances.

### V. Documentation & Knowledge Flow
Only the root `README.md` may live outside `/docs`. All Copilot-authored references, guides, or templates must reside under `/docs/copilot/session-YYYY-MM-DD/`. XML documentation comments are required for all public APIs across Common, Repository, and WebApi projects.

## Engineering Constraints & Standards

- **Tech Stack**: .NET 10 solution with WebApi (REST + Swagger), Admin MVC app, Web Razor Pages app, shared logic in Common/Repository.
- **Frontend Assets**: No CDN usage. JavaScript packages (jQuery 3.7.1, Bootstrap 5.3.8 JS, DataTables 2.3.5 + extensions, Bootstrap Icons, validation libs, JSZip/PDFMake) are sourced via npm, copied to `wwwroot/lib` by `npm run build`, and executed automatically on `dotnet build`. CSS originates from the WebSpark.Bootswatch theme system.
- **DataTables Behavior**: `.table` elements auto-initialize with pagination, search, and state saving. Use `data-datatable="false"` to opt out and `.datatable-export` to enable export buttons.
- **Global Usings**: Respect project-level `GlobalUsing.cs` files; add explicit `using` statements only when necessary.
- **Connection Strings**: WebApi/Repository use `InquirySparkContext` (SQL Server). Admin uses `InquirySparkConnection` (SQLite demo) and `ControlSparkUserContextConnection` (Identity). Web uses `DefaultConnection` (SQL Server + Identity).

## Development Workflow & Quality Gates

- **Build Commands**: `dotnet build InquirySpark.sln` for the full solution; project-specific builds (e.g., `dotnet build InquirySpark.Admin/InquirySpark.Admin.csproj`) automatically trigger the npm pipeline.
- **Run Targets**: `dotnet run --project InquirySpark.WebApi` (https://localhost:5001), `InquirySpark.Admin` (https://localhost:7001), `InquirySpark.Web` (https://localhost:5002).
- **Testing**: Use `dotnet test` or target projects such as `InquirySpark.Common.Tests` explicitly.
- **UI Rules**: No inline styles; rely on Bootstrap utility classes. All action columns include `.no-sort`. Buttons follow the standard outlined in the table template (details/edit/delete with Bootstrap Icon glyphs).
- **Common Pitfalls Guardrail**: Never add Bootstrap CSS to npm, never bypass `DbContextHelper`, never query EF entities directly inside controllers, always check for existing DB views before inventing new joins.

## Governance

This constitution mirrors `.github/copilot-instructions.md`. When conflicts arise, update this file and the referenced instructions together. Reviews must confirm adherence to response wrappers, DI, EF stewardship, UI standards, asset pipeline rules, documentation placement, and XML comments. Amendments require documenting rationale, aligning `.github/copilot-instructions.md`, and setting new version metadata.

**Version**: 1.0.0 | **Ratified**: 2025-12-04 | **Last Amended**: 2025-12-04
