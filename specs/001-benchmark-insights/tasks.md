# Tasks: Benchmark Insights & Reporting Platform

**Input**: Design documents from `/specs/001-benchmark-insights/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Not explicitly requested; focus on implementation tasks with independent verification per user story.

**Organization**: Tasks are grouped by user story (US1–US5) after shared Setup and Foundational phases.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish documentation trail and configuration scaffolding required by all subsequent work.

- [ ] T001 Create docs/copilot/session-2025-12-14/README.md summarizing Benchmark Insights scope, linking to spec.md/plan.md, and listing prerequisite tooling.
- [ ] T002 Add Charting:Storage, Search, ServiceBus, and BatchRendering configuration sections to InquirySpark.WebApi/appsettings.json and InquirySpark.WebApi/appsettings.Development.json so downstream services can bind strongly typed options.
- [ ] T003 Update InquirySpark.Admin/package.json to include Chart.js 4, chartjs-plugin-datalabels, chartjs-node-canvas bridge scripts, and DataTables 2.3.5 export bundles required by the new chart builder assets.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Database schema, EF Core entities, DTOs, and DI wiring that every story relies on.

**⚠️ CRITICAL**: Complete this phase before starting any user story.

- [ ] T004 Create InquirySpark.Database/dbo/Tables/ChartDefinition.sql, ChartVersion.sql, ChartBuildJob.sql, and ChartBuildTask.sql with primary/foreign keys, status enums, and audit columns that match data-model.md.
- [ ] T005 Create InquirySpark.Database/dbo/Tables/ChartAsset.sql, ChartAssetFile.sql, and DataExportRequest.sql with retention metadata, blob paths, and export formats.
- [ ] T006 Create InquirySpark.Database/dbo/Tables/DeckProject.sql, DeckSlide.sql, DashboardDefinition.sql, GaugeTile.sql, MetricGroup.sql, and MetricScoreSnapshot.sql covering hierarchy depth limits and drilldown relationships.
- [ ] T007 Add EF Core entity classes under InquirySpark.Repository/Database/Entities/Charting/ (ChartDefinitionEntity, ChartVersionEntity, ChartBuildJobEntity, ChartBuildTaskEntity, ChartAssetEntity, ChartAssetFileEntity, DeckProjectEntity, DeckSlideEntity, DashboardDefinitionEntity, GaugeTileEntity, MetricGroupEntity, MetricScoreSnapshotEntity, DataExportRequestEntity) with navigation properties matching the new tables.
- [ ] T008 Update InquirySpark.Repository/InquirySparkContext.cs to expose DbSet properties for every new entity plus Fluent API configurations (composite keys, enum conversions, precision rules).
- [ ] T009 Add DTOs in InquirySpark.Repository/Models/Charting/ChartDefinitionDto.cs, ChartAssetDto.cs, and MetricGroupDto.cs reflecting response payloads shared across services and controllers.
- [ ] T010 Create InquirySpark.Repository/Mapping/ChartingProfile.cs to translate between EF entities and DTOs, including nested mapping for versions, assets, files, and deck slides.
- [ ] T011 Register new options and services in InquirySpark.WebApi/Program.cs and InquirySpark.Repository/ServiceCollectionExtensions.cs (Charting services, Hangfire server, Azure Service Bus clients, Azure Blob clients, Azure Cognitive Search client).
- [ ] T054 Create InquirySpark.Database/dbo/Tables/AuditLog.sql and InquirySpark.Repository/Database/Entities/Security/AuditLogEntity.cs with indexes on ActorId, EntityType, and CreatedDt, plus DbContext wiring.
- [ ] T055 Implement InquirySpark.Repository/Services/Security/AuditLogService.cs and policy/role helpers (Analyst/Operator/Consultant/Executive) with DI registration and unit tests for BaseResponse logging.
- [ ] T056 Apply policy-based `[Authorize(Policy="...")]` attributes to all new WebApi controllers plus Admin `BaseController`, ensuring restricted actions hide/disable in Razor views and write audit records through AuditLogService.
- [ ] T057 Create InquirySpark.Database/dbo/Tables/UserPreference.sql, corresponding EF entity, and Fluent configuration to store per-user JSON payloads with optimistic concurrency.
- [ ] T058 Implement InquirySpark.Repository/Services/UserPreferences/UserPreferenceService.cs and InquirySpark.WebApi/Controllers/UserPreferencesController.cs to save, retrieve, and delete preference payloads for builder, explorer, decks, and dashboards.

---

## Phase 3: User Story 1 - Reusable Chart Definitions (Priority: P1) 🎯 MVP

**Goal**: Analysts configure chart definitions once (dataset selection, filters, Chart.js styling) and reuse them without engineering support.

**Independent Test**: Provision a dataset, configure a chart, save it, reopen with a different slice, and confirm the preview updates without developer intervention.

### Implementation for User Story 1

- [ ] T012 [P] [US1] Create InquirySpark.Repository/Services/Charting/ChartDefinitionService.cs with dataset catalog listing, GetDefinitionAsync, SaveDefinitionAsync, and DeleteAsync methods using DbContextHelper + BaseResponse patterns.
- [ ] T013 [US1] Extend ChartDefinitionService with version history, optimistic locking, and rollback helpers to persist ChartVersion records and enforce concurrency tokens in InquirySpark.Repository/Services/Charting/ChartDefinitionService.cs.
- [ ] T014 [P] [US1] Add contract models InquirySpark.WebApi/Contracts/Requests/ChartDefinitionRequest.cs and Contracts/Responses/ChartDefinitionResponse.cs capturing datasetId, filterPayload, visualPayload, calculationPayload, and version metadata.
- [ ] T015 [US1] Implement InquirySpark.WebApi/Controllers/ChartDefinitionsController.cs with CRUD, version history, and rollback endpoints wired through ApiResponseHelper and ChartDefinitionService.
- [ ] T016 [P] [US1] Add InquirySpark.Admin/Areas/Inquiry/Controllers/ChartBuilderController.cs with Index, Create, Edit, Preview, and VersionHistory actions enforcing Admin UI conventions.
- [ ] T017 [P] [US1] Build Razor views in InquirySpark.Admin/Areas/Inquiry/Views/ChartBuilder/ (Index.cshtml, _FilterPanelPartial.cshtml, _PropertyPanel.cshtml, _VersionTimeline.cshtml, _DatasetModal.cshtml) following Bootstrap 5 + DataTables template.
- [ ] T018 [P] [US1] Implement Chart.js-driven modules in InquirySpark.Admin/wwwroot/src/js/chart-builder/filters.ts, chart-preview.ts, and property-panel.ts with debounced updates (<300 ms) and saved state restoration.
- [ ] T019 [US1] Wire new chart builder bundles into InquirySpark.Admin/build/webpack.common.js and webpack.dev.js so npm run build outputs wwwroot/js/chart-builder.bundle.js referenced by the ChartBuilder views.
- [ ] T020 [US1] Add dataset catalog DataTable initialization, dataset field mapping modal, and Save As workflow in InquirySpark.Admin/Areas/Inquiry/Views/ChartBuilder/_DatasetModal.cshtml plus accompanying TypeScript interop.
- [ ] T021 [US1] Introduce schema validation helpers using Newtonsoft.Json.Schema in InquirySpark.Repository/Services/Charting/ChartDefinitionService.cs to validate filterPayload and calculationPayload before persisting definitions.
- [ ] T059 [US1] Integrate UserPreferenceService with ChartBuilderController and TypeScript bundles so per-user view modes, panel layouts, and drafts are loaded on entry and saved on changes.
- [ ] T060 [US1] Extend ChartDefinitionService, WebApi ChartDefinitionsController, and Admin UI to run validation pipelines (dataset availability, schema validation, formula safety) and auto-approve definitions with audit logging + UI badges.
- [ ] T061 [P] [US1] Implement InquirySpark.Repository/Services/Charting/FormulaParserService.cs (or integrate NCalc) with compatibility checks per chart type plus unit tests for supported functions.
- [ ] T062 [US1] Build a guided formula editor experience in ChartBuilder views + chart-builder TypeScript (function picker, column autocomplete, inline errors) that calls FormulaParserService validation APIs before save.

---

## Phase 4: User Story 2 - Automated Chart Rebuilds & Asset Library (Priority: P1)

**Goal**: Operators trigger or schedule batch jobs that rebuild every approved chart definition, store each rendition, and expose downloadable assets with telemetry.

**Independent Test**: Execute Build All against sample definitions, monitor job telemetry, and verify assets (PNG/SVG/PDF/JPEG) plus metadata are cataloged with version tags.

### Implementation for User Story 2

- [ ] T022 [P] [US2] Create InquirySpark.Repository/Services/Charting/ChartBuildService.cs to queue ChartBuildJob + ChartBuildTask records, apply retries, and summarize throughput stats.
- [ ] T023 [US2] Create InquirySpark.Repository/Services/Charting/ChartAssetService.cs to manage asset metadata, Blob paths, and approval states, exposing methods consumed by batch jobs and library queries.
- [ ] T024 [US2] Implement InquirySpark.Repository/Jobs/ChartRenderOrchestrator.cs (Hangfire job) to iterate active definitions, enqueue Service Bus messages per definition, and log progress.
- [ ] T025 [US2] Build Azure Service Bus worker pipeline in InquirySpark.WebApi/BackgroundWorkers/ServiceBusListeners.cs that resolves Chart.js configs, calls chartjs-node-canvas for PNG/SVG/PDF rendering, uploads blobs, and updates ChartAssetService.
- [ ] T026 [US2] Implement InquirySpark.WebApi/Controllers/ChartBuildsController.cs exposing POST /api/chart-builds, GET /api/chart-builds/{jobId}, and scheduling endpoints returning BaseResponse payloads.
- [ ] T027 [P] [US2] Implement InquirySpark.WebApi/Controllers/ChartAssetsController.cs with Cognitive Search queries, detail retrieval, and add-to-deck operations backed by ChartAssetService.
- [ ] T028 [US2] Create InquirySpark.Admin/Areas/Inquiry/Controllers/ChartAssetsController.cs and Razor views (Views/ChartAssets/Index.cshtml, _PreviewPanel.cshtml, _BatchActions.cshtml) rendering DataTables with export buttons.
- [ ] T029 [P] [US2] Add InquirySpark.Admin/wwwroot/src/js/chart-assets/library.ts to power search facets, asset previews, download toggles, and add-to-deck actions via API calls.
- [ ] T030 [US2] Implement build notifications in InquirySpark.Repository/Services/Notifications/BuildNotificationService.cs plus wiring in ChartBuildService to send email/in-app alerts when batches complete or fail.
- [ ] T063 [US2] Implement InquirySpark.Repository/Services/Analytics/ChartAssetUsageService.cs and supporting database objects to capture impressions, previews, downloads, shares, and add-to-deck events with Application Insights export.
- [ ] T064 [US2] Surface usage analytics in ChartAssetsController + Razor views (trend badges, usage columns, filters) and expose API endpoints for querying reuse metrics.

---

## Phase 5: User Story 3 - Data Validation & Export (Priority: P2)

**Goal**: Consultants inspect underlying datasets (up to 100K rows), apply filters under 500 ms, and export filtered rows to XLSX/PDF/CSV with metadata.

**Independent Test**: Launch data viewer for a chart, apply compound filters, confirm interactions stay below 500 ms, and export filtered rows to XLSX with metadata header.

### Implementation for User Story 3

- [ ] T031 [P] [US3] Implement InquirySpark.Repository/Services/DataExplorer/DataExplorerService.cs to build paged SQL queries against DatasetCatalog sources with server-side filtering, sorting, and summary stats.
- [ ] T032 [US3] Implement InquirySpark.Repository/Services/DataExplorer/DataExportService.cs to persist DataExportRequest records, enforce 50K row cap, and assemble export manifests.
- [ ] T033 [US3] Create InquirySpark.WebApi/Controllers/DataExplorerController.cs exposing GET /api/charts/{id}/data with DataTables-friendly payloads using ApiResponseHelper.
- [ ] T034 [P] [US3] Create InquirySpark.WebApi/Controllers/ExportsController.cs exposing POST /api/exports and GET /api/exports/{id} for download URLs bound to DataExportService.
- [ ] T035 [US3] Add Hangfire export worker InquirySpark.Repository/Jobs/DataExportJob.cs to render XLSX/PDF/CSV outputs, zip them when needed, and push assets to Azure Blob Storage.
- [ ] T036 [US3] Add InquirySpark.Admin/Areas/Inquiry/Controllers/DataExplorerController.cs with Razor views (Views/DataExplorer/Index.cshtml, _FilterBuilder.cshtml, _ExportModal.cshtml) hooking into new APIs.
- [ ] T037 [P] [US3] Implement InquirySpark.Admin/wwwroot/src/js/data-explorer/grid.ts to wire DataTables server-side mode, virtual scrolling, saved filter sets, and export triggers.
- [ ] T038 [US3] Extend InquirySpark.Admin/wwwroot/js/site.js to register `.datatable-export` options for the data explorer grid (custom page lengths, summary banner) and reuse JSZip/PDFMake bundles.
- [ ] T065 [US3] Enforce read-only scopes in DataExplorerService/DataExplorerController by rejecting mutation verbs, watermarking exports, and emitting audit entries for denied actions.
- [ ] T066 [US3] Persist data explorer preferences (column layouts, saved filter sets, page lengths) via UserPreferenceService and hydrate them in the Admin UI on load.

---

## Phase 6: User Story 4 - Chart Browsing & Deck Assembly (Priority: P2)

**Goal**: Consultants browse the global asset library with categories/tags/search, preview charts, and stage selected assets into deck projects exportable to PPTX/Google Slides/PDF/ZIP.

**Independent Test**: Index 10K chart assets, search by tag and creator, add selections to a deck, and export a PPTX honoring ordering and template rules.

### Implementation for User Story 4

- [ ] T039 [P] [US4] Implement InquirySpark.Repository/Services/Search/ChartAssetIndexService.cs to push asset metadata to Azure Cognitive Search with tag/creator facets and usage analytics.
- [ ] T040 [US4] Implement InquirySpark.Repository/Services/Decks/DeckProjectService.cs with CRUD, drag-order persistence, and add-to-deck APIs referencing ChartAssetService.
- [ ] T041 [US4] Build InquirySpark.WebApi/Controllers/DecksController.cs for deck CRUD plus POST /api/decks/{id}/export endpoints tied to DeckProjectService.
- [ ] T042 [US4] Create InquirySpark.Admin/Areas/Inquiry/Controllers/DecksController.cs and Razor views (Views/Decks/Index.cshtml, _DeckBuilder.cshtml, _SlidePreview.cshtml) following Admin DataTables patterns.
- [ ] T043 [P] [US4] Develop InquirySpark.Admin/wwwroot/src/js/decks/deck-builder.ts handling search results drag-and-drop, slide ordering, and export requests with progress indicators.
- [ ] T044 [US4] Implement deck export worker InquirySpark.Repository/Jobs/DeckExportJob.cs leveraging OpenXML SDK (PPTX), Google Slides API client, PDF rendering, and ZIP packaging for image bundles.
- [ ] T067 [US4] Persist deck drafts, slide ordering, and export presets through UserPreferenceService so consultants can resume work-in-progress decks across sessions.
- [ ] T068 [US4] Emit deck reuse telemetry (assets added from library vs. custom uploads) and expose reporting endpoints/UI badges to measure SC-004 progress.

---

## Phase 7: User Story 5 - Gauge Dashboards with Drill-Down (Priority: P3)

**Goal**: Executives load configurable gauge dashboards (30+ tiles) and drill into metric hierarchies down to question-level heatmaps while retaining filters.

**Independent Test**: Configure a dashboard template with 30 gauges, load on desktop/tablet, drill dimension→metric→question detail, and confirm <1s response per navigation.

### Implementation for User Story 5

- [ ] T045 [US5] Implement InquirySpark.Repository/Services/Dashboards/DashboardService.cs to load DashboardDefinition, GaugeTile layouts, and filter-resolved MetricScoreSnapshot data with cache hints.
- [ ] T046 [US5] Create InquirySpark.Repository/Services/Dashboards/MetricScoreService.cs plus supporting queries to refresh MetricScoreSnapshot nightly and assemble drill-down tables.
- [ ] T047 [US5] Implement InquirySpark.WebApi/Controllers/DashboardsController.cs exposing GET /api/dashboards/{id} and POST /api/dashboards/{id}/refresh endpoints returning dashboard payloads.
- [ ] T048 [US5] Add InquirySpark.Admin/Areas/Inquiry/Controllers/DashboardsController.cs with Razor views (Views/Dashboards/Index.cshtml, _GaugeTile.cshtml, _DrilldownTable.cshtml, _Breadcrumb.cshtml) for filtering + drill-down UX.
- [ ] T049 [P] [US5] Build InquirySpark.Admin/wwwroot/src/js/dashboards/gauges.ts and drilldown.ts to render Chart.js gauge plugins, trend indicators, breadcrumbs, and filter persistence.
- [ ] T050 [US5] Schedule MetricScoreSnapshot refresh worker in InquirySpark.WebApi/BackgroundWorkers/MetricSnapshotRefresher.cs via Hangfire recurring job, ensuring metrics stay <500 ms to retrieve.
- [ ] T069 [US5] Persist dashboard filter sets, tile layouts, and drill-down breadcrumbs via UserPreferenceService and reload them when executives return to the dashboard.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Documentation, automation, and observability work that spans multiple user stories.

- [ ] T051 Update docs/copilot/session-2025-12-14/quickstart-updates.md with new appsettings keys, Hangfire dashboard URLs, Cognitive Search index commands, and Blob container guidance referenced in quickstart.md.
- [ ] T052 Extend eng/BuildVerification.ps1 to run charting-specific `dotnet test` filters, npm build for InquirySpark.Admin, and smoke API calls against /api/chart-definitions and /api/chart-builds.
- [ ] T053 Instrument InquirySpark.WebApi/Program.cs and appsettings.* with OpenTelemetry + logging scopes for chart builder, batch jobs, data explorer, deck exports, and dashboards so performance targets can be monitored.
- [ ] T070 Add analytics events/endpoints to capture chart builder completion funnel, chart reuse ratios, dashboard satisfaction surveys, and nightly KPI aggregation dashboards aligned with SC-001/SC-004/SC-006.

---

## Dependencies & Execution Order

1. **Phase 1 → Phase 2**: Documentation/config scaffolding precedes schema and DI work.
2. **Phase 2 → User Stories**: Database + DTO layers must exist before any story work; they are hard blockers for US1–US5.
3. **User Story Order**: US1 (MVP) → US2 (batch/asset library) → US3 (data explorer) → US4 (browsing/decks) → US5 (dashboards). Later stories depend on assets, exports, or deck metadata produced earlier, though they can begin once foundational APIs exist.
4. **Polish** waits until desired user stories are code-complete.

## Parallel Execution Opportunities

- Tasks marked [P] can run concurrently (distinct files, no blocking dependencies).
- After Phase 2 completes, US1–US5 can proceed in parallel across different teams if shared assets are mocked.
- Front-end TypeScript work (e.g., filters.ts, library.ts, deck-builder.ts, gauges.ts) can run alongside backend controller/service implementation once contracts are stable.
- Batch infrastructure (Hangfire + Service Bus) can progress while Admin UI views are developed, provided DTOs are finalized.

## Implementation Strategy

1. Deliver MVP by completing Phases 1–3 (Setup, Foundational, User Story 1) to unlock reusable chart definitions and previews.
2. Layer automation (US2) to meet SLA requirements for chart regeneration and asset surfacing.
3. Add data explorer (US3) to build trust, then browsing/decks (US4) for reuse, and finally dashboards (US5) for executive views.
4. Use recurring Hangfire jobs and Service Bus workers for long-running processes; keep Admin UI responsive through async APIs.
5. Finish with polish to document operations, automate verification, and instrument performance counters.
