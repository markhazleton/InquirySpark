namespace InquirySpark.Common.Models.UnifiedWeb;

/// <summary>
/// Configuration model representing a go/no-go cutover decision for retiring
/// a legacy application entry point in favor of InquirySpark.Web.
/// Not an EF entity — state is held in-memory and persisted via ILogger audit trail.
/// </summary>
public sealed class CutoverDecisionRecordItem
{
    /// <summary>Gets or sets the unique decision record identifier.</summary>
    public string RecordId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>Gets or sets the capability domain being cut over (e.g., "DecisionSpark").</summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>Gets or sets the legacy application being retired ("DecisionSpark" or "InquirySpark.Admin").</summary>
    public string LegacyApp { get; set; } = string.Empty;

    /// <summary>Gets or sets the decision outcome ("Go", "NoGo", "Deferred").</summary>
    public string Decision { get; set; } = string.Empty;

    /// <summary>Gets or sets the identity of the approver.</summary>
    public string ApprovedBy { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC timestamp of the decision.</summary>
    public DateTimeOffset DecidedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Gets or sets the rationale or notes for the decision.</summary>
    public string? Rationale { get; set; }

    /// <summary>Gets or sets the pre-cutover gate criteria pass/fail evidence reference.</summary>
    public string? GateCriteriaEvidenceRef { get; set; }

    /// <summary>Gets or sets whether the domain has been successfully cut over.</summary>
    public bool IsCutOver { get; set; }
}
