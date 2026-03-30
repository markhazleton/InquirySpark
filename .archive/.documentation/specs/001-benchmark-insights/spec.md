# Feature Specification: Benchmark Insights & Reporting Platform

**Feature Branch**: `[001-benchmark-insights]`  
**Created**: December 7, 2025  
**Status**: Draft  
**Input**: User description: "Update InquirySpark with an API layer and new UI that implements the Benchmark Insights & Reporting Platform plan (chart builder, automated asset library, data exploration, browsing, and gauge dashboards) while reusing the existing database."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Reusable Chart Definitions (Priority: P1)

Analysts configure a chart once by selecting datasets, defining filters, and styling visuals so they can regenerate consistent benchmark graphics for any engagement without custom development.

**Why this priority**: Without reusable chart definitions, analysts remain dependent on engineers, preventing the 60% time-to-insight reduction goal.

**Independent Test**: Provision a dataset, configure a chart, save it, and verify it can be re-opened and previewed with a different data slice without developer intervention.

**Acceptance Scenarios**:

1. **Given** an authenticated analyst with access to benchmark datasets, **When** they select a dataset, apply filters, map fields, and choose a chart type, **Then** the live preview renders within 1 second and reflects the selected filters.
2. **Given** a saved chart configuration with version history, **When** another analyst loads the definition and clicks Save As, **Then** a new version is recorded with optimistic locking so no edits are lost.

---

### User Story 2 - Automated Chart Rebuilds & Asset Library (Priority: P1)

Operational users trigger or schedule a batch job that regenerates every approved chart definition, stores each image rendition, and surfaces progress, errors, and downloadable assets.

**Why this priority**: Automated builds are required to hit the <2 hour ingestion-to-presentation SLA and reduce developer effort by 80%.

**Independent Test**: Execute a manual build against sample definitions, observe job telemetry, and verify resulting assets (PNG/SVG/PDF) are cataloged with metadata and version tags.

**Acceptance Scenarios**:

1. **Given** 50 active chart definitions, **When** an operator clicks Build All Charts, **Then** the system queues each definition, renders all four target resolutions, and reports completion within 3 minutes (20 charts/minute throughput) while logging failures.
2. **Given** a scheduled nightly build, **When** one definition fails because the source dataset is unavailable, **Then** the batch retries up to 3 times, records diagnostics, skips to the next definition, and surfaces the error with a retry action.

---

### User Story 3 - Data Validation & Export (Priority: P2)

Consultants open the underlying dataset for any chart, interactively filter/sort up to 100K rows, and export the view (Excel, PDF, CSV) to share validated benchmark evidence with clients.

**Why this priority**: Trust in the insights depends on transparent access to source data, and exports are required for custom client calculations.

**Independent Test**: Launch the data viewer for a chart, apply compound filters, confirm the preview stays under 500 ms per action, and export the filtered rows to XLSX with metadata.

**Acceptance Scenarios**:

1. **Given** a chart driven by a 90K-row dataset, **When** a consultant applies column filters and switches pagination to 250 rows, **Then** the grid virtual scrolls at 60 fps and shows accurate row counts.
2. **Given** a filtered grid selection, **When** the user exports to PDF with landscape orientation, **Then** the document includes the applied filters, summary statistics, and is generated in under 10 seconds.

---

### User Story 4 - Chart Browsing & Deck Assembly (Priority: P2)

Consultants browse the global asset library using categories, tags, and full-text search, preview charts, and stage selected assets into deck projects that can be exported to PowerPoint/Google Slides.

**Why this priority**: Reuse of chart assets (>70% target) requires a rich discovery experience and streamlined deck creation across engagements.

**Independent Test**: Index 10K chart assets, search by tag plus creator filter, open detail previews, add items to a deck, and export a PPTX that honors ordering and template rules.

**Acceptance Scenarios**:

1. **Given** the asset library contains tagged charts, **When** a user filters by industry tag, creator, and approval status, **Then** results update within 500 ms and show accurate hit counts.
2. **Given** a deck project with 12 charts, **When** the user exports to PowerPoint, **Then** each slide contains the selected chart, title, description, and slide notes with source data links.

---

### User Story 5 - Gauge Dashboards with Drill-Down (Priority: P3)

Executives load configurable gauge dashboards that summarize dimensional scores, then drill through metric hierarchies down to question-level heatmaps while retaining applied filters.

**Why this priority**: High-level dashboards communicate performance at a glance and support the benchmarking value proposition for leadership personas.

**Independent Test**: Configure a dashboard template with 30 gauges, load it on desktop and tablet, drill from dimension to question detail, and confirm response times stay within 1 second per navigation.

**Acceptance Scenarios**:

1. **Given** a dashboard configured with departmental filters, **When** a user switches from region A to region B, **Then** every gauge re-renders in under 500 ms and updates the last-refreshed timestamp.
2. **Given** a gauge linked to a drill-down path, **When** the executive clicks the tile, **Then** they see a breadcrumb-enabled view of sub-metrics plus a table of underlying questions, with an option to export the detailed data.

---

### Edge Cases

- Source dataset metadata is missing or outdated; the UI must flag the issue and block saving until a valid dataset is selected.
- Filters resolve to zero rows (e.g., conflicting tags); the preview must warn the analyst and allow quick filter removal without clearing the entire configuration.
- Concurrent edits on the same chart definition; optimistic locking needs to prevent silent overwrites and prompt the user to merge changes.
- Batch generation encounters storage quota limits; processing should pause gracefully, surface the error, and avoid partial metadata writes.
- Export jobs that exceed maximum row or file-size thresholds must chunk outputs and warn the user before proceeding.
- Dashboard drill-down path references metrics that were deleted; the system must display a fallback message and log the configuration gap.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The Admin UI MUST provide a dataset catalog sourced from the existing InquirySpark database and API endpoints that expose dataset metadata (row counts, last refresh, owner) for chart configuration.
- **FR-002**: The chart builder MUST support multi-select filters (search, hierarchy browser, AND/OR logic, tag filters, bulk select) with state persistence when switching tabs.
- **FR-003**: The live preview pane MUST re-render within 1 second after users adjust filters, field mappings, or visual properties, using debounced updates (300 ms) to avoid UI thrashing.
- **FR-004**: Saved chart definitions MUST capture identifiers, descriptions, dataset references, filter JSON, visual properties, creator, timestamps, and version numbers with optimistic locking enforced.
- **FR-005**: The platform MUST expose RESTful endpoints that allow programmatic CRUD operations for chart definitions, batch jobs, assets, deck projects, and dashboards so the new UI and external systems share the same API.
- **FR-006**: Batch processing MUST queue every active chart definition, apply stored filters, render PNG/SVG/PDF/JPEG outputs at all required resolutions, embed metadata, and update processing logs with retries and circuit breakers.
- **FR-007**: Generated assets MUST be stored in hierarchical folders (year/month/definition/version) with CDN-ready URLs, version retention (last N versions), and archival policies linked to approval status.
- **FR-008**: The asset library UI MUST provide grid/list modes, full-text search with boolean operators, tag/category filters, usage analytics, and quick actions (view, download, add to deck, share, regenerate, delete). Usage analytics MUST capture per-asset impressions, preview opens, downloads, add-to-deck events, and share actions so reuse ratios can be reported and audited.
- **FR-009**: Deck assembly MUST allow multi-select chart staging, drag-and-drop slide ordering, persistent deck drafts, and export options for PPTX, Google Slides, PDF, and ZIP image bundles with corporate theming applied.
- **FR-010**: The data explorer MUST render up to 100K rows with virtual scrolling, column operations (resize, reorder, hide), compound filters, saved filter sets, summary statistics, and export options (XLSX, PDF, CSV/TSV/custom delimiter) that honor visible columns and filters.
- **FR-011**: Dashboard gauge tiles MUST support configurable thresholds, color palettes, hover tooltips, drill-down targets, trend indicators, responsive layouts, and filter inheritance across up to five hierarchy levels.
- **FR-012**: Security MUST enforce role-based access control so only authorized users can edit chart definitions, run batch jobs, view restricted datasets, or access client-specific dashboards, with all actions audit logged. Roles include Analyst, Operator, Consultant, and Executive, and every controller/service call MUST evaluate policy-based requirements plus write structured audit log entries (actor, action, entity, timestamp, outcome, metadata).
- **FR-013**: Notifications MUST alert operators when builds finish or fail (email and in-app) and include summary statistics plus links to retry or inspect logs.
- **FR-014**: The system MUST persist user preferences for view modes, column layouts, deck drafts, and dashboard filters so analysts resume their last context. Preferences MUST be stored per-user as versioned JSON payloads with schema validation, CRUD APIs, and UI integrations for the chart builder, data explorer, deck builder, and dashboards.
- **FR-015**: The platform MUST auto-approve chart definitions upon save so long as they meet validation rules, immediately surfacing assets in the browsing experience while logging approvals for audit. Validation MUST include dataset availability, filter schema compliance, calculation safety, and metadata completeness; approval state, approver identity, timestamps, and reason codes must be persisted and exposed via APIs/UI badges.
- **FR-016**: The chart builder MUST support basic calculated fields (sum, average, ratio, percentage change) configured via a guided formula editor so analysts can derive metrics without pre-modeled columns, including validation that calculated outputs remain compatible with the selected chart type. The editor MUST provide function pickers, column autocomplete, inline error messaging, and backend parsing that rejects unsafe expressions.
- **FR-017**: The data explorer MUST remain strictly view-only with no inline edits or annotations; exports must mirror the read-only data to ensure the platform never diverges from the system-of-record. APIs MUST enforce read-only scopes, watermark exports with "Source of Truth" metadata, and block mutation attempts with audited warnings.

### Key Entities

- **Dataset**: Represents a reusable data source (table, view, API) with metadata such as schema, refresh cadence, ownership, and allowed segments.
- **ChartDefinition**: Stores the reusable configuration for visualization (dataset bindings, filters, field mappings, styling, version info, approval state).
- **ChartAsset**: Materialized output of a chart definition at a point in time; holds file locations for each format/resolution plus metadata (snapshot date, usage counts, annotations).
- **BatchJob**: Tracks execution of automated chart generation, including queue state, per-definition logs, retries, and summary statistics.
- **DeckProject**: User-curated collection of chart assets with ordering, export settings, and collaboration metadata.
- **MetricGroup**: Hierarchical definition of gauges/dimensions referencing question pools, weights, calculation methods, and drill-down relationships.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 90% of new chart definitions can be configured, saved, and previewed within 15 minutes by an analyst without engineering support.
- **SC-002**: Automated batch generation sustains at least 20 charts per minute and completes 95% of nightly runs without manual intervention.
- **SC-003**: Data explorer interactions (filter, sort, paginate) complete in under 500 ms for datasets up to 100K rows, and exports up to 50K rows finish within 10 seconds.
- **SC-004**: Chart reuse rate reaches 70% (at least 7 of 10 charts used in a deliverable come from the asset library) within three months of launch.
- **SC-005**: Gauge dashboards load 50 tiles in under 3 seconds on broadband connections and maintain drill-down response times under 1 second per level.
- **SC-006**: Analyst satisfaction with the reporting workflow scores at least 4.5/5 in post-launch surveys, and developer involvement in standard reports drops by 80%.

### Telemetry & Measurement Requirements

- Instrumentation MUST emit analytics events aligned with SC-001 through SC-006 (e.g., chart builder completion funnel, batch throughput, data explorer latency samples, chart reuse counters, dashboard render timings, post-workflow surveys) so success criteria can be validated from production telemetry.
