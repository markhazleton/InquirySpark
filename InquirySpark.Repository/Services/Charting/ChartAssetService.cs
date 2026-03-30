using InquirySpark.Common.Models;
using InquirySpark.Repository.Configuration;
using InquirySpark.Repository.Database;
using InquirySpark.Repository.Database.Entities.Charting;
using InquirySpark.Repository.Models.Charting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Services.Charting;

public interface IChartAssetService
{
    Task<BaseResponse<ChartAssetDto>> CreateAssetAsync(ChartAssetDto asset);
    Task<BaseResponse<ChartAssetDto>> GetAssetAsync(int chartAssetId);
    Task<BaseResponseCollection<ChartAssetDto>> GetAssetsByDefinitionAsync(int chartDefinitionId);
    Task<BaseResponseCollection<ChartAssetDto>> SearchAssetsAsync(string? searchText, string? tags, string? approvalStatus, int pageSize = 50, int pageIndex = 0);
    Task<BaseResponse<bool>> UpdateApprovalStatusAsync(int chartAssetId, string approvalStatus, int? approvedById = null);
    Task<BaseResponse<bool>> IncrementUsageCountAsync(int chartAssetId);
    Task<BaseResponse<bool>> AddAssetFileAsync(int chartAssetId, ChartAssetFileDto fileDto);
    Task<BaseResponse<bool>> DeleteAssetAsync(int chartAssetId);
    Task<BaseResponse<AssetStorageStatsDto>> GetStorageStatsAsync();
}

public class ChartAssetDto
{
    public int ChartAssetId { get; set; }
    public int ChartDefinitionId { get; set; }
    public int ChartVersionId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public DateTime GenerationDt { get; set; }
    public DateTime DataSnapshotDt { get; set; }
    public string ApprovalStatus { get; set; } = "Draft";
    public int UsageCount { get; set; }
    public DateTime? LastAccessedDt { get; set; }
    public string? CdnBaseUrl { get; set; }
    public string? CommentsJson { get; set; }
    public List<ChartAssetFileDto> Files { get; set; } = new();
    public string? ChartDefinitionName { get; set; }
}

public class ChartAssetFileDto
{
    public int ChartAssetFileId { get; set; }
    public int ChartAssetId { get; set; }
    public string Format { get; set; } = string.Empty;
    public string? ResolutionHint { get; set; }
    public string BlobPath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string? Checksum { get; set; }
    public DateTime? ExpiresDt { get; set; }
}

public class AssetStorageStatsDto
{
    public int TotalAssets { get; set; }
    public int TotalFiles { get; set; }
    public long TotalSizeBytes { get; set; }
    public int ApprovedAssets { get; set; }
    public int DraftAssets { get; set; }
    public int ArchivedAssets { get; set; }
    public DateTime? OldestAssetDate { get; set; }
    public DateTime? NewestAssetDate { get; set; }
}

public class ChartAssetService(InquirySparkContext context, ILogger<ChartAssetService> logger) : IChartAssetService
{
    private readonly InquirySparkContext _context = context;
    private readonly ILogger<ChartAssetService> _logger = logger;

    /// <summary>
    /// Creates a new chart asset record with metadata
    /// </summary>
    public async Task<BaseResponse<ChartAssetDto>> CreateAssetAsync(ChartAssetDto assetDto)
    {
        return await DbContextHelper.ExecuteAsync<ChartAssetDto>(async () =>
        {
            var asset = new ChartAssetEntity
            {
                ChartDefinitionId = assetDto.ChartDefinitionId,
                ChartVersionId = assetDto.ChartVersionId,
                DisplayName = assetDto.DisplayName,
                Description = assetDto.Description,
                Tags = assetDto.Tags,
                GenerationDt = DateTime.UtcNow,
                DataSnapshotDt = assetDto.DataSnapshotDt,
                ApprovalStatus = assetDto.ApprovalStatus,
                UsageCount = 0,
                CdnBaseUrl = assetDto.CdnBaseUrl,
                CommentsJson = assetDto.CommentsJson
            };

            _context.ChartAssets.Add(asset);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created chart asset {AssetId} for definition {DefinitionId}", 
                asset.ChartAssetId, asset.ChartDefinitionId);

            // Reload with navigation properties
            var created = await _context.ChartAssets
                .Include(a => a.ChartDefinition)
                .Include(a => a.Files)
                .FirstOrDefaultAsync(a => a.ChartAssetId == asset.ChartAssetId);

            return MapAssetToDto(created!);
        });
    }

    /// <summary>
    /// Retrieves a chart asset by ID with all files
    /// </summary>
    public async Task<BaseResponse<ChartAssetDto>> GetAssetAsync(int chartAssetId)
    {
        return await DbContextHelper.ExecuteAsync<ChartAssetDto>(async () =>
        {
            var asset = await _context.ChartAssets
                .Include(a => a.ChartDefinition)
                .Include(a => a.Files)
                .FirstOrDefaultAsync(a => a.ChartAssetId == chartAssetId);

            if (asset == null)
            {
                throw new KeyNotFoundException($"Chart asset {chartAssetId} not found");
            }

            return MapAssetToDto(asset);
        });
    }

    /// <summary>
    /// Gets all assets for a specific chart definition
    /// </summary>
    public async Task<BaseResponseCollection<ChartAssetDto>> GetAssetsByDefinitionAsync(int chartDefinitionId)
    {
        return await DbContextHelper.ExecuteCollectionAsync<ChartAssetDto>(async () =>
        {
            var assets = await _context.ChartAssets
                .Include(a => a.ChartDefinition)
                .Include(a => a.Files)
                .Where(a => a.ChartDefinitionId == chartDefinitionId)
                .OrderByDescending(a => a.GenerationDt)
                .ToListAsync();

            return assets.Select(MapAssetToDto).ToList();
        });
    }

    /// <summary>
    /// Searches chart assets with filters and pagination
    /// </summary>
    public async Task<BaseResponseCollection<ChartAssetDto>> SearchAssetsAsync(
        string? searchText, 
        string? tags, 
        string? approvalStatus, 
        int pageSize = 50, 
        int pageIndex = 0)
    {
        return await DbContextHelper.ExecuteCollectionAsync<ChartAssetDto>(async () =>
        {
            var query = _context.ChartAssets
                .Include(a => a.ChartDefinition)
                .Include(a => a.Files)
                .AsQueryable();

            // Filter by search text (matches display name or description)
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var search = searchText.ToLower();
                query = query.Where(a => 
                    a.DisplayName.ToLower().Contains(search) || 
                    (a.Description != null && a.Description.ToLower().Contains(search)));
            }

            // Filter by tags
            if (!string.IsNullOrWhiteSpace(tags))
            {
                var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim().ToLower())
                    .ToList();
                
                query = query.Where(a => a.Tags != null && 
                    tagList.Any(tag => a.Tags.ToLower().Contains(tag)));
            }

            // Filter by approval status
            if (!string.IsNullOrWhiteSpace(approvalStatus))
            {
                query = query.Where(a => a.ApprovalStatus == approvalStatus);
            }

            // Apply pagination
            var assets = await query
                .OrderByDescending(a => a.GenerationDt)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Asset search returned {Count} results (page {PageIndex}, size {PageSize})", 
                assets.Count, pageIndex, pageSize);

            return assets.Select(MapAssetToDto).ToList();
        });
    }

    /// <summary>
    /// Updates the approval status of an asset
    /// </summary>
    public async Task<BaseResponse<bool>> UpdateApprovalStatusAsync(int chartAssetId, string approvalStatus, int? approvedById = null)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            var asset = await _context.ChartAssets
                .FirstOrDefaultAsync(a => a.ChartAssetId == chartAssetId);

            if (asset == null)
            {
                throw new KeyNotFoundException($"Chart asset {chartAssetId} not found");
            }

            var oldStatus = asset.ApprovalStatus;
            asset.ApprovalStatus = approvalStatus;

            // Update comments with approval history
            if (approvalStatus == "Approved" && approvedById.HasValue)
            {
                var comment = $"Approved by user {approvedById} on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                asset.CommentsJson = string.IsNullOrEmpty(asset.CommentsJson) 
                    ? $"[{comment}]" 
                    : asset.CommentsJson.TrimEnd(']') + $",{comment}]";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Asset {AssetId} status changed: {OldStatus} → {NewStatus}", 
                chartAssetId, oldStatus, approvalStatus);

            return true;
        });
    }

    /// <summary>
    /// Increments the usage count when an asset is accessed
    /// </summary>
    public async Task<BaseResponse<bool>> IncrementUsageCountAsync(int chartAssetId)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            var asset = await _context.ChartAssets
                .FirstOrDefaultAsync(a => a.ChartAssetId == chartAssetId);

            if (asset == null)
            {
                throw new KeyNotFoundException($"Chart asset {chartAssetId} not found");
            }

            asset.UsageCount++;
            asset.LastAccessedDt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        });
    }

    /// <summary>
    /// Adds a file (PNG, SVG, PDF, etc.) to an asset
    /// </summary>
    public async Task<BaseResponse<bool>> AddAssetFileAsync(int chartAssetId, ChartAssetFileDto fileDto)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            var asset = await _context.ChartAssets
                .FirstOrDefaultAsync(a => a.ChartAssetId == chartAssetId);

            if (asset == null)
            {
                throw new KeyNotFoundException($"Chart asset {chartAssetId} not found");
            }

            var file = new ChartAssetFileEntity
            {
                ChartAssetId = chartAssetId,
                Format = fileDto.Format,
                ResolutionHint = fileDto.ResolutionHint,
                BlobPath = fileDto.BlobPath,
                FileSizeBytes = fileDto.FileSizeBytes,
                Checksum = fileDto.Checksum,
                ExpiresDt = fileDto.ExpiresDt
            };

            _context.ChartAssetFiles.Add(file);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added {Format} file to asset {AssetId} (Path: {BlobPath})", 
                fileDto.Format, chartAssetId, fileDto.BlobPath);

            return true;
        });
    }

    /// <summary>
    /// Soft deletes an asset by changing approval status to Archived
    /// </summary>
    public async Task<BaseResponse<bool>> DeleteAssetAsync(int chartAssetId)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            var asset = await _context.ChartAssets
                .FirstOrDefaultAsync(a => a.ChartAssetId == chartAssetId);

            if (asset == null)
            {
                throw new KeyNotFoundException($"Chart asset {chartAssetId} not found");
            }

            asset.ApprovalStatus = "Archived";
            await _context.SaveChangesAsync();

            _logger.LogInformation("Archived chart asset {AssetId}", chartAssetId);

            return true;
        });
    }

    /// <summary>
    /// Retrieves storage statistics across all assets
    /// </summary>
    public async Task<BaseResponse<AssetStorageStatsDto>> GetStorageStatsAsync()
    {
        return await DbContextHelper.ExecuteAsync<AssetStorageStatsDto>(async () =>
        {
            var assets = await _context.ChartAssets.ToListAsync();
            var files = await _context.ChartAssetFiles.ToListAsync();

            var stats = new AssetStorageStatsDto
            {
                TotalAssets = assets.Count,
                TotalFiles = files.Count,
                TotalSizeBytes = files.Sum(f => f.FileSizeBytes),
                ApprovedAssets = assets.Count(a => a.ApprovalStatus == "Approved"),
                DraftAssets = assets.Count(a => a.ApprovalStatus == "Draft"),
                ArchivedAssets = assets.Count(a => a.ApprovalStatus == "Archived"),
                OldestAssetDate = assets.Any() ? assets.Min(a => a.GenerationDt) : null,
                NewestAssetDate = assets.Any() ? assets.Max(a => a.GenerationDt) : null
            };

            return stats;
        });
    }

    private static ChartAssetDto MapAssetToDto(ChartAssetEntity asset)
    {
        return new ChartAssetDto
        {
            ChartAssetId = asset.ChartAssetId,
            ChartDefinitionId = asset.ChartDefinitionId,
            ChartVersionId = asset.ChartVersionId,
            DisplayName = asset.DisplayName,
            Description = asset.Description,
            Tags = asset.Tags,
            GenerationDt = asset.GenerationDt,
            DataSnapshotDt = asset.DataSnapshotDt,
            ApprovalStatus = asset.ApprovalStatus,
            UsageCount = asset.UsageCount,
            LastAccessedDt = asset.LastAccessedDt,
            CdnBaseUrl = asset.CdnBaseUrl,
            CommentsJson = asset.CommentsJson,
            ChartDefinitionName = asset.ChartDefinition?.Name,
            Files = asset.Files?.Select(f => new ChartAssetFileDto
            {
                ChartAssetFileId = f.ChartAssetFileId,
                ChartAssetId = f.ChartAssetId,
                Format = f.Format,
                ResolutionHint = f.ResolutionHint,
                BlobPath = f.BlobPath,
                FileSizeBytes = f.FileSizeBytes,
                Checksum = f.Checksum,
                ExpiresDt = f.ExpiresDt
            }).ToList() ?? new List<ChartAssetFileDto>()
        };
    }
}
