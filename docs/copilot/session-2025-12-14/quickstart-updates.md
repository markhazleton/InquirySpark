# Benchmark Insights Quickstart Updates

## Configuration Keys

### InquirySpark.Admin appsettings.json

```json
{
  "ConnectionStrings": {
    "InquirySparkContext": "Server=localhost;Database=InquirySpark;...",
    "ControlSparkUserContextConnection": "Data Source=data/sqlite/users.db",
    "Hangfire": "Server=localhost;Database=InquirySparkJobs;..."
  },
  "Azure": {
    "StorageAccount": {
      "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
      "ChartAssetsContainer": "chart-assets",
      "ExportsContainer": "data-exports",
      "DeckPackagesContainer": "deck-packages"
    },
    "ServiceBus": {
      "ConnectionString": "Endpoint=sb://...",
      "ChartRenderQueue": "chart-render-queue"
    },
    "CognitiveSearch": {
      "ServiceName": "inquiryspark-search",
      "ApiKey": "...",
      "IndexName": "chart-assets"
    }
  },
  "ChartBuilder": {
    "DefaultBackgroundColor": "#FFFFFF",
    "MaxDimensionsPerChart": 10,
    "PreviewTimeoutSeconds": 30
  },
  "DataExplorer": {
    "MaxRowsPerPage": 100000,
    "MaxExportRows": 50000,
    "ExportRetentionDays": 7
  }
}
```

## Hangfire Dashboard Access

After running `InquirySpark.Admin`, navigate to:
- Local: https://localhost:7001/hangfire
- Production: https://your-domain.com/hangfire

**Authentication**: Requires `Executive` policy (admin users only)

**Key Recurring Jobs**:
- `chart-asset-indexer` - Runs every 15 minutes to sync approved charts
- `data-export-cleanup` - Daily at 1 AM to delete exports older than 7 days
- `metric-snapshot-refresh` - Daily at 2 AM to precompute gauge metrics

## Azure Cognitive Search Setup

### Create Index
```bash
# Install Azure CLI if not present
az login

# Create search service (Standard S1 tier recommended)
az search service create \
  --name inquiryspark-search \
  --resource-group InquirySparkRG \
  --sku Standard

# Get admin API key
az search admin-key show \
  --resource-group InquirySparkRG \
  --service-name inquiryspark-search
```

### Index Schema
```json
{
  "name": "chart-assets",
  "fields": [
    {"name": "chartAssetId", "type": "Edm.String", "key": true},
    {"name": "chartNm", "type": "Edm.String", "searchable": true},
    {"name": "chartDefinitionNm", "type": "Edm.String", "filterable": true},
    {"name": "tags", "type": "Collection(Edm.String)", "filterable": true, "facetable": true},
    {"name": "creatorId", "type": "Edm.String", "filterable": true},
    {"name": "approvalStatusCd", "type": "Edm.String", "filterable": true},
    {"name": "createdDt", "type": "Edm.DateTimeOffset", "filterable": true, "sortable": true}
  ]
}
```

### Trigger Initial Indexing
```powershell
# Run from InquirySpark.Admin project root
dotnet run --urls="https://localhost:7001"

# In separate terminal, trigger indexing
Invoke-RestMethod -Method POST -Uri "https://localhost:7001/api/ChartAssets/index-all"
```

## Azure Blob Storage Containers

### Create Containers
```bash
# Create storage account (if not exists)
az storage account create \
  --name inquirysparkblob \
  --resource-group InquirySparkRG \
  --location eastus \
  --sku Standard_LRS

# Get connection string
az storage account show-connection-string \
  --name inquirysparkblob \
  --resource-group InquirySparkRG

# Create containers
az storage container create --name chart-assets --account-name inquirysparkblob
az storage container create --name data-exports --account-name inquirysparkblob
az storage container create --name deck-packages --account-name inquirysparkblob
```

### Container Purposes
- **chart-assets**: PNG images from chart renderer (1-year retention)
- **data-exports**: XLSX/PDF/CSV exports (7-day retention, auto-cleanup)
- **deck-packages**: PPTX/PDF/ZIP deck bundles (30-day retention)

## Service Bus Setup

### Create Queue
```bash
# Create Service Bus namespace
az servicebus namespace create \
  --name inquiryspark-sb \
  --resource-group InquirySparkRG

# Create queue for chart rendering
az servicebus queue create \
  --name chart-render-queue \
  --namespace-name inquiryspark-sb \
  --resource-group InquirySparkRG \
  --max-delivery-count 3 \
  --lock-duration PT5M
```

## Verification Commands

### Test API Endpoints
```powershell
# Chart definitions
Invoke-RestMethod -Uri "https://localhost:7001/api/ChartDefinitions"

# Chart builds (recent)
Invoke-RestMethod -Uri "https://localhost:7001/api/ChartBuilds?limit=10"

# Data explorer (requires chartId)
Invoke-RestMethod -Method POST -Uri "https://localhost:7001/api/DataExplorer/charts/1/data" `
  -ContentType "application/json" `
  -Body '{"page":1,"pageSize":25}'

# Gauge dashboards
Invoke-RestMethod -Uri "https://localhost:7001/api/GaugeDashboards"
```

### Build & Test
```powershell
# From repo root
eng/BuildVerification.ps1

# Expected output:
# ✓ Solution builds successfully
# ✓ Tests pass (XX passed, 0 failed)
# ✓ npm build completes
# ✓ API smoke tests pass
```

## Troubleshooting

### Hangfire Dashboard 404
- Ensure `HangfireAuthorizationFilter` is registered in Program.cs
- Verify connection string points to valid SQL Server database
- Check `Executive` policy exists and user has required claims

### Azure Search Not Returning Results
- Run index-all command to populate index
- Verify API key has admin permissions
- Check firewall rules allow inbound connections

### Chart Rendering Timeout
- Increase `ChartBuilder:PreviewTimeoutSeconds` in appsettings
- Check Service Bus queue for dead-letter messages
- Review ChartRenderWorker logs for exceptions

### Export Files Not Appearing
- Verify Blob Storage connection string is correct
- Check container names match appsettings configuration
- Ensure Hangfire `DataExportJob` is executing (check /hangfire dashboard)

## Performance Targets

- **SC-001**: Chart preview < 3 seconds → Monitor `ChartBuildsController` telemetry
- **SC-006**: Dashboard navigation < 1 second → `MetricSnapshotRefresher` ensures <500ms queries
- **SC-004**: 80% library reuse → Track via `/api/DeckTelemetry/reuse-badge` endpoint
