#nullable enable
namespace InquirySpark.Common.Models.UnifiedWeb;

/// <summary>
/// Configuration model representing a single inventoried capability from a legacy application.
/// Tracks parity status, delivery phase, and evidence artifacts for FR-001 and FR-010.
/// Not an EF entity — bound from appsettings.json via IOptions.
/// </summary>
public sealed class CapabilityItem
{
    /// <summary>Gets or sets the unique capability identifier (e.g., "CAP-DS-001").</summary>
    public string CapabilityId { get; set; } = string.Empty;

    /// <summary>Gets or sets the domain this capability belongs to (e.g., "DecisionSpark").</summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>Gets or sets the human-readable capability name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the source legacy application ("DecisionSpark" or "InquirySpark.Admin").</summary>
    public string SourceApp { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current completion phase (0=NotStarted, 1=InProgress, 2=Implemented, 3=Validated, 4=CutOver).
    /// </summary>
    public int Phase { get; set; }

    /// <summary>
    /// Gets or sets the current status string ("not-started", "in-progress", "implemented", "validated", "cut-over").
    /// </summary>
    public string Status { get; set; } = "not-started";

    /// <summary>Gets or sets the delivery priority (1=P1 highest, 3=P3 lowest).</summary>
    public int Priority { get; set; } = 1;

    /// <summary>Gets or sets an optional note about the current implementation state.</summary>
    public string? Notes { get; set; }
}
