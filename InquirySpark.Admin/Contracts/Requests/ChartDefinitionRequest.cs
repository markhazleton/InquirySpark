using System.ComponentModel.DataAnnotations;

namespace InquirySpark.Admin.Contracts.Requests;

/// <summary>
/// Request model for creating or updating chart definitions
/// </summary>
public class ChartDefinitionRequest
{
    public int ChartDefinitionId { get; set; }

    [Required]
    public int DatasetId { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? Tags { get; set; }

    /// <summary>
    /// JSON payload containing filter configuration (AND/OR tree structure)
    /// </summary>
    public string? FilterPayload { get; set; }

    /// <summary>
    /// JSON payload matching Chart.js configuration schema
    /// </summary>
    public string? VisualPayload { get; set; }

    /// <summary>
    /// JSON payload containing calculation expressions and metadata
    /// </summary>
    public string? CalculationPayload { get; set; }

    public bool AutoApprovedFl { get; set; }
}

/// <summary>
/// Request model for rolling back to a previous version
/// </summary>
public class ChartRollbackRequest
{
    [Required]
    public int ChartDefinitionId { get; set; }

    [Required]
    public int VersionNumber { get; set; }
}

/// <summary>
/// Request model for comparing two versions
/// </summary>
public class ChartVersionComparisonRequest
{
    [Required]
    public int ChartDefinitionId { get; set; }

    [Required]
    public int FromVersion { get; set; }

    [Required]
    public int ToVersion { get; set; }
}
