namespace InquirySpark.Common.Models.UnifiedWeb;

/// <summary>
/// Configuration model representing a logical domain grouping of capabilities in InquirySpark.Web.
/// Examples: "DecisionSpark", "InquiryAdministration", "InquiryAuthoring", "InquiryOperations".
/// Not an EF entity — bound from appsettings.json via IOptions.
/// </summary>
public sealed class CapabilityDomainItem
{
    /// <summary>Gets or sets the unique domain identifier (e.g., "DecisionSpark").</summary>
    public string DomainId { get; set; } = string.Empty;

    /// <summary>Gets or sets the human-readable domain name.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Gets or sets the source legacy application ("DecisionSpark" or "InquirySpark.Admin").</summary>
    public string SourceApp { get; set; } = string.Empty;

    /// <summary>Gets or sets the unified controller serving this domain (e.g., "DecisionConversation").</summary>
    public string UnifiedController { get; set; } = string.Empty;

    /// <summary>Gets or sets an optional description of the domain.</summary>
    public string? Description { get; set; }
}
