# Analytics & KPI Tracking Implementation Notes

## Analytics Events/Endpoints (T070)

### Chart Builder Completion Funnel
**Purpose**: Track user progression through chart creation workflow to identify drop-off points.

**Events to Capture**:
1. `chart_builder_started` - User opens chart builder
2. `chart_template_selected` - User selects template/definition
3. `chart_dimensions_configured` - User configures dimensions
4. `chart_preview_requested` - User clicks preview
5. `chart_preview_completed` - Preview renders successfully
6. `chart_preview_failed` - Preview fails (with error type)
7. `chart_saved` - User saves chart build
8. `chart_published` - Chart approved and published to library

**Metrics**:
- Conversion rate per step (funnel visualization)
- Average time per step
- Common exit points
- Error frequency by type

**Implementation**:
```csharp
// InquirySpark.Repository/Services/Analytics/ChartBuilderAnalyticsService.cs
public async Task TrackFunnelEventAsync(string userId, string eventName, int? chartBuildId = null)
{
    await _context.AnalyticsEvents.AddAsync(new AnalyticsEvent
    {
        UserId = userId,
        EventNm = eventName,
        ChartBuildId = chartBuildId,
        EventDt = DateTime.UtcNow
    });
    await _context.SaveChangesAsync();
}
```

### Chart Reuse Ratios (SC-004 Tracking)
**Purpose**: Monitor library vs custom chart usage to track 80% reuse target.

**Metrics**:
- Overall library reuse percentage (already implemented in DeckTelemetryService)
- Reuse rate by user role (Analyst vs Consultant vs Executive)
- Most reused charts (top 10 leaderboard)
- Custom upload frequency trends

**Endpoint**: `/api/Analytics/chart-reuse`
```json
{
  "overallReusePercentage": 82.5,
  "byRole": {
    "Analyst": 85.0,
    "Consultant": 78.0,
    "Executive": 90.0
  },
  "topReusedCharts": [
    {"chartDefinitionId": 15, "chartNm": "CSAT by Department", "usageCount": 234},
    {"chartDefinitionId": 8, "chartNm": "NPS Trend", "usageCount": 189}
  ],
  "trendData": [
    {"month": "2024-01", "reusePercentage": 75.0},
    {"month": "2024-02", "reusePercentage": 78.5}
  ]
}
```

### Dashboard Satisfaction Surveys
**Purpose**: Capture executive feedback on gauge dashboard usefulness.

**Implementation**:
- Add optional "Helpful?" thumbs up/down buttons to gauge dashboards
- Track satisfaction by dashboard type
- Capture qualitative feedback via optional text input

**Table Schema**:
```sql
CREATE TABLE DashboardFeedback (
    DashboardFeedbackId INT PRIMARY KEY,
    GaugeDashboardId INT NOT NULL,
    UserId NVARCHAR(50) NOT NULL,
    IsHelpfulFl BIT NOT NULL,
    FeedbackTx NVARCHAR(500) NULL,
    FeedbackDt DATETIME NOT NULL,
    FOREIGN KEY (GaugeDashboardId) REFERENCES GaugeDashboard(GaugeDashboardId)
);
```

**Endpoint**: `/api/Analytics/dashboard-satisfaction`
```json
{
  "satisfactionRate": 0.87,
  "totalFeedback": 145,
  "positiveCount": 126,
  "negativeCount": 19,
  "commonComplaints": [
    "Slow refresh times",
    "Missing drill-down to questions"
  ],
  "mostHelpfulDashboards": [
    {"dashboardNm": "Executive Summary", "satisfactionRate": 0.92},
    {"dashboardNm": "Department Metrics", "satisfactionRate": 0.89}
  ]
}
```

### Nightly KPI Aggregation Dashboard
**Purpose**: Surface key success criteria metrics aligned with SC-001/SC-004/SC-006.

**KPIs to Track**:
1. **SC-001**: Chart preview performance (target: <3s)
   - Average preview time (last 24h, 7d, 30d)
   - P95/P99 latency
   - Timeout rate

2. **SC-004**: Library reuse ratio (target: 80%)
   - Current reuse percentage
   - Trend (improving/declining)
   - Delta from target

3. **SC-006**: Dashboard navigation performance (target: <1s)
   - Average navigation time
   - Metric query response times (target: <500ms)
   - Slow query count

**Hangfire Recurring Job**:
```csharp
// InquirySpark.Repository/Jobs/KpiAggregationJob.cs
[AutomaticRetry(Attempts = 2)]
public async Task AggregateKpisAsync()
{
    // Aggregate SC-001: Chart preview times
    var previewStats = await _context.AnalyticsEvents
        .Where(e => e.EventNm == "chart_preview_completed" && e.EventDt >= DateTime.UtcNow.AddDays(-1))
        .Select(e => e.DurationMs)
        .ToListAsync();
    
    var avgPreviewTime = previewStats.Average();
    var p95PreviewTime = CalculatePercentile(previewStats, 0.95);
    
    // Aggregate SC-004: Library reuse
    var reuseStats = await _deckTelemetryService.GetGlobalStatsAsync();
    
    // Aggregate SC-006: Dashboard navigation
    var navStats = await _context.AnalyticsEvents
        .Where(e => e.EventNm == "dashboard_navigation" && e.EventDt >= DateTime.UtcNow.AddDays(-1))
        .Select(e => e.DurationMs)
        .ToListAsync();
    
    // Store aggregated KPIs
    await _context.KpiSnapshots.AddAsync(new KpiSnapshot
    {
        SnapshotDt = DateTime.UtcNow,
        AvgChartPreviewMs = avgPreviewTime,
        P95ChartPreviewMs = p95PreviewTime,
        LibraryReusePercentage = reuseStats.OverallReuseRate,
        AvgDashboardNavMs = navStats.Average(),
        CreatedDt = DateTime.UtcNow
    });
    
    await _context.SaveChangesAsync();
}

// Schedule: Run at 1 AM daily
RecurringJob.AddOrUpdate<KpiAggregationJob>(
    "kpi-aggregation",
    job => job.AggregateKpisAsync(),
    Cron.Daily(1));
```

**Dashboard Endpoint**: `/api/Analytics/kpis`
```json
{
  "asOfDt": "2024-12-14T01:00:00Z",
  "sc001_ChartPreview": {
    "target": 3000,
    "avgMs": 2150,
    "p95Ms": 2850,
    "p99Ms": 3200,
    "status": "warning",
    "trend": "improving"
  },
  "sc004_LibraryReuse": {
    "target": 80.0,
    "current": 82.5,
    "status": "success",
    "trend": "stable"
  },
  "sc006_DashboardNav": {
    "targetMs": 1000,
    "avgMs": 875,
    "slowQueriesCount": 12,
    "status": "success",
    "trend": "stable"
  }
}
```

## Performance Instrumentation

### Logging Scopes
All charting operations use structured logging with scopes for correlation:

```csharp
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["UserId"] = userId,
    ["ChartBuildId"] = chartBuildId,
    ["Operation"] = "ChartPreview"
}))
{
    _logger.LogInformation("Starting chart preview");
    // ... chart rendering logic
    _logger.LogInformation("Chart preview completed in {DurationMs}ms", duration);
}
```

### OpenTelemetry Integration (Future)
When enabled via appsettings, InquirySpark.Admin will emit:
- **Traces**: Chart builder workflow spans, batch job execution, API request traces
- **Metrics**: Chart builder completion rates, library reuse ratios, performance counters

Custom activity sources:
- `InquirySpark.ChartBuilder` - Chart definition/build operations
- `InquirySpark.BatchJobs` - Hangfire background workers
- `InquirySpark.DataExplorer` - Data explorer queries and exports
- `InquirySpark.DeckExports` - Deck assembly and PPTX generation
- `InquirySpark.GaugeDashboards` - Dashboard rendering and navigation

## Implementation Status

**Completed**:
- ✅ Logging scopes configured in Program.cs
- ✅ Performance targets defined in appsettings.json
- ✅ OpenTelemetry placeholder in Program.cs (disabled by default)
- ✅ DeckTelemetryService for SC-004 tracking

**TODO** (Future Enhancement):
- ⏳ Create AnalyticsEvent/KpiSnapshot tables
- ⏳ Implement ChartBuilderAnalyticsService for funnel tracking
- ⏳ Create DashboardFeedback table and satisfaction endpoints
- ⏳ Implement KpiAggregationJob for nightly aggregation
- ⏳ Build analytics dashboard UI for KPI visualization
- ⏳ Enable OpenTelemetry and configure OTLP exporter

## Success Criteria Alignment

- **SC-001** (Chart preview <3s): Tracked via chart_preview_completed event duration
- **SC-004** (80% library reuse): Tracked via DeckTelemetryService + KPI aggregation
- **SC-006** (Dashboard nav <1s): Tracked via dashboard_navigation event duration + metric query times
