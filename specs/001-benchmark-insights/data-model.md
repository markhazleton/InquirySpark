# Data Model — Benchmark Insights & Reporting Platform
**Date:** December 14, 2025  
**Source Spec:** [spec.md](spec.md)

## Entity Overview

| Entity | Purpose | Key Fields | Notes |
|--------|---------|------------|-------|
| `DatasetCatalog` | Represents each reusable dataset or view exposed to the chart builder. | `DatasetId (int)`, `Name`, `Description`, `SourceType (enum: SqlTable, SqlView, Api)`, `SourceRef`, `RowCount`, `LastRefreshDt`, `OwnerId`, `ActiveFl`. | Populated from InquirySparkContext metadata; filtered by RBAC.
| `ChartDefinition` | Canonical definition that analysts edit. | `ChartDefinitionId`, `DatasetId`, `Name`, `Description`, `Tags (nvarchar)`, `FilterPayload (json)`, `VisualPayload (json)`, `CalculationPayload (json)`, `VersionNumber`, `AutoApprovedFl`, `CreatedById`, `CreatedDt`, `ModifiedById`, `ModifiedDt`, `IsArchivedFl`. | `FilterPayload` stores AND/OR tree. `VisualPayload` matches Chart.js schema. `CalculationPayload` stores expression metadata.
| `ChartVersion` | Immutable snapshot for history/rollback. | `ChartVersionId`, `ChartDefinitionId`, `VersionNumber`, `SnapshotPayload (json)`, `ApprovedFl`, `ApprovedById`, `ApprovedDt`, `DiffSummary`, `RollbackSourceVersionNumber`. | Created on each save; `SnapshotPayload` includes dataset schema hash for compatibility checks.
| `ChartBuildJob` | Batch execution record (parent). | `ChartBuildJobId`, `TriggerType (Manual, Schedule, Webhook)`, `RequestedById`, `RequestedDt`, `Status (Pending, Running, Completed, Failed, Cancelled)`, `StartedDt`, `CompletedDt`, `SuccessCount`, `FailureCount`, `SummaryLog`. | Schedules per-definition tasks via Hangfire.
| `ChartBuildTask` | Child per-definition execution. | `ChartBuildTaskId`, `ChartBuildJobId`, `ChartDefinitionId`, `ChartVersionId`, `Priority`, `Status`, `StartedDt`, `CompletedDt`, `ErrorPayload`. | Enables retries + status UI.
| `ChartAsset` | Metadata for rendered chart versions. | `ChartAssetId`, `ChartDefinitionId`, `ChartVersionId`, `DisplayName`, `Description`, `Tags`, `GenerationDt`, `DataSnapshotDt`, `ApprovalStatus (Draft, Approved, Archived)`, `UsageCount`, `LastAccessedDt`, `CdnBaseUrl`, `CommentsJson`. | Links to one or more files.
| `ChartAssetFile` | Format-specific artifact. | `ChartAssetFileId`, `ChartAssetId`, `Format (Png, Svg, Pdf, Jpeg, Thumbnail, Zip)`, `ResolutionHint`, `BlobPath`, `FileSizeBytes`, `Checksum`, `ExpiresDt`. | Retention policies operate at this level.
| `DeckProject` | Working collection of chart assets. | `DeckProjectId`, `Name`, `Description`, `OwnerId`, `Status (Draft, Published)`, `Theme`, `CreatedDt`, `ModifiedDt`. | References multiple `DeckSlide` records.
| `DeckSlide` | Ordered entry inside a deck. | `DeckSlideId`, `DeckProjectId`, `ChartAssetId`, `SortOrder`, `SlideTitle`, `SlideNotes`, `ExportOptionsJson`. | `SortOrder` supports drag-and-drop reordering.
| `DashboardDefinition` | High-level gauge dashboard template. | `DashboardDefinitionId`, `Name`, `Description`, `Slug`, `DefaultFiltersJson`, `LayoutJson`, `OwnerId`, `CreatedDt`, `ModifiedDt`, `PublishedFl`. | Multi-tenant friendly via slug.
| `GaugeTile` | Individual gauge bound to metric group. | `GaugeTileId`, `DashboardDefinitionId`, `MetricNodeId`, `TileType (Circular, Linear, Numeric)`, `ThresholdsJson`, `DrillTargetUrl`, `Size (Small, Medium, Large)`, `ColorPalette`, `TrendSource`, `LastRenderedDt`. | `ThresholdsJson` stores ranges + colors.
| `MetricGroup` | Logical grouping of metrics. | `MetricGroupId`, `Name`, `Description`, `ParentMetricGroupId (nullable)`, `CalculationType (Average, WeightedAverage, Sum, CustomExpression)`, `Weight`, `QuestionSetRef`, `BenchmarkTarget`, `DisplayOrder`. | Supports up to 5 hierarchy levels.
| `MetricScoreSnapshot` | Cached gauge values for dashboards. | `MetricScoreSnapshotId`, `MetricGroupId`, `SnapshotDt`, `FilterHash`, `ScoreValue`, `TargetValue`, `SampleSize`, `TrendDelta`, `DataVersionId`. | Pre-computed nightly to keep dashboards <500 ms per tile.
| `DataExportRequest` | Tracks long-running data exports. | `DataExportRequestId`, `ChartDefinitionId`, `RequestedById`, `RequestedDt`, `FilterPayload`, `ColumnSettingsJson`, `Format (Xlsx, Pdf, Csv, Tsv)`, `RowCount`, `Status`, `CompletionDt`, `BlobPath`. | Enables async downloads + rate limiting.

## Relationships

- `DatasetCatalog 1 - * ChartDefinition`
- `ChartDefinition 1 - * ChartVersion`
- `ChartVersion 1 - * ChartAsset` (non-enforced but typical one-to-many)
- `ChartAsset 1 - * ChartAssetFile`
- `ChartBuildJob 1 - * ChartBuildTask`
- `ChartDefinition 1 - * ChartBuildTask`
- `DeckProject 1 - * DeckSlide`
- `ChartAsset 1 - * DeckSlide`
- `DashboardDefinition 1 - * GaugeTile`
- `MetricGroup (self-referencing)` allows parent/child relationships; `GaugeTile` references `MetricNodeId` (alias for `MetricGroupId` leaf nodes).
- `MetricGroup 1 - * MetricScoreSnapshot`

## State Machines & Rules

### ChartDefinition Lifecycle
```
Draft (default) --auto-approval--> Approved --manual action--> Archived
Draft --delete--> (soft delete w/ Archived flag)
Approved --rollback--> Draft clone (new version number)
```
- Auto-approval occurs on save when validation passes. Audit trail logs `ApprovedById = CreatedById` by default.
- Archival prevents new chart builds but keeps history.

### ChartBuildJob States
```
Pending -> Running -> (Completed | Failed | Cancelled)
Running --partial failure--> Completed (FailureCount > 0, exposes retry)
```
- Each `ChartBuildTask` can be retried independently (Hangfire job id stored on record).

### Dashboard Tile Refresh
- Tiles refresh asynchronously when filters change. `GaugeTile.LastRenderedDt` tracks caching; a refresh request triggers recalculation of affected `MetricScoreSnapshot` rows.

## Validation Rules

- `FilterPayload` schemas validated against dataset columns using `Newtonsoft.Json.Schema` at save time.
- Calculated fields (basic math/ratios) validated so referenced columns exist and numeric data types align with expression requirements.
- Deck slides enforce uniqueness per `(DeckProjectId, ChartAssetId, SlideTitle)` to avoid duplicates unless intentionally duplicated with distinct notes.
- `MetricGroup` trees limited to depth 5; database constraint ensures no circular references.
- `DataExportRequest` enforces 50K row hard limit; requests beyond threshold are split into batches before job creation.

## Derived Data & Indexing

- SQL indexes on `ChartAsset (ChartDefinitionId, GenerationDt DESC)` and `ChartAsset (Tags)` to speed audit queries.
- `ChartBuildTask` indexed by `(Status, Priority)` for Hangfire dashboards.
- `MetricScoreSnapshot` has composite (`MetricGroupId`, `SnapshotDt`, `FilterHash`).
- Change-tracking triggers push metadata to Azure Cognitive Search and update Blob retention policies.
