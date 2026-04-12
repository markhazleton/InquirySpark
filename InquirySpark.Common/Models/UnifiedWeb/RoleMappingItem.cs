namespace InquirySpark.Common.Models.UnifiedWeb;

/// <summary>
/// Configuration model representing a role-to-role mapping between legacy applications and InquirySpark.Web.
/// Used to preserve role and permission semantics during capability completion (FR-004).
/// Not an EF entity — bound from appsettings.json via IOptions.
/// </summary>
public sealed class RoleMappingItem
{
    /// <summary>Gets or sets the source application identifier (e.g., "DecisionSpark", "InquirySpark.Admin").</summary>
    public string SourceApp { get; set; } = string.Empty;

    /// <summary>Gets or sets the role name as it exists in the source application.</summary>
    public string SourceRole { get; set; } = string.Empty;

    /// <summary>Gets or sets the mapped role name in InquirySpark.Web / the unified Identity store.</summary>
    public string UnifiedRole { get; set; } = string.Empty;

    /// <summary>Gets or sets an optional description of the mapping rationale.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this mapping grants equivalent or reduced privileges in the unified role.
    /// True = unified role has equivalent privileges; false = reduced (escalation must be reviewed).
    /// </summary>
    public bool IsEquivalent { get; set; } = true;
}
