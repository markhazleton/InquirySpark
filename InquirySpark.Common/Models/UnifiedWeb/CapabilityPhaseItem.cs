namespace InquirySpark.Common.Models.UnifiedWeb;

/// <summary>
/// Configuration model representing a controlled capability-completion rollout stage.
/// Defines the scope, eligibility criteria, readiness checks, and rollback conditions
/// for a specific phase of InquirySpark.Web capability delivery.
/// Not an EF entity — bound from appsettings.json via IOptions.
/// Note: renamed from MigrationPhaseItem per spec terminology convention —
/// "Capability completion" = building features in InquirySpark.Web;
/// "Migration" = technical data/identity transitions only.
/// </summary>
public sealed class CapabilityPhaseItem
{
    /// <summary>Gets or sets the phase number (0=NotStarted through 4=CutOver).</summary>
    public int PhaseNumber { get; set; }

    /// <summary>Gets or sets the phase name (e.g., "NotStarted", "InProgress", "Implemented", "Validated", "CutOver").</summary>
    public string PhaseName { get; set; } = string.Empty;

    /// <summary>Gets or sets the human-readable description of this phase.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the list of capability IDs included in this phase.</summary>
    public List<string> CapabilityIds { get; set; } = [];

    /// <summary>Gets or sets the entry criteria that must be met to enter this phase.</summary>
    public List<string> EntryCriteria { get; set; } = [];

    /// <summary>Gets or sets the exit criteria that must be met to leave this phase.</summary>
    public List<string> ExitCriteria { get; set; } = [];

    /// <summary>Gets or sets the rollback conditions that would require reverting to the previous phase.</summary>
    public List<string> RollbackConditions { get; set; } = [];

    /// <summary>Gets or sets whether this phase is currently active for any domain.</summary>
    public bool IsActive { get; set; }
}
