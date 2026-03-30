using InquirySpark.Common.Models;
using InquirySpark.Repository.Configuration;
using InquirySpark.Repository.Database;
using InquirySpark.Repository.Database.Entities.Charting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Services.DataExplorer;

public interface IDataExportService
{
    Task<BaseResponse<DataExportRequestDto>> CreateExportRequestAsync(int chartDefinitionId, int requestedById, DataExportOptionsDto options);
    Task<BaseResponse<DataExportRequestDto>> GetExportRequestAsync(int exportRequestId);
    Task<BaseResponseCollection<DataExportRequestDto>> GetUserExportRequestsAsync(int userId, int limit = 20);
    Task<BaseResponse<bool>> UpdateExportStatusAsync(int exportRequestId, string status, string? blobPath = null);
    Task<BaseResponse<bool>> DeleteExpiredExportsAsync(int retentionDays = 7);
}

public class DataExportOptionsDto
{
    public string Format { get; set; } = "Xlsx"; // Xlsx, Pdf, Csv, Tsv
    public string? FilterPayload { get; set; }
    public string? ColumnSettingsJson { get; set; }
    public bool IncludeMetadata { get; set; } = true;
    public int MaxRows { get; set; } = 50000;
}

public class DataExportRequestDto
{
    public int DataExportRequestId { get; set; }
    public int ChartDefinitionId { get; set; }
    public int RequestedById { get; set; }
    public DateTime RequestedDt { get; set; }
    public string? FilterPayload { get; set; }
    public string? ColumnSettingsJson { get; set; }
    public string Format { get; set; } = string.Empty;
    public int RowCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CompletionDt { get; set; }
    public string? BlobPath { get; set; }
    public string? ChartDefinitionName { get; set; }
}

public class DataExportService(
    InquirySparkContext context,
    ILogger<DataExportService> logger) : IDataExportService
{
    private readonly InquirySparkContext _context = context;
    private readonly ILogger<DataExportService> _logger = logger;
    private const int MaxRowCap = 50000; // Hard limit per spec

    /// <summary>
    /// Creates a new export request and queues it for background processing
    /// </summary>
    public async Task<BaseResponse<DataExportRequestDto>> CreateExportRequestAsync(
        int chartDefinitionId,
        int requestedById,
        DataExportOptionsDto options)
    {
        return await DbContextHelper.ExecuteAsync<DataExportRequestDto>(async () =>
        {
            // Validate chart definition exists
            var chartDef = await _context.ChartDefinitions
                .FirstOrDefaultAsync(c => c.ChartDefinitionId == chartDefinitionId);

            if (chartDef == null)
            {
                throw new KeyNotFoundException($"Chart definition {chartDefinitionId} not found");
            }

            // Enforce row cap
            var rowLimit = Math.Min(options.MaxRows, MaxRowCap);

            // Create export request
            var exportRequest = new DataExportRequestEntity
            {
                ChartDefinitionId = chartDefinitionId,
                RequestedById = requestedById,
                RequestedDt = DateTime.UtcNow,
                FilterPayload = options.FilterPayload,
                ColumnSettingsJson = options.ColumnSettingsJson,
                Format = options.Format,
                RowCount = 0, // Will be updated during processing
                Status = "Pending"
            };

            _context.DataExportRequests.Add(exportRequest);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Created export request {ExportId} for chart {ChartId} by user {UserId} (Format: {Format}, Max rows: {MaxRows})",
                exportRequest.DataExportRequestId, chartDefinitionId, requestedById, options.Format, rowLimit);

            // Reload with navigation properties
            var created = await _context.DataExportRequests
                .Include(e => e.ChartDefinition)
                .FirstOrDefaultAsync(e => e.DataExportRequestId == exportRequest.DataExportRequestId);

            return MapToDto(created!);
        });
    }

    /// <summary>
    /// Retrieves an export request by ID
    /// </summary>
    public async Task<BaseResponse<DataExportRequestDto>> GetExportRequestAsync(int exportRequestId)
    {
        return await DbContextHelper.ExecuteAsync<DataExportRequestDto>(async () =>
        {
            var request = await _context.DataExportRequests
                .Include(e => e.ChartDefinition)
                .FirstOrDefaultAsync(e => e.DataExportRequestId == exportRequestId);

            if (request == null)
            {
                throw new KeyNotFoundException($"Export request {exportRequestId} not found");
            }

            return MapToDto(request);
        });
    }

    /// <summary>
    /// Gets recent export requests for a specific user
    /// </summary>
    public async Task<BaseResponseCollection<DataExportRequestDto>> GetUserExportRequestsAsync(int userId, int limit = 20)
    {
        return await DbContextHelper.ExecuteCollectionAsync<DataExportRequestDto>(async () =>
        {
            var requests = await _context.DataExportRequests
                .Include(e => e.ChartDefinition)
                .Where(e => e.RequestedById == userId)
                .OrderByDescending(e => e.RequestedDt)
                .Take(limit)
                .ToListAsync();

            return requests.Select(MapToDto).ToList();
        });
    }

    /// <summary>
    /// Updates the status of an export request (called by background workers)
    /// </summary>
    public async Task<BaseResponse<bool>> UpdateExportStatusAsync(
        int exportRequestId,
        string status,
        string? blobPath = null)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            var request = await _context.DataExportRequests
                .FirstOrDefaultAsync(e => e.DataExportRequestId == exportRequestId);

            if (request == null)
            {
                throw new KeyNotFoundException($"Export request {exportRequestId} not found");
            }

            var oldStatus = request.Status;
            request.Status = status;

            if (!string.IsNullOrEmpty(blobPath))
            {
                request.BlobPath = blobPath;
            }

            if (status == "Completed" || status == "Failed")
            {
                request.CompletionDt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Export request {ExportId} status updated: {OldStatus} → {NewStatus}",
                exportRequestId, oldStatus, status);

            return true;
        });
    }

    /// <summary>
    /// Deletes export requests and files older than retention period
    /// </summary>
    public async Task<BaseResponse<bool>> DeleteExpiredExportsAsync(int retentionDays = 7)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

            var expiredRequests = await _context.DataExportRequests
                .Where(e => e.RequestedDt < cutoffDate)
                .ToListAsync();

            if (expiredRequests.Any())
            {
                _context.DataExportRequests.RemoveRange(expiredRequests);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted {Count} expired export requests older than {CutoffDate}",
                    expiredRequests.Count, cutoffDate);
            }

            return true;
        });
    }

    private static DataExportRequestDto MapToDto(DataExportRequestEntity entity)
    {
        return new DataExportRequestDto
        {
            DataExportRequestId = entity.DataExportRequestId,
            ChartDefinitionId = entity.ChartDefinitionId,
            RequestedById = entity.RequestedById,
            RequestedDt = entity.RequestedDt,
            FilterPayload = entity.FilterPayload,
            ColumnSettingsJson = entity.ColumnSettingsJson,
            Format = entity.Format,
            RowCount = entity.RowCount,
            Status = entity.Status,
            CompletionDt = entity.CompletionDt,
            BlobPath = entity.BlobPath,
            ChartDefinitionName = entity.ChartDefinition?.Name
        };
    }
}
