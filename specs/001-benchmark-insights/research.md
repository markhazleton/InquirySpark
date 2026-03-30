# Phase 0 Research — Benchmark Insights & Reporting Platform
**Date:** December 14, 2025  
**Source Spec:** [spec.md](spec.md)

Each decision resolves a previously open item from the Technical Context or outlines best practices for critical dependencies.

## Decision 1: Preserve InquirySpark Response Wrapper Pipeline
- **Decision:** All new charting, asset, browsing, and dashboard services will live in `InquirySpark.Repository` and expose `BaseResponse<T>` / `BaseResponseCollection<T>` via `DbContextHelper.ExecuteAsync` wrappers. WebApi controllers will only orchestrate calls and translate to HTTP through `ApiResponseHelper`.
- **Rationale:** Keeps parity with Constitution Principle I, ensures consistent telemetry + error handling, and allows Admin/Web apps plus third parties to consume the same APIs without duplication.
- **Alternatives considered:** Direct EF usage inside controllers (rejected—violates constitution and complicates reuse); bespoke DTO-only service layer (rejected—breaks existing helper infrastructure).

## Decision 2: Hangfire + Azure Service Bus for Batch Generation
- **Decision:** Use Hangfire Server hosted inside `InquirySpark.WebApi` (or dedicated worker) with SQLite storage for job metadata and Azure Service Bus queues for fan-out of per-chart rendering tasks.
- **Rationale:** Hangfire provides retries, dashboards, delayed/scheduled execution, and integrates cleanly with ASP.NET Core DI. Azure Service Bus topics let us scale out worker instances for heavy rendering without blocking the main web host.
- **Alternatives considered:** Quartz.NET with SQL triggers (rejected—less tooling around retries/monitoring); Azure Functions Timer jobs (rejected—would require new deployment stack and complicate on-prem installs).

## Decision 3: Azure Blob Storage + CDN for Chart Assets
- **Decision:** Persist rendered assets and exported bundles in Azure Blob Storage using hierarchical paths `/charts/{year}/{month}/{definitionId}/{version}/`. Expose them through Azure CDN endpoints stored in the asset metadata table.
- **Rationale:** Meets spec requirements for multiple formats, retention policies, and CDN-friendly URLs. Azure SDKs integrate with .NET 10 and allow server-side encryption + lifecycle policies for archival.
- **Alternatives considered:** File system storage (rejected—fails multi-instance + CDN goals); AWS S3 (rejected—team already standardized on Azure per infrastructure notes).

## Decision 4: Azure Cognitive Search for Library Discovery
- **Decision:** Push chart metadata (name, tags, description, creator, approval status, usage stats) to an Azure Cognitive Search index `chart-assets-index`, enabling boolean filters, fuzzy search, highlight snippets, and aggregations for facets.
- **Rationale:** Spec demands AND/OR filters, saved searches, and sub-second response times over 100K assets—features that exceed what SQLite full-text search or LIKE queries can reliably supply.
- **Alternatives considered:** SQLite full-text search (rejected—limited fuzziness and facet support); Elasticsearch self-hosted (rejected—higher ops burden than managed Azure service for this stack).

## Decision 5: Chart.js 4 + Reusable Config Serializers
- **Decision:** Use Chart.js 4 (already compatible with Bootstrap/Bootswatch theming) for live previews and server-side rendering via `chartjs-node-canvas` hosted in a .NET worker through NodeServices. Store visual configurations as JSON matching Chart.js schema, enabling preview + batch rendering parity.
- **Rationale:** Chart.js provides all required chart types (bar/line/area/pie/donut/scatter/gauge) and a rich plugin ecosystem. Serializing Chart.js configs avoids impedance mismatch between UI and batch renderer.
- **Alternatives considered:** D3.js custom renderers (rejected—longer build time per chart, higher skill requirement); Plotly (rejected—license implications for server-side generation at scale).

## Decision 6: DataTables 2.3.5 with Server-Side Paging for Data Explorer
- **Decision:** Extend the existing DataTables integration to use server-side processing for datasets >10K rows, powered by new WebApi endpoints that emit paged JSON along with column metadata and summary statistics. Reuse the `.datatable-export` pattern for export triggers.
- **Rationale:** Leverages the established Admin UI conventions and avoids introducing another grid library. Server-side mode keeps filtering and sorting under 500 ms even with 100K rows.
- **Alternatives considered:** AG Grid Enterprise (rejected—new license + bundle size); TanStack Table (rejected—would require a React/Vite rewrite of the Admin UI).
