# InquirySpark Development Guidelines

Auto-generated from all feature plans. Last updated: 2025-12-04

## Active Technologies
- C# 13 on .NET 10 (WebApi + Admin + Repository) + ASP.NET Core MVC/WebApi, EF Core 10, Hangfire 1.8, Azure Service Bus, Azure Blob Storage SDK, Azure Cognitive Search SDK, Chart.js 4 + chartjs-node-canvas, DataTables 2.3.5, JSZip/PDFMake for exports (001-benchmark-insights)
- SQL Server 2022 (`InquirySparkContext`) for OLTP + Hangfire metadata, Azure Blob Storage for rendered assets/exports, Azure Cognitive Search index for chart discovery, Azure Service Bus queues/topics for batch fan-out (001-benchmark-insights)

- C# 13 / .NET 10 (ASP.NET Core MVC, Razor Pages, Web API) + EF Core 10 (Sqlite provider), ASP.NET Core Identity, MSTest, WebSpark Bootswatch, DataTables, Microsoft.Extensions.Logging (001-remove-sql-server)

## Project Structure

```text
backend/
frontend/
tests/
```

## Commands

# Add commands for C# 13 / .NET 10 (ASP.NET Core MVC, Razor Pages, Web API)

## Code Style

C# 13 / .NET 10 (ASP.NET Core MVC, Razor Pages, Web API): Follow standard conventions

## Recent Changes
- 001-benchmark-insights: Added C# 13 on .NET 10 (WebApi + Admin + Repository) + ASP.NET Core MVC/WebApi, EF Core 10, Hangfire 1.8, Azure Service Bus, Azure Blob Storage SDK, Azure Cognitive Search SDK, Chart.js 4 + chartjs-node-canvas, DataTables 2.3.5, JSZip/PDFMake for exports

- 001-remove-sql-server: Added C# 13 / .NET 10 (ASP.NET Core MVC, Razor Pages, Web API) + EF Core 10 (Sqlite provider), ASP.NET Core Identity, MSTest, WebSpark Bootswatch, DataTables, Microsoft.Extensions.Logging

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
