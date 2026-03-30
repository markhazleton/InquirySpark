using InquirySpark.Common.Models;
using InquirySpark.Repository.Database;
using InquirySpark.Repository.Database.Entities.Charting;
using InquirySpark.Repository.Models.Charting;
using InquirySpark.Repository.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Services.Charting;

public interface IChartDefinitionService
{
    Task<BaseResponse<ChartDefinitionDto>> GetDefinitionAsync(int chartDefinitionId);
    Task<BaseResponse<ChartDefinitionDto>> SaveDefinitionAsync(ChartDefinitionDto definition);
    Task<BaseResponse<bool>> DeleteAsync(int chartDefinitionId);
    Task<BaseResponseCollection<ChartDefinitionDto>> GetDatasetCatalogAsync();
    Task<BaseResponseCollection<ChartVersionDto>> GetVersionHistoryAsync(int chartDefinitionId);
    Task<BaseResponse<ChartDefinitionDto>> RollbackToVersionAsync(int chartDefinitionId, int versionNumber, int userId);
    Task<BaseResponse<ChartVersionComparisonDto>> CompareVersionsAsync(int chartDefinitionId, int fromVersion, int toVersion);
}

public class ChartVersionDto
{
    public int ChartVersionId { get; set; }
    public int ChartDefinitionId { get; set; }
    public int VersionNumber { get; set; }
    public string? SnapshotPayload { get; set; }
    public bool ApprovedFl { get; set; }
    public int? ApprovedById { get; set; }
    public DateTime? ApprovedDt { get; set; }
    public string? DiffSummary { get; set; }
    public int? RollbackSourceVersionNumber { get; set; }
}

public class ChartVersionComparisonDto
{
    public int FromVersion { get; set; }
    public int ToVersion { get; set; }
    public string? NameDiff { get; set; }
    public string? DescriptionDiff { get; set; }
    public string? FilterPayloadDiff { get; set; }
    public string? VisualPayloadDiff { get; set; }
    public string? CalculationPayloadDiff { get; set; }
}

public class ChartDefinitionService(InquirySparkContext context, ILogger<ChartDefinitionService> logger) : IChartDefinitionService
{
    private readonly InquirySparkContext _context = context;
    private readonly ILogger<ChartDefinitionService> _logger = logger;

    public async Task<BaseResponse<ChartDefinitionDto>> GetDefinitionAsync(int chartDefinitionId)
    {
        return await DbContextHelper.ExecuteAsync<ChartDefinitionDto>(async () =>
        {
            var entity = await _context.ChartDefinitions
                .FirstOrDefaultAsync(c => c.ChartDefinitionId == chartDefinitionId);

            if (entity == null)
                return null;

            return new ChartDefinitionDto
            {
                ChartDefinitionId = entity.ChartDefinitionId,
                DatasetId = entity.DatasetId,
                Name = entity.Name,
                Description = entity.Description,
                Tags = entity.Tags,
                FilterPayload = entity.FilterPayload,
                VisualPayload = entity.VisualPayload,
                CalculationPayload = entity.CalculationPayload,
                VersionNumber = entity.VersionNumber,
                AutoApprovedFl = entity.AutoApprovedFl,
                CreatedById = entity.CreatedById,
                CreatedDt = entity.CreatedDt,
                ModifiedById = entity.ModifiedById,
                ModifiedDt = entity.ModifiedDt,
                IsArchivedFl = entity.IsArchivedFl
            };
        });
    }

    public async Task<BaseResponse<ChartDefinitionDto>> SaveDefinitionAsync(ChartDefinitionDto definition)
    {
        return await DbContextHelper.ExecuteAsync<ChartDefinitionDto>(async () =>
        {
            ChartDefinitionEntity entity;
            
            if (definition.ChartDefinitionId > 0)
            {
                entity = await _context.ChartDefinitions
                    .FirstOrDefaultAsync(c => c.ChartDefinitionId == definition.ChartDefinitionId);
                
                if (entity == null)
                    throw new InvalidOperationException($"Chart definition {definition.ChartDefinitionId} not found");
                
                // Create snapshot before updating
                var snapshotVersion = new ChartVersionEntity
                {
                    ChartDefinitionId = entity.ChartDefinitionId,
                    VersionNumber = entity.VersionNumber,
                    SnapshotPayload = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        entity.DatasetId,
                        entity.Name,
                        entity.Description,
                        entity.Tags,
                        entity.FilterPayload,
                        entity.VisualPayload,
                        entity.CalculationPayload
                    }),
                    ApprovedFl = entity.AutoApprovedFl,
                    DiffSummary = $"Version {entity.VersionNumber} snapshot"
                };
                _context.ChartVersions.Add(snapshotVersion);
                
                entity.VersionNumber++;
            }
            else
            {
                entity = new ChartDefinitionEntity
                {
                    VersionNumber = 1,
                    CreatedDt = DateTime.UtcNow,
                    CreatedById = definition.CreatedById
                };
                _context.ChartDefinitions.Add(entity);
            }

            entity.DatasetId = definition.DatasetId;
            entity.Name = definition.Name;
            entity.Description = definition.Description;
            entity.Tags = definition.Tags;
            entity.FilterPayload = definition.FilterPayload;
            entity.VisualPayload = definition.VisualPayload;
            entity.CalculationPayload = definition.CalculationPayload;
            entity.AutoApprovedFl = definition.AutoApprovedFl;
            entity.ModifiedById = definition.ModifiedById;
            entity.ModifiedDt = DateTime.UtcNow;
            entity.IsArchivedFl = definition.IsArchivedFl;

            await _context.SaveChangesAsync();

            definition.ChartDefinitionId = entity.ChartDefinitionId;
            definition.VersionNumber = entity.VersionNumber;
            definition.ModifiedDt = entity.ModifiedDt;

            return definition;
        });
    }

    public async Task<BaseResponse<bool>> DeleteAsync(int chartDefinitionId)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            var entity = await _context.ChartDefinitions
                .FirstOrDefaultAsync(c => c.ChartDefinitionId == chartDefinitionId);
            
            if (entity == null)
                return false;

            entity.IsArchivedFl = true;
            entity.ModifiedDt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        });
    }

    public async Task<BaseResponseCollection<ChartDefinitionDto>> GetDatasetCatalogAsync()
    {
        return await DbContextHelper.ExecuteCollectionAsync<ChartDefinitionDto>(async () =>
        {
            var definitions = await _context.ChartDefinitions
                .Where(c => !c.IsArchivedFl)
                .Select(c => new ChartDefinitionDto
                {
                    ChartDefinitionId = c.ChartDefinitionId,
                    DatasetId = c.DatasetId,
                    Name = c.Name,
                    Description = c.Description,
                    Tags = c.Tags,
                    VersionNumber = c.VersionNumber,
                    AutoApprovedFl = c.AutoApprovedFl,
                    CreatedById = c.CreatedById,
                    CreatedDt = c.CreatedDt,
                    ModifiedById = c.ModifiedById,
                    ModifiedDt = c.ModifiedDt
                })
                .ToListAsync();

            return definitions;
        });
    }

    public async Task<BaseResponseCollection<ChartVersionDto>> GetVersionHistoryAsync(int chartDefinitionId)
    {
        return await DbContextHelper.ExecuteCollectionAsync<ChartVersionDto>(async () =>
        {
            var versions = await _context.ChartVersions
                .Where(v => v.ChartDefinitionId == chartDefinitionId)
                .OrderByDescending(v => v.VersionNumber)
                .Select(v => new ChartVersionDto
                {
                    ChartVersionId = v.ChartVersionId,
                    ChartDefinitionId = v.ChartDefinitionId,
                    VersionNumber = v.VersionNumber,
                    SnapshotPayload = v.SnapshotPayload,
                    ApprovedFl = v.ApprovedFl,
                    ApprovedById = v.ApprovedById,
                    ApprovedDt = v.ApprovedDt,
                    DiffSummary = v.DiffSummary,
                    RollbackSourceVersionNumber = v.RollbackSourceVersionNumber
                })
                .ToListAsync();

            return versions;
        });
    }

    public async Task<BaseResponse<ChartDefinitionDto>> RollbackToVersionAsync(int chartDefinitionId, int versionNumber, int userId)
    {
        return await DbContextHelper.ExecuteAsync<ChartDefinitionDto>(async () =>
        {
            var definition = await _context.ChartDefinitions
                .FirstOrDefaultAsync(c => c.ChartDefinitionId == chartDefinitionId);
            
            if (definition == null)
                throw new InvalidOperationException($"Chart definition {chartDefinitionId} not found");

            var targetVersion = await _context.ChartVersions
                .FirstOrDefaultAsync(v => v.ChartDefinitionId == chartDefinitionId && v.VersionNumber == versionNumber);
            
            if (targetVersion == null)
                throw new InvalidOperationException($"Version {versionNumber} not found for chart {chartDefinitionId}");

            // Create snapshot of current state before rollback
            var currentSnapshot = new ChartVersionEntity
            {
                ChartDefinitionId = chartDefinitionId,
                VersionNumber = definition.VersionNumber,
                SnapshotPayload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    definition.DatasetId,
                    definition.Name,
                    definition.Description,
                    definition.Tags,
                    definition.FilterPayload,
                    definition.VisualPayload,
                    definition.CalculationPayload
                }),
                ApprovedFl = definition.AutoApprovedFl,
                DiffSummary = $"Version {definition.VersionNumber} before rollback to version {versionNumber}"
            };
            _context.ChartVersions.Add(currentSnapshot);

            // Restore from target version
            var snapshotData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(targetVersion.SnapshotPayload!);
            
            definition.DatasetId = snapshotData.GetProperty("DatasetId").GetInt32();
            definition.Name = snapshotData.GetProperty("Name").GetString()!;
            definition.Description = snapshotData.TryGetProperty("Description", out var desc) ? desc.GetString() : null;
            definition.Tags = snapshotData.TryGetProperty("Tags", out var tags) ? tags.GetString() : null;
            definition.FilterPayload = snapshotData.TryGetProperty("FilterPayload", out var filter) ? filter.GetString() : null;
            definition.VisualPayload = snapshotData.TryGetProperty("VisualPayload", out var visual) ? visual.GetString() : null;
            definition.CalculationPayload = snapshotData.TryGetProperty("CalculationPayload", out var calc) ? calc.GetString() : null;
            definition.VersionNumber++;
            definition.ModifiedById = userId;
            definition.ModifiedDt = DateTime.UtcNow;

            // Create new version record for the rollback
            var rollbackVersion = new ChartVersionEntity
            {
                ChartDefinitionId = chartDefinitionId,
                VersionNumber = definition.VersionNumber,
                SnapshotPayload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    definition.DatasetId,
                    definition.Name,
                    definition.Description,
                    definition.Tags,
                    definition.FilterPayload,
                    definition.VisualPayload,
                    definition.CalculationPayload
                }),
                ApprovedFl = false, // Rolled back versions require re-approval
                RollbackSourceVersionNumber = versionNumber,
                DiffSummary = $"Rolled back from version {definition.VersionNumber - 1} to version {versionNumber}"
            };
            _context.ChartVersions.Add(rollbackVersion);

            await _context.SaveChangesAsync();

            return new ChartDefinitionDto
            {
                ChartDefinitionId = definition.ChartDefinitionId,
                DatasetId = definition.DatasetId,
                Name = definition.Name,
                Description = definition.Description,
                Tags = definition.Tags,
                FilterPayload = definition.FilterPayload,
                VisualPayload = definition.VisualPayload,
                CalculationPayload = definition.CalculationPayload,
                VersionNumber = definition.VersionNumber,
                AutoApprovedFl = definition.AutoApprovedFl,
                CreatedById = definition.CreatedById,
                CreatedDt = definition.CreatedDt,
                ModifiedById = definition.ModifiedById,
                ModifiedDt = definition.ModifiedDt,
                IsArchivedFl = definition.IsArchivedFl
            };
        });
    }

    public async Task<BaseResponse<ChartVersionComparisonDto>> CompareVersionsAsync(int chartDefinitionId, int fromVersion, int toVersion)
    {
        return await DbContextHelper.ExecuteAsync<ChartVersionComparisonDto>(async () =>
        {
            var versions = await _context.ChartVersions
                .Where(v => v.ChartDefinitionId == chartDefinitionId && 
                           (v.VersionNumber == fromVersion || v.VersionNumber == toVersion))
                .ToListAsync();

            if (versions.Count != 2)
                throw new InvalidOperationException($"Could not find both versions {fromVersion} and {toVersion} for chart {chartDefinitionId}");

            var from = versions.First(v => v.VersionNumber == fromVersion);
            var to = versions.First(v => v.VersionNumber == toVersion);

            var fromData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(from.SnapshotPayload!);
            var toData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(to.SnapshotPayload!);

            var comparison = new ChartVersionComparisonDto
            {
                FromVersion = fromVersion,
                ToVersion = toVersion
            };

            // Compare Name
            var fromName = fromData.GetProperty("Name").GetString();
            var toName = toData.GetProperty("Name").GetString();
            if (fromName != toName)
                comparison.NameDiff = $"'{fromName}' → '{toName}'";

            // Compare Description
            var fromDesc = fromData.TryGetProperty("Description", out var fd) ? fd.GetString() : null;
            var toDesc = toData.TryGetProperty("Description", out var td) ? td.GetString() : null;
            if (fromDesc != toDesc)
                comparison.DescriptionDiff = $"Changed";

            // Compare payloads (simplified - just indicate if changed)
            var fromFilter = fromData.TryGetProperty("FilterPayload", out var ff) ? ff.GetString() : null;
            var toFilter = toData.TryGetProperty("FilterPayload", out var tf) ? tf.GetString() : null;
            if (fromFilter != toFilter)
                comparison.FilterPayloadDiff = "Filters modified";

            var fromVisual = fromData.TryGetProperty("VisualPayload", out var fv) ? fv.GetString() : null;
            var toVisual = toData.TryGetProperty("VisualPayload", out var tv) ? tv.GetString() : null;
            if (fromVisual != toVisual)
                comparison.VisualPayloadDiff = "Visual configuration modified";

            var fromCalc = fromData.TryGetProperty("CalculationPayload", out var fc) ? fc.GetString() : null;
            var toCalc = toData.TryGetProperty("CalculationPayload", out var tc) ? tc.GetString() : null;
            if (fromCalc != toCalc)
                comparison.CalculationPayloadDiff = "Calculation logic modified";

            return comparison;
        });
    }
}
