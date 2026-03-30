src/
tests/
ios/ or android/
# Implementation Plan: Benchmark Insights & Reporting Platform

**Branch**: `[001-benchmark-insights]` | **Date**: December 14, 2025 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-benchmark-insights/spec.md`

## Summary

InquirySpark needs a reusable insights platform that lets analysts configure chart definitions once, regenerate assets in bulk, browse/search approved visuals, inspect the underlying data, and present gauge dashboards with drill-down paths. We will extend the existing .NET 10 stack by adding charting/domain services inside `InquirySpark.Repository`, REST API endpoints in `InquirySpark.Admin` (Controllers/Api folder), and new MVC experiences plus JavaScript bundles in `InquirySpark.Admin`. Batch rendering relies on Hangfire + Azure Service Bus, storage targets Azure Blob + CDN, and discovery runs through Azure Cognitive Search so that asset queries remain sub-second at 100K+ charts.

**Note**: The InquirySpark.Admin application serves dual purposes: MVC UI for administrative tasks and RESTful API endpoints for programmatic access.

## Technical Context

**Language/Version**: C# 13 on .NET 10 (Admin + Repository + Web)  
**Primary Dependencies**: ASP.NET Core MVC/WebApi, EF Core 10, Hangfire 1.8, Azure Service Bus, Azure Blob Storage SDK, Azure Cognitive Search SDK, Chart.js 4 + chartjs-node-canvas, DataTables 2.3.5, JSZip/PDFMake for exports  
**Storage**: SQLite (`InquirySparkContext`) for OLTP + Hangfire metadata, Azure Blob Storage for rendered assets/exports, Azure Cognitive Search index for chart discovery, Azure Service Bus queues/topics for batch fan-out  
**Testing**: `dotnet test` suite (xUnit-based Common/Repository projects) plus new integration tests that exercise chart builder APIs and Hangfire job pipelines; UI smoke checks via existing Admin automation harness  
**Target Platform**: Web applications (InquirySpark.Admin with dual MVC/API hosting, InquirySpark.Web) running on Windows/Linux hosts or Azure App Service; Hangfire workers run within Admin application  
**Project Type**: Multi-tier enterprise web (API + MVC admin + background workers)  
**Performance Goals**: Per spec—Chart builder loads <2s, preview rerenders <1s, filter ops <500 ms up to 100K rows, batch throughput >=20 charts/min, dashboards render 50 gauges <3s, drill-down <1s  
**Constraints**: Response wrapper contracts via `BaseResponse<T>`, DI-only services, EF queries through `InquirySparkContext` + pre-existing views, Admin UI must follow Bootstrap/DataTables template with no inline styles/CDN assets, documentation confined to `docs/copilot/session-YYYY-MM-DD`, assets stored off-box with encryption, policy-based authorization must guard every new API, and telemetry must prove success metrics
**Scale/Scope**: Targeting 100K chart assets, 1M-row datasets for exports, 50+ gauge tiles per dashboard, concurrent analysts across Admin + API consumers

## Constitution Check

**Pre-Design Gate (before research)**
- *Response Wrapper Consistency* — Plan routes all persistence/business logic through new repository services that return `BaseResponse<T>`/`BaseResponseCollection<T>` via `DbContextHelper.ExecuteAsync`. Controllers stay thin and use `ApiResponseHelper`. **Status: PASS**
- *Dependency Injection Discipline* — Hangfire jobs, search/indexers, and services are registered via `Program.cs` and injected using primary constructors. No static service locators are introduced. **Status: PASS**
- *EF Core Context Stewardship* — New queries leverage existing InquirySpark views where possible and follow naming conventions for new tables (e.g., `ChartDefinitionId`, `ActiveFl`). **Status: PASS**
- *Admin UI Standardization* — Chart builder, asset library, and deck/dashboards will reuse the Bootstrap 5 + DataTables card layout with `.no-sort` action columns and Bootswatch theming; JS bundles continue to ship via npm/webpack without CDNs. **Status: PASS**
- *Documentation & Knowledge Flow* — All planning artifacts live under `specs/001-benchmark-insights/` and future references go under `docs/copilot/session-2025-12-14/` as required. **Status: PASS**

**Post-Design Revalidation**
- Data model + contracts maintain response wrapper boundaries, ensure new tables honor naming conventions, and keep Admin UX tied to the standard template. No constitution exceptions were introduced during design. **Status: PASS**

## Project Structure

### Documentation (this feature)

```text
specs/001-benchmark-insights/
├── plan.md          # Implementation plan (this file)
├── research.md      # Phase 0 decisions (API stack, storage, search, grid)
├── data-model.md    # Entity definitions + state machines
├── quickstart.md    # Environment + runbook steps
├── contracts/
│   └── charting-api.yaml  # OpenAPI slice for charting assets
└── tasks.md         # Will be produced by /speckit.tasks
```

### Source Code (repository impact)

```text
InquirySpark.Repository/
├── Services/
│   ├── Charting/
│   │   ├── ChartDefinitionService.cs
│   │   ├── ChartBuildService.cs
│   │   └── ChartAssetService.cs
│   ├── Decks/DeckProjectService.cs
│   └── Dashboards/DashboardService.cs
├── Models/Charting/
│   ├── ChartDefinitionDto.cs
│   ├── ChartAssetDto.cs
│   └── MetricGroupDto.cs
└── Jobs/
    └── ChartRenderOrchestrator.cs

InquirySpark.Admin/
├── Controllers/Api/
│   ├── ChartDefinitionsController.cs
│   ├── ChartBuildsController.cs
│   ├── ChartAssetsController.cs
│   ├── DecksController.cs
│   ├── DashboardsController.cs
│   ├── DataExplorerController.cs
│   └── UserPreferencesController.cs
├── BackgroundWorkers/
│   ├── HangfireStartup.cs
│   └── ServiceBusListeners.cs
├── Contracts/Requests/
│   └── ChartDefinitionRequest.cs
├── Areas/Inquiry/Controllers/
│   ├── ChartBuilderController.cs
│   ├── ChartAssetsController.cs
│   ├── DecksController.cs
│   └── DashboardsController.cs
├── Areas/Inquiry/Views/ChartBuilder/
│   ├── Index.cshtml
│   └── _FilterPanelPartial.cshtml
├── Areas/Inquiry/Views/ChartAssets/
│   ├── Index.cshtml
│   └── _PreviewPanel.cshtml
├── wwwroot/src/js/chart-builder/
│   ├── filters.ts
│   ├── chart-preview.ts
│   └── property-panel.ts
└── wwwroot/src/js/dashboards/
    ├── gauges.ts
    └── drilldown.ts

Docs & Infrastructure
├── docs/copilot/session-2025-12-14/ (future how-tos, index templates)
└── eng/ (existing build scripts; may gain Hangfire provisioning helper)
```

**Structure Decision**: Reuse the existing three-application solution. Repository centralizes business logic + EF. WebApi exposes REST endpoints and hosts Hangfire/background listeners. Admin MVC gains new areas, controllers, and JS bundles for chart builder, asset library, and dashboards. No additional standalone services are required; Service Bus/Hangfire integrations stay co-located with WebApi for simpler deployments.

## Cross-Cutting Enhancements

- **Security & Auditing**: Introduce `AuditLog` tables plus `AuditLogService` inside Repository. All new controllers (chart definitions, builds, assets, decks, dashboards, data explorer, exports) use policy-based authorization mapped to Analyst/Operator/Consultant/Executive roles and emit audit entries via `ApiResponseHelper` wrappers.
- **User Preference Service**: Add `UserPreference` table with JSON payloads keyed by user + surface slug. Repository exposes `UserPreferenceService`, WebApi adds `UserPreferencesController`, and Admin UI integrates save/restore flows for chart builder, explorer, decks, and dashboards.
- **Auto-Approval Workflow**: Extend `ChartDefinitionService` to run validation pipelines (dataset availability, schema validation, formula safety). Approved definitions automatically set `ApprovalFl`, log approver metadata, and immediately publish to the asset catalog.
- **Calculated Fields Engine**: Add `FormulaParserService` (likely leveraging NCalc or a custom AST) with compatibility checks by chart type. Admin UI gets a guided formula editor with autocomplete, validation badges, and inline errors.
- **Asset Usage Analytics**: Track impressions, previews, downloads, shares, and add-to-deck events via `ChartAssetUsageService` writing to SQL + Application Insights. Asset library UI surfaces usage counters, trending charts, and filters.
- **Read-Only Data Explorer**: `DataExplorerService` enforces read-only scopes, redacts mutation verbs, watermarks exports, and logs any denied write attempts for audit.
- **Telemetry for Success Criteria**: Application Insights/OpenTelemetry captures chart builder funnels (SC-001), reuse events (SC-004), satisfaction prompts + survey submissions (SC-006), and existing perf metrics (SC-002/SC-003/SC-005). Dashboards aggregate these KPIs for operations.

## Complexity Tracking

No constitution violations or extra projects beyond the standard InquirySpark stack were introduced, so no exceptions are required.
