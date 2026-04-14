# InquirySpark AI Coding Agent Instructions

> **Governance**: This file is a practical reference for AI coding agents. The authoritative source of engineering principles, rules, and validation criteria is the **project constitution** at [`.documentation/memory/constitution.md`](../.documentation/memory/constitution.md). When in doubt, consult the constitution. Any conflict between this file and the constitution is resolved in favor of the constitution.

## Project Overview
InquirySpark is a .NET 10 unified survey/inquiry and decision-management system with one active web application:
- **InquirySpark.Web** - Unified operations workspace (MVC + Razor Views, Areas, Bootstrap 5 + DataTables)

Core business logic resides in **InquirySpark.Common** (shared models/SDK) and **InquirySpark.Repository** (EF Core + services).

> **Decommissioned**: `InquirySpark.Admin` and `DecisionSpark` have been removed from the active solution (spec `001-unified-web-experience`). Their capabilities are delivered by `InquirySpark.Web/Areas/Unified/`.

## Architecture Patterns

### Response Wrapper Pattern
All service methods return `BaseResponse<T>` or `BaseResponseCollection<T>` for consistent error handling:

```csharp
// Single item
public async Task<BaseResponse<SurveyItem>> GetSurveyBySurveyId(int surveyId)
{
    return await DbContextHelper.ExecuteAsync<SurveyItem>(async () =>
    {
        return await _context.Surveys
            .Where(w => w.SurveyId == surveyId)
            .Include(i => i.QuestionGroups)
            .Select(s => SurveyServices_Mappers.Create(s))
            .FirstOrDefaultAsync();
    });
}
```

**Key Points:**
- `DbContextHelper.ExecuteAsync` wraps EF Core operations with exception handling
- Returns `IsSuccessful` boolean and `Errors` collection
- `Data` property contains the result (null if failed)
- Use `ExecuteCollectionAsync` for List<T> results

### Dependency Injection Pattern
Services follow constructor injection with primary constructors:

```csharp
public class SurveyService(InquirySparkContext context, ILogger<SurveyService> logger) : ISurveyService
{
    private readonly InquirySparkContext _context = context;
    private readonly ILogger<SurveyService> _logger = logger;
}
```

Register services in Program.cs:
```csharp
builder.Services.AddDbContext<InquirySparkContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddTransient<ISurveyService, SurveyService>();
```

### EF Core Context Pattern
`InquirySparkContext` contains 40+ DbSets including:
- Entity tables (Application, Survey, Question, Company)
- Lookup tables (LuApplicationType, LuSurveyType, LuQuestionType)
- View mappings (VwApplication, VwQuestionLibrary, VwSurveyResponseDetail)

**Views are heavily used for complex queries** - check for existing views before writing complex joins.

## Database Conventions

### Naming Patterns
- Primary Keys: `{Entity}Id` (e.g., `SurveyId`, `QuestionId`)
- Foreign Keys: `{Entity}Id` (e.g., `SurveyTypeId`)
- Boolean flags: `{Name}Fl` (e.g., `ActiveFl`, `CommentFl`)
- Descriptions: `{Name}Ds` (e.g., `QuestionDs`, `SurveyDs`)
- Names: `{Name}Nm` (e.g., `SurveyNm`, `SurveyShortNm`)
- Codes: `{Name}Cd` (e.g., `ApplicationCd`)
- Dates: `{Name}Dt` (e.g., `ModifiedDt`, `StartDt`)
- Audit fields: `ModifiedDt`, `ModifiedId` (present on most tables)

### Connection Strings
- **All projects**: SQLite exclusively (SQL Server removed — see `specs/001-remove-sql-server`)
- **InquirySpark.Web**: `InquirySparkConnection` (SQLite, read-only) + `ControlSparkUserContextConnection` (Identity SQLite, read-write)
- Format: `Data Source=<path>;Mode=ReadOnly` for inquiry data; `Mode=ReadWriteCreate` for Identity
- SQLite `.db` assets: `Database.Migrate()` is disabled — schema changes are applied manually via `sqlite3`

## Unified Web UI Conventions (InquirySpark.Web)

All capability views in `InquirySpark.Web/Areas/Unified/` follow the Bootstrap 5 + DataTables card template.

### Bootstrap 5 + DataTables Standard
**All list views follow this pattern:**

```html
<div class="card border-0 shadow-sm">
    <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
        <h2 class="mb-0"><i class="bi bi-{icon}"></i> {Title}</h2>
        <a asp-action="Create" class="btn btn-light btn-sm">
            <i class="bi bi-plus-circle"></i> Create New
        </a>
    </div>
    <div class="card-body p-0">
        <div class="table-responsive">
            <table class="table table-striped table-hover align-middle mb-0">
                <thead class="table-light">
                    <tr>
                        <th scope="col"><i class="bi bi-{icon}"></i> Column Name</th>
                        <th scope="col" class="no-sort">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var item in Model)
                    {
                        <tr>
                            <td>@item.Property</td>
                            <td>
                                <div class="btn-group btn-group-sm" role="group">
                                    <a asp-action="Details" asp-route-id="@item.Id" class="btn btn-outline-info" title="View Details">
                                        <i class="bi bi-eye"></i>
                                    </a>
                                    <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-outline-primary" title="Edit">
                                        <i class="bi bi-pencil"></i>
                                    </a>
                                    <a asp-action="Delete" asp-route-id="@item.Id" class="btn btn-outline-danger" title="Delete">
                                        <i class="bi bi-trash"></i>
                                    </a>
                                </div>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </div>
    <div class="card-footer text-muted d-flex justify-content-between align-items-center">
        <span><i class="bi bi-info-circle"></i> Total: @Model.Count() records</span>
        <span><i class="bi bi-lightning"></i> DataTables enabled</span>
    </div>
</div>
```

**Key Requirements:**
- Use Bootstrap utility classes (no inline styles, no custom CSS)
- Use Bootstrap Icons (bi-*) for visual indicators
- DataTables auto-initializes on `.table` class
- Add `class="no-sort"` to action columns
- All `<th>` elements use `scope="col"` for accessibility
- Card layout: `card border-0 shadow-sm` → header → `card-body p-0` → footer
- Authorization: every controller or action must have `[Authorize]` or `[AllowAnonymous]`

### NPM Build System (100% CDN-Free)
Frontend dependencies are managed via npm in `InquirySpark.Web/`:

```bash
# Install dependencies
npm install

# Build (copies to wwwroot/lib)
npm run build

# Automatically runs on dotnet build
```

**Libraries Included:**
- jQuery 3.7.1, Bootstrap 5.3.8 (JS only, CSS from Bootswatch)
- DataTables 2.3.5 with Bootstrap 5 integration
- DataTables Extensions: Buttons, Responsive, Select, SearchPanes
- Bootstrap Icons 1.13.1
- jQuery Validation (for ASP.NET Core forms)
- JSZip & PDFMake (for DataTables export)

**Critical:** Bootstrap CSS comes from WebSpark.Bootswatch theme system, NOT npm. Only JavaScript is local.

### DataTables Configuration
Configured in `InquirySpark.Web/wwwroot/js/unified-app.js`:
- Auto-initializes all `.table` elements
- Default: 25 rows, sorting, searching, pagination
- Add `data-datatable="false"` to disable
- Add `.datatable-export` class for Excel/PDF/CSV export buttons
- Add `class="no-sort"` to `<th>` to disable column sorting
- State saving enabled (24-hour persistence)

### Bootswatch Theme Integration
Dynamic theme switching via WebSpark.Bootswatch package:
- 28+ themes with light/dark modes
- CSS served from embedded resources (no CDN)
- JavaScript from local npm package
- Theme persists in cookies

## Global Usings

### InquirySpark.Web
```csharp
// Uses GlobalUsing.cs — check before adding explicit usings
global using Microsoft.AspNetCore.Mvc;
```

### InquirySpark.Repository
Check existing global usings before adding namespace imports.

## Controller Patterns

### Unified Area Controllers
Seal controllers and use primary constructors; all require `[Area("Unified")]` and `[Authorize]`:

```csharp
[Area("Unified")]
[Authorize]
public sealed class InquiryAuthoringController(
    InquirySparkContext context) : Controller
{
    // Direct EF read queries are acceptable in Unified area controllers
    // (InquirySparkContext is read-only SQLite — no DbContextHelper wrapping needed)
}
```

### API Controllers
Use primary constructors with ApiController attribute:

```csharp
[ApiController]
[Route("api/[controller]")]
public class SurveyController(ISurveyService service, ILogger<SurveyController> logger) : ControllerBase
{
    protected readonly ISurveyService _service = service;
    protected readonly ILogger<SurveyController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return await ApiResponseHelper.ExecuteAsync(() => _service.GetSurveyCollection(), _logger);
    }
}
```

`ApiResponseHelper` converts `BaseResponse<T>` to appropriate IActionResult (200/400/404/500).

## Build & Test Commands

### Building
```powershell
# Build entire solution (zero-warning gate)
dotnet build InquirySpark.sln -warnaserror

# Build specific project (also runs npm pipeline)
dotnet build InquirySpark.Web/InquirySpark.Web.csproj
```

### Running
```powershell
# Unified Web Application (default: https://localhost:5002)
dotnet run --project InquirySpark.Web
```

### Testing
```powershell
# Run all tests
dotnet test InquirySpark.sln

# Run specific test project
dotnet test InquirySpark.Common.Tests
```

## Common Pitfalls

1. **Don't add Bootstrap CSS to npm** - Themes come from Bootswatch, only JS is local
2. **Don't use inline styles** - Use Bootstrap utility classes exclusively
3. **Don't forget `DbContextHelper.ExecuteAsync`** - Repository service methods must wrap EF operations
4. **Don't query entities directly in controllers outside Unified area** - Use services and response wrappers
5. **Check for existing DB views** - Database has many pre-built views for complex queries
6. **Don't bypass DataTables** - All tables should auto-initialize unless explicitly disabled with `data-datatable="false"`
7. **Use global usings** - Check `GlobalUsing.cs` before adding repetitive using statements
8. **Don't reference InquirySpark.Admin or DecisionSpark** - Both are decommissioned; all capability work goes in `InquirySpark.Web/Areas/Unified/`

## XML Documentation
All public APIs require XML doc comments (enabled in Common, Repository, and Web projects):

```csharp
/// <summary>
/// Gets survey by unique identifier.
/// </summary>
/// <param name="surveyId">The survey identifier.</param>
/// <returns>Survey item with question groups and members.</returns>
public async Task<BaseResponse<SurveyItem>> GetSurveyBySurveyId(int surveyId)
```

## Documentation File Organization

**CRITICAL: Governed by [`.documentation/memory/constitution.md`](../.documentation/memory/constitution.md) — Constitution § V.**

- **`README.md` only**: The only `.md` file permitted at the project root
- **All other documentation**: MUST be placed in `/.documentation/` subdirectories
  - Specs: `/.documentation/specs/{spec-id}/`
  - Session notes: `/.documentation/copilot/session-YYYY-MM-DD/`
  - Governance: `/.github/` (copilot-instructions.md and approved governance files only)
  - The legacy `docs/` directory is **retired** — do not create files there

**Examples:**
- ✅ `/.documentation/specs/001-unified-web-experience/plan.md`
- ✅ `/.documentation/copilot/session-2026-04-12/NOTES.md`
- ✅ `/README.md`
- ❌ `docs/anything.md`
- ❌ `InquirySpark.Web/NOTES.md`

## References
- [Project Constitution](../.documentation/memory/constitution.md) — primary governance, rules & validation
- [Unified Web Experience Spec](../.documentation/specs/001-unified-web-experience/spec.md)
- [Capability Parity Traceability](../.documentation/specs/001-unified-web-experience/contracts/capability-parity-traceability.md)
- [UX Conventions](../.documentation/specs/001-unified-web-experience/contracts/unified-ux-conventions.md)
