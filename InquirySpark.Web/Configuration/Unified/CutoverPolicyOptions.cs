namespace InquirySpark.Web.Configuration.Unified;

/// <summary>
/// Options controlling the phased cutover policy for unified capability domains.
/// Bound from the "UnifiedWeb:CutoverPolicy" section of appsettings.json.
/// These options are read-only governance values; cutover state itself is maintained
/// in <see cref="InquirySpark.Repository.Services.UnifiedWeb.UnifiedWebCapabilityService"/>.
/// </summary>
public sealed class CutoverPolicyOptions
{
    /// <summary>Gets or sets the ordered list of domains eligible for cutover with their gate criteria.</summary>
    public IReadOnlyList<DomainCutoverPolicy> Domains { get; set; } = [];

    /// <summary>
    /// Gets or sets whether DryRunMode is enabled.
    /// When true, RecordCutoverDecision actions are logged but not persisted as permanent state changes.
    /// </summary>
    public bool DryRunMode { get; set; } = true;

    /// <summary>
    /// Gets or sets whether legacy fallback is allowed after a domain cutover.
    /// Per FR-006: true in development, false in production.
    /// </summary>
    public bool EnableLegacyFallback { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of days post-cutover that incident metrics must be tracked
    /// before a domain is considered permanently decommissioned.
    /// Default: 30 days per spec contract/cutover-runbook.md.
    /// </summary>
    public int PostCutoverMonitoringDays { get; set; } = 30;
}

/// <summary>Per-domain cutover governance configuration.</summary>
public sealed class DomainCutoverPolicy
{
    /// <summary>Gets or sets the domain name (must match CapabilityItem.Domain values).</summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>Gets or sets the legacy application being replaced by this domain's cutover.</summary>
    public string LegacyApp { get; set; } = string.Empty;

    /// <summary>Gets or sets the required minimum phase level for all capabilities before a Go decision may be recorded.</summary>
    public int RequiredMinimumPhase { get; set; } = 3;

    /// <summary>Gets or sets whether functional parity verification is required before this domain's cutover is permitted.</summary>
    public bool RequireFunctionalParityEvidence { get; set; } = true;

    /// <summary>Gets or sets whether permission parity verification is required before this domain's cutover is permitted.</summary>
    public bool RequirePermissionParityEvidence { get; set; } = true;

    /// <summary>Gets or sets whether performance validation is required before this domain's cutover is permitted.</summary>
    public bool RequirePerformanceValidation { get; set; } = true;

    /// <summary>Gets or sets the domain priority order (lower = cut over sooner).</summary>
    public int Order { get; set; } = 99;
}
