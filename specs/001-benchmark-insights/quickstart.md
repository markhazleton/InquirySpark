# Quickstart — Benchmark Insights & Reporting Platform
**Date:** December 14, 2025  
**Audience:** InquirySpark engineers spinning up the Benchmark Insights feature branch (`001-benchmark-insights`).

## 1. Prerequisites
- .NET SDK 10.0 preview (includes C# 13)
- Node.js 20.x + npm 10.x (Admin webpack build)
- Azure CLI (for blob container + Cognitive Search provisioning)
- SQL Server 2022 local instance (for `InquirySparkContext` + Hangfire storage)
- Optional: Azure Storage Explorer for inspecting chart assets

## 2. Environment Setup
1. Clone and checkout branch:
   ```sh
   git clone https://github.com/MarkHazleton/InquirySpark.git
   cd InquirySpark
   git checkout 001-benchmark-insights
   ```
2. Restore tools:
   ```sh
   dotnet tool restore
   npm install --prefix InquirySpark.Admin
   ```
3. Copy sample environment files and update secrets:
   ```sh
   cp InquirySpark.Repository/appsettings.Development.json InquirySpark.Repository/appsettings.Local.json
   cp InquirySpark.WebApi/appsettings.Development.json InquirySpark.WebApi/appsettings.Local.json
   cp InquirySpark.Admin/appsettings.Development.json InquirySpark.Admin/appsettings.Local.json
   ```
   - Set `ConnectionStrings:InquirySparkContext` to your SQL Server.
   - Add `Storage:Charts:ConnectionString` + `ContainerName` for Azure Blob Storage.
   - Add `Search:ServiceName`, `Search:ApiKey` for Azure Cognitive Search.
   - Configure `Hangfire:DashboardAuth` secrets as needed.

## 3. Database & Storage
1. Apply schema updates (charting tables, deck tables, metric tables) via the Database project:
   ```sh
   dotnet build InquirySpark.Database/InquirySpark.Database.sqlproj /p:TargetDatabase=InquirySparkLocal
   ```
   or run the generated migration scripts in SSMS.
2. Seed reference data:
   ```sh
   dotnet run --project data/Seeders/InquirySpark.Seeder -- charts
   ```
3. Create Azure Blob containers:
   ```sh
   az storage container create --account-name <acct> --name charts --public-access blob
   ```
4. Provision Azure Cognitive Search index using the template JSON in `docs/copilot/session-2025-12-04/chart-assets-index.json` (to be added in Phase 2 tasks) or via portal.

## 4. Running the Stack
1. Start Repository + WebApi (includes Hangfire server + Service Bus listeners):
   ```sh
   dotnet run --project InquirySpark.WebApi
   ```
2. Start Admin UI (builds JS + serves chart builder pages):
   ```sh
   pushd InquirySpark.Admin
   npm run dev   # webpack watch mode
   dotnet run
   popd
   ```
3. Optional: Start Web app for public dashboard previews:
   ```sh
   dotnet run --project InquirySpark.Web
   ```

## 5. Background Jobs & Schedulers
- Hangfire Dashboard available at `/hangfire` on the API host (restricted to Admin roles).
- Seed a nightly recurring job via Swagger `POST /api/chart-builds/schedules` or Hangfire dashboard.
- Webhook trigger endpoint: `POST /api/chart-builds/webhook/{datasetKey}`.

## 6. Testing & Verification
1. Run unit tests (Common + Repository):
   ```sh
   dotnet test InquirySpark.sln
   ```
2. Smoke-test REST APIs via Swagger at https://localhost:5001/swagger.
3. UI checklist:
   - Create a chart definition, save, and confirm history timeline updates.
   - Trigger Build All -> monitor Hangfire dashboard for job completion.
   - Browse asset library; ensure search facets respond under 500 ms.
   - Launch data explorer from any chart and export filtered results to XLSX.
   - Open dashboard view and verify gauge refresh + drill-down path.

## 7. Troubleshooting
- **Hangfire jobs stuck in Scheduled:** verify Service Bus connection string and ensure worker host is running.
- **Asset previews broken:** confirm Blob container is publicly readable (or SAS tokens configured) and CDN endpoint is active.
- **Search requests failing:** re-run `az search admin-key show` and update `Search:ApiKey` in appsettings.
- **Charts not rendering in Admin:** run `npm run build` to ensure updated Chart.js bundles are available under `wwwroot/lib`.
