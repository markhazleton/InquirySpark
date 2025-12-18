namespace InquirySpark.Admin.Contracts.Responses;

/// <summary>
/// Response model for chart definition operations
/// </summary>
public class ChartDefinitionResponse
{
    public int ChartDefinitionId { get; set; }
    public int DatasetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public string? FilterPayload { get; set; }
    public string? VisualPayload { get; set; }
    public string? CalculationPayload { get; set; }
    public int VersionNumber { get; set; }
    public bool AutoApprovedFl { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedDt { get; set; }
    public int ModifiedById { get; set; }
    public DateTime ModifiedDt { get; set; }
    public bool IsArchivedFl { get; set; }
}

/// <summary>
/// Response model for version history entries
/// </summary>
public class ChartVersionResponse
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

/// <summary>
/// Response model for version comparison operations
/// </summary>
public class ChartVersionComparisonResponse
{
    public int FromVersion { get; set; }
    public int ToVersion { get; set; }
    public string? NameDiff { get; set; }
    public string? DescriptionDiff { get; set; }
    public string? FilterPayloadDiff { get; set; }
    public string? VisualPayloadDiff { get; set; }
    public string? CalculationPayloadDiff { get; set; }
}

/// <summary>
/// Response model for dataset catalog entries
/// </summary>
public class DatasetCatalogResponse
{
    public int DatasetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public string? SourceRef { get; set; }
    public int? RowCount { get; set; }
    public DateTime? LastRefreshDt { get; set; }
    public bool ActiveFl { get; set; }
}
