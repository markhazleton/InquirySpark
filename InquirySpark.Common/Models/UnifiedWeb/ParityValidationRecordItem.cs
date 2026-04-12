namespace InquirySpark.Common.Models.UnifiedWeb;

/// <summary>
/// Configuration model representing evidence that a migrated capability meets
/// functional, permission, and UX expectations in InquirySpark.Web.
/// Not an EF entity — state is held in-memory and persisted via ILogger audit trail.
/// </summary>
public sealed class ParityValidationRecordItem
{
    /// <summary>Gets or sets the unique record identifier.</summary>
    public string RecordId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>Gets or sets the capability ID this record validates (e.g., "CAP-DS-001").</summary>
    public string CapabilityId { get; set; } = string.Empty;

    /// <summary>Gets or sets the validator's identity (user name or system actor).</summary>
    public string ValidatedBy { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC timestamp of the validation.</summary>
    public DateTimeOffset ValidatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Gets or sets whether functional parity was confirmed.</summary>
    public bool FunctionalParityPassed { get; set; }

    /// <summary>Gets or sets whether permission parity was confirmed.</summary>
    public bool PermissionParityPassed { get; set; }

    /// <summary>Gets or sets whether UX consistency standards were met.</summary>
    public bool UxConsistencyPassed { get; set; }

    /// <summary>Gets or sets whether the performance target (≤2s for 95% of key actions) was met.</summary>
    public bool PerformancePassed { get; set; }

    /// <summary>Gets or sets optional notes from the validator.</summary>
    public string? Notes { get; set; }

    /// <summary>Gets whether all parity checks passed.</summary>
    public bool IsFullyValidated =>
        FunctionalParityPassed && PermissionParityPassed && UxConsistencyPassed && PerformancePassed;
}
